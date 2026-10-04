using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Models;

namespace ShippingQuote.Server.Data;

public class ShippingQuoteDbContext : DbContext
{
	public ShippingQuoteDbContext(DbContextOptions<ShippingQuoteDbContext> options): base(options)
	{
	}

	public DbSet<Carrier> Carriers => Set<Carrier>();
	public DbSet<DeliveryService> DeliveryServices => Set<DeliveryService>();
	public DbSet<PricingRule> PricingRules => Set<PricingRule>();
	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Carrier>()
			.HasMany(c => c.DeliveryServices)
			.WithOne(d => d.Carrier)
			.HasForeignKey(d => d.CarrierId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<DeliveryService>()
			.HasMany(d => d.PricingRules)
			.WithOne(p => p.DeliveryService)
			.HasForeignKey(p => p.DeliveryServiceId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<User>()
			.HasOne(u => u.Carrier)
			.WithMany(c => c.Users)
			.HasForeignKey(u => u.CarrierId)
			.OnDelete(DeleteBehavior.SetNull);

		modelBuilder.Entity<Carrier>()
			.HasIndex(c => c.Code)
			.IsUnique();

		modelBuilder.Entity<DeliveryService>()
			.Property(d => d.Type)
			.HasConversion<string>();

		modelBuilder.Entity<User>()
			.Property(u => u.Role)
			.HasConversion<string>();

		modelBuilder.Entity<DeliveryService>()
			.Property(d => d.MaxWeightKg)
			.HasPrecision(8, 2);

		modelBuilder.Entity<DeliveryService>()
			.Property(d => d.MaxLengthCm)
			.HasPrecision(8, 2);

		modelBuilder.Entity<DeliveryService>()
			.Property(d => d.MaxWidthCm)
			.HasPrecision(8, 2);

		modelBuilder.Entity<DeliveryService>()
			.Property(d => d.MaxHeightCm)
			.HasPrecision(8, 2);

		modelBuilder.Entity<PricingRule>()
			.Property(p => p.MinWeightKg)
			.HasPrecision(8, 2);

		modelBuilder.Entity<PricingRule>()
			.Property(p => p.MaxWeightKg)
			.HasPrecision(8, 2);

		modelBuilder.Entity<PricingRule>()
			.Property(p => p.Price)
			.HasPrecision(10, 2);

		modelBuilder.Entity<PricingRule>()
			.Property(p => p.FreeFromOrderValue)
			.HasPrecision(10, 2);
	}
}