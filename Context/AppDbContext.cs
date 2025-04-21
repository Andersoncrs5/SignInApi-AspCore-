using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignInApi.Entities;
using SignInApiEntities;

namespace SignInApi.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
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