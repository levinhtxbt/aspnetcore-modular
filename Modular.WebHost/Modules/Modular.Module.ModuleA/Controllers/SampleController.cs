using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Modular.Module.ModuleA.Services;
using Modular.Module.ModuleA.ViewModels;

namespace Modular.Module.ModuleA.Controllers
{
    public class SampleController: Controller
    {
        private readonly ISampleService _sampleService;

        public SampleController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<IActionResult> Index()
        {
            var sample = await _sampleService.GetSamplesAsync();
            var sampleVms = sample.Select(x => new SampleViewModel(x.Id, x.Name, x.Description)).ToList();
            return View(sampleVms);
        }
    }
}
