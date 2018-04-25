using System;
using System.Collections.Generic;
using System.Text;

namespace Modular.Infrastructure.Domain.Models
{
    public interface IEntityWithTypedId<TId>
    {
        TId Id { get; }
    }
}
