using System;
using System.Collections.Generic;
using System.Text;

namespace Modular.Module.ModuleA.ViewModels
{
    public class SampleViewModel
    {
        public SampleViewModel(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
