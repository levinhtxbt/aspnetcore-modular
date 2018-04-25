using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Modular.Module.ModuleA.Infrastructure;
using Modular.Module.ModuleA.Models;

namespace Modular.Module.ModuleA.Services
{
    public class SampleService : ISampleService
    {
        private readonly ISampleRepository _sampleRepository;

        public SampleService(ISampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }


        public async Task<IEnumerable<Sample>> GetSamplesAsync()
        {
            return await _sampleRepository.Query().ToListAsync();
        }
    }
}
