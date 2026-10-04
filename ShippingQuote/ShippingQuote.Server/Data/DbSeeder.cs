using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Enums;
using ShippingQuote.Server.Models;

namespace ShippingQuote.Server.Data;

public static class DbSeeder
{
	public static async Task SeedAsync(ShippingQuoteDbContext context)
	{
		if (await context.Carriers.AnyAsync())
		{
			return;
		}

		var carrier1 = new Carrier
		{
			Name = "Baltic Parcel",
			Code = "BP",
			Description = "Demo parcel delivery provider",
			IsActive = true
		};

		var carrier2 = new Carrier
		{
			Name = "Fast Ship",
			Code = "FS",
			Description = "Demo courier provider",
			IsActive = true
		};

		var carrier3 = new Carrier
		{
			Name = "Quick Box",
			Code = "QB",
			IsActive = true
		};

		var carrier4 = new Carrier
		{
			Name = "North Courier",
			Code = "NC",
			IsActive = true
		};

		var carrier5 = new Carrier
		{
			Name = "Parcel Go",
			Code = "PG",
			IsActive = true
		};

		context.Carriers.AddRange(
			carrier1,
			carrier2,
			carrier3,
			carrier4,
			carrier5
		);

		await context.SaveChangesAsync();

		var lockerService = new DeliveryService
		{
			CarrierId = carrier1.Id,
			Name = "Parcel Locker",
			Type = DeliveryServiceType.ParcelLocker,
			MaxWeightKg = 20,
			MaxLengthCm = 60,
			MaxWidthCm = 40,
			MaxHeightCm = 40,
			MinDeliveryDays = 1,
			MaxDeliveryDays = 2,
			IsActive = true
		};

		var courierService = new DeliveryService
		{
			CarrierId = carrier2.Id,
			Name = "Courier Delivery",
			Type = DeliveryServiceType.Courier,
			MaxWeightKg = 30,
			MaxLengthCm = 100,
			MaxWidthCm = 60,
			MaxHeightCm = 60,
			MinDeliveryDays = 1,
			MaxDeliveryDays = 2,
			IsActive = true
		};

		var pickupService = new DeliveryService
		{
			CarrierId = carrier3.Id,
			Name = "Pickup Point",
			Type = DeliveryServiceType.PickupPoint,
			MaxWeightKg = 15,
			MaxLengthCm = 50,
			MaxWidthCm = 40,
			MaxHeightCm = 40,
			MinDeliveryDays = 2,
			MaxDeliveryDays = 3,
			IsActive = true
		};

		var secondCourierService = new DeliveryService
		{
			CarrierId = carrier4.Id,
			Name = "Standard Courier",
			Type = DeliveryServiceType.Courier,
			MaxWeightKg = 25,
			MaxLengthCm = 80,
			MaxWidthCm = 50,
			MaxHeightCm = 50,
			MinDeliveryDays = 1,
			MaxDeliveryDays = 3,
			IsActive = true
		};

		var secondLockerService = new DeliveryService
		{
			CarrierId = carrier5.Id,
			Name = "Smart Locker",
			Type = DeliveryServiceType.ParcelLocker,
			MaxWeightKg = 10,
			MaxLengthCm = 45,
			MaxWidthCm = 35,
			MaxHeightCm = 35,
			MinDeliveryDays = 2,
			MaxDeliveryDays = 3,
			IsActive = true
		};

		context.DeliveryServices.AddRange(
			lockerService,
			courierService,
			pickupService,
			secondCourierService,
			secondLockerService
		);

		await context.SaveChangesAsync();

		context.PricingRules.AddRange(
			new PricingRule
			{
				DeliveryServiceId = lockerService.Id,
				MinWeightKg = 0,
				MaxWeightKg = 2,
				Price = 2.99m,
				ValidFrom = DateTimeOffset.UtcNow,
				IsActive = true
			},
			new PricingRule
			{
				DeliveryServiceId = lockerService.Id,
				MinWeightKg = 2,
				MaxWeightKg = 5,
				Price = 3.49m,
				ValidFrom = DateTimeOffset.UtcNow,
				IsActive = true
			},
			new PricingRule
			{
				DeliveryServiceId = courierService.Id,
				MinWeightKg = 0,
				MaxWeightKg = 5,
				Price = 5.99m,
				ValidFrom = DateTimeOffset.UtcNow,
				IsActive = true
			},
			new PricingRule
			{
				DeliveryServiceId = pickupService.Id,
				MinWeightKg = 0,
				MaxWeightKg = 5,
				Price = 3.20m,
				ValidFrom = DateTimeOffset.UtcNow,
				IsActive = true
			},
			new PricingRule
			{
				DeliveryServiceId = secondLockerService.Id,
				MinWeightKg = 0,
				MaxWeightKg = 5,
				Price = 3.79m,
				ValidFrom = DateTimeOffset.UtcNow,
				IsActive = true
			}
		);

		await context.SaveChangesAsync();
	}
}