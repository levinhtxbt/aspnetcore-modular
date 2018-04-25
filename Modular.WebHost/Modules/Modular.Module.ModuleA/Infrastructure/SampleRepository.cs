using System;
using System.Collections.Generic;
using System.Text;
using Modular.Infrastructure.Domain;
using Modular.Module.Core.Infrastructure;
using Modular.Module.ModuleA.Models;

namespace Modular.Module.ModuleA.Infrastructure
{
    public interface ISampleRepository : IRepository<Sample>
    {
    }



    public class SampleRepository : Repository<Sample>, ISampleRepository
    {
        public SampleRepository(ModularDbContext context) : base(context)
        {
        }
    }
}
