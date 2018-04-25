using System;
using System.Collections.Generic;
using System.Text;

namespace Modular.Infrastructure.Domain.Models
{
    public class EntityWithTypedId<TId> : IEntityWithTypedId<TId>
    {
        public TId Id { get; protected set; }
    }
}
