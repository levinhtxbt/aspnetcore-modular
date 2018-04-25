using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Modular.Core;
using Modular.Infrastructure;
using Modular.Module.ModuleA.Infrastructure;
using Modular.Module.ModuleA.Services;

namespace Modular.Module.ModuleA
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISampleRepository, SampleRepository>();
            serviceCollection.AddScoped<ISampleService, SampleService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
        }
    }
}
