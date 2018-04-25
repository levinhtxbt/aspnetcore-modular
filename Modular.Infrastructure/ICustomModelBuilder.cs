using Microsoft.EntityFrameworkCore;

namespace Modular.Infrastructure
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}