using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modular.Infrastructure;
using Modular.Module.Core.Models;

namespace Modular.Module.Core.Infrastructure
{
    public class CoreCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Core_User");

            modelBuilder.Entity<Role>()
                .ToTable("Core_Role");

            modelBuilder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("Core_UserClaim");
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("Core_RoleClaim");
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("Core_UserRole");
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.ToTable("Core_UserLogin");
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.ToTable("Core_UserToken");
            });
        }
    }
}
