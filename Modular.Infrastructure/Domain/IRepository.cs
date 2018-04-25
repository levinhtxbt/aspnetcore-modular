using System;
using System.Collections.Generic;
using System.Text;
using Modular.Infrastructure.Domain.Models;

namespace Modular.Infrastructure.Domain
{
    public interface IRepository<T> : IRepositoryWithTypedId<T, int> where T : IEntityWithTypedId<int>
    {
    }
}
