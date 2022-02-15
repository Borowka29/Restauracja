using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restauracja.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restauracja.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Dish>()
                .Property(o => o.discount).HasColumnType("decimal(5,2)");
            modelBuilder.Entity<Dish>()
                .Property(o => o.Price).HasColumnType("decimal(5,2)");
        }
        
        public DbSet<Opinion> Opinions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
    }
}
