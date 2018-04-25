using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Modular.Core;
using Modular.Infrastructure;
using Modular.Module.Core.Infrastructure;
using Modular.WebHost.Extensions;

namespace Modular.WebHost
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ModularDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Modular.WebHost")));

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());
            });

            List<ModuleInfo> modules = new List<ModuleInfo>();
            // Load modules
            var moduleRootFolder = _hostingEnvironment.ContentRootFileProvider.GetDirectoryContents("Modules");
            foreach (var modularFolder in moduleRootFolder.Where(x => x.IsDirectory))
            {
                var binFolder = new DirectoryInfo(Path.Combine(modularFolder.PhysicalPath, "bin"));
                if (!binFolder.Exists)
                    continue;

                foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly;

                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException ex)
                    {
                        if (ex.Message == "Assembly with same name is already loaded")
                        {
                            continue;
                        }
                        throw;
                    }

                    if (assembly.FullName.Contains(modularFolder.Name) && !assembly.FullName.Contains("PrecompiledViews"))
                    {
                        modules.Add(new ModuleInfo { Assembly = assembly, Name = modularFolder.Name, Path = modularFolder.PhysicalPath });
                    }

                }

            }

            // Add mvc
            var mvcBuilder = services
                .AddMvc()
                .AddRazorOptions(o =>
                {
                    foreach (var module in modules)
                    {
                        o.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(module.Assembly.Location));
                    }
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            foreach (var module in modules)
            {
                // Register controller from modules
                mvcBuilder.AddApplicationPart(module.Assembly);

                // Register dependency in modules
                var moduleInitializerType = module.Assembly.GetTypes()
                    .FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                if (moduleInitializerType != null && moduleInitializerType != typeof(IModuleInitializer))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.ConfigureServices(services);
                }
            }

            GlobalConfiguration.Modules = modules;

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Serving static file for modules
            foreach (var module in GlobalConfiguration.Modules)
            {
                var wwwrootFolder = new DirectoryInfo(Path.Combine(module.Path, "wwwroot"));
                if (!wwwrootFolder.Exists)
                {
                    continue;
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(module.Path),
                    RequestPath = new PathString("/" + module.SortName)
                });

            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
