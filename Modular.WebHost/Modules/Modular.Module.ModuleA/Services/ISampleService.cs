using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Modular.Module.ModuleA.Models;

namespace Modular.Module.ModuleA.Services
{
    public interface ISampleService
    {
        Task<IEnumerable<Sample>> GetSamplesAsync();
    }
}
