using Microsoft.EntityFrameworkCore;
using SignInApi.Entities;

namespace SignInApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UsersEntity>? users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsersEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UsersEntity>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }

}