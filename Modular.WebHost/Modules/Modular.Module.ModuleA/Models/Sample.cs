using System;
using System.Collections.Generic;
using System.Text;
using Modular.Infrastructure.Domain.Models;

namespace Modular.Module.ModuleA.Models
{
    public class Sample : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
