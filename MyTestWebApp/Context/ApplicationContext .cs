using Microsoft.EntityFrameworkCore;
using MyTestWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTestWebApp.Context
{
    public class ApplicationContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Ad> Ads { get; set; }

        public ApplicationContext(DbContextOptions options)
        : base(options)
        {

        }
    }
}
