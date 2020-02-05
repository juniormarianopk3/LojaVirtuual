using System;
using System.Collections.Generic;
using System.Text;
using LojaVirtual.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet <Department> Departments { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }
        public DbSet<Brand> Brands { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(p =>
           {
               p.HasKey(e => e.Id);

               p.Property(e => e.Name).IsRequired().HasMaxLength(100);
               p.Property(e => e.Price).IsRequired();
               p.Property(e => e.Description).IsRequired().HasMaxLength(800);

           });

            builder.Entity<ImageProduct>(p =>
           {
               p.HasKey(e => e.Id);
               p.Property(e => e.Image).IsRequired();
               p.Property(e => e.ProductId).IsRequired();

           });

            builder.Entity<Department>(p =>
            {
                p.HasKey(e => e.Id);
                p.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

        }
    }
}
