using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modular.Core;
using Modular.Infrastructure;
using Modular.Module.Core.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.Variance;
using Modular.Infrastructure.Domain;

namespace Modular.WebHost.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection LoadInstalledModules(this IServiceCollection services, IHostingEnvironment hostingEnvironment)
        {
            List<ModuleInfo> modules = new List<ModuleInfo>();

            // Load modules
            var moduleRootFolder = hostingEnvironment.ContentRootFileProvider.GetDirectoryContents("Modules");
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

            
            foreach (var module in modules)
            {
                var moduleInitializerType = module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    services.AddSingleton(typeof(IModuleInitializer), moduleInitializerType);
                }
            }

            GlobalConfiguration.Modules = modules;

            return services;
        }

        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());
            });

            var mvcBuilder = services
                .AddMvc()
                .AddRazorOptions(o =>
                {
                    foreach (var module in modules)
                    {
                        o.AdditionalCompilationReferences
                            .Add(MetadataReference.CreateFromFile(module.Assembly.Location));
                    }
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            foreach (var module in GlobalConfiguration.Modules)
            {
                mvcBuilder.AddApplicationPart(module.Assembly);
            }


            return services;
        }

        public static IServiceCollection AddCustomizedDataStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ModularDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Modular.WebHost")));

            return services;
        }

        public static IServiceCollection AddModulesConfigureServices(this IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var moduleInitializers = sp.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.ConfigureServices(services);
            }

            return services;
        }

        public static IServiceProvider Build(this IServiceCollection services, IConfiguration configuration,
            IHostingEnvironment env)
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(RepositoryWithTypedId<,>)).As(typeof(IRepositoryWithTypedId<,>));
            builder.RegisterSource(new ContravariantRegistrationSource());

            foreach (var module in GlobalConfiguration.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(module.Assembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
            }

            builder.RegisterInstance(configuration);
            builder.RegisterInstance(env);
            builder.Populate(services);
            var container = builder.Build();

            return container.Resolve<IServiceProvider>();
            
        }
    }
}