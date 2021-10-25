using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Models;
using System;

namespace MyTestWebApp.Context
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<Ad> Ads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
               new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = "user", ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "USER" },
               new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = "admin", ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "ADMIN" }
               );
        }
    }
}
