using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Modular.Infrastructure.Domain;
using Modular.Module.ModuleA.Infrastructure;
using Modular.Module.ModuleA.Models;

namespace Modular.Module.ModuleA.Services
{
    public class SampleService : ISampleService
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IRepository<Test> _testRepository;

        public SampleService(ISampleRepository sampleRepository, IRepository<Test> testRepository)
        {
            _sampleRepository = sampleRepository;
            _testRepository = testRepository;
        }


        public async Task<IEnumerable<Sample>> GetSamplesAsync()
        {
            _testRepository.Add(new Test()
            {
                TestName = "Hello world"
            });
            _testRepository.SaveChange();

            return await _sampleRepository.Query().ToListAsync();
        }
    }
}
