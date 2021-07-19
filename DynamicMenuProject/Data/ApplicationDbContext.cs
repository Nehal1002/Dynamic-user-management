using System;
using System.Collections.Generic;
using System.Text;
using DynamicMenuProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicMenuProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MenuItems> MenuItems { get; set; }
        public DbSet<MenuPermissions> MenuPermissions { get; set; }
    }
}
