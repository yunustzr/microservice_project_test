using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Domain.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuthenticationApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Soft Delete Filter
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

            // Indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("IX_Users_NormalizedEmail");

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
        }
    }
}
