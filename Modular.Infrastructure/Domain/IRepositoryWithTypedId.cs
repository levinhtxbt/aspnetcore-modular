using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modular.Infrastructure.Domain.Models;

namespace Modular.Infrastructure.Domain
{
    public interface IRepositoryWithTypedId<T, in TId> where T : IEntityWithTypedId<TId>
    {
        IQueryable<T> Query();

        void Add(T entity);

        void SaveChange();

        void Remove(T entity);
    }
}
