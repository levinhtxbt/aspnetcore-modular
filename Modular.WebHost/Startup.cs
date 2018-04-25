using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modular.Infrastructure;
using Modular.WebHost.Extensions;
using System;
using System.Linq;

namespace Modular.WebHost
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.LoadInstalledModules(_hostingEnvironment);
            services.AddCustomizedDataStore(_configuration);
            services.AddCustomizedMvc(GlobalConfiguration.Modules);
            services.AddModulesConfigureServices();
            return services.Build(_configuration, _hostingEnvironment);
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
            app.UseCustomizedStaticFiles(env);
            app.UseCustomizedMvc();
            app.UseModulesApplicationConfigure(env);
        }
    }
}