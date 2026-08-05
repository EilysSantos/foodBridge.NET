using foodBridgeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace foodBridgeAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Donacion> Donaciones => Set<Donacion>();
    public DbSet<Solicitud> Solicitudes => Set<Solicitud>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Donacion>()
            .HasOne(d => d.Donante)
            .WithMany(u => u.Donaciones)
            .HasForeignKey(d => d.DonanteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Solicitud>()
            .HasOne(s => s.Donacion)
            .WithOne(d => d.Solicitud)
            .HasForeignKey<Solicitud>(s => s.DonacionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Solicitud>()
            .HasIndex(s => s.DonacionId)
            .IsUnique();

        modelBuilder.Entity<Solicitud>()
            .HasOne(s => s.Fundacion)
            .WithMany(u => u.Solicitudes)
            .HasForeignKey(s => s.FundacionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
