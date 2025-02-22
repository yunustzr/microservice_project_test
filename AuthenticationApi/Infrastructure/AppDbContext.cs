using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Domain.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuthenticationApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
