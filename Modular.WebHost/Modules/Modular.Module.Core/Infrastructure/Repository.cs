using System;
using System.Collections.Generic;
using System.Text;
using Modular.Infrastructure.Domain;
using Modular.Infrastructure.Domain.Models;

namespace Modular.Module.Core.Infrastructure
{
    public class Repository<T> : RepositoryWithTypedId<T, int>, IRepository<T>
        where T : class, IEntityWithTypedId<int>
    {
        public Repository(ModularDbContext context) : base(context)
        {
        }
    }
}
