using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Enums;
using ShippingQuote.Server.Models;

namespace ShippingQuote.Server.Data;

public static class DbSeeder
{
	public static async Task SeedAsync(
		ShippingQuoteDbContext context,
		IPasswordHasher<User> passwordHasher)
	{
		// -------------------------
		// CARRIERS
		// -------------------------

		Carrier balticParcel = await GetOrCreateCarrierAsync(
			context,
			"Baltic Parcel",
			"BP",
			"Demo parcel delivery provider");

		Carrier fastShip = await GetOrCreateCarrierAsync(
			context,
			"Fast Ship",
			"FS",
			"Demo courier provider");

		Carrier quickBox = await GetOrCreateCarrierAsync(
			context,
			"Quick Box",
			"QB",
			"Demo pickup point provider");

		Carrier northCourier = await GetOrCreateCarrierAsync(
			context,
			"North Courier",
			"NC",
			"Demo courier provider");

		Carrier parcelGo = await GetOrCreateCarrierAsync(
			context,
			"Parcel Go",
			"PG",
			"Demo parcel locker provider");

		Carrier dpd = await GetOrCreateCarrierAsync(
			context,
			"DPD Lietuva",
			"DPD",
			"Parcel and courier delivery provider");

		Carrier omniva = await GetOrCreateCarrierAsync(
			context,
			"Omniva",
			"OMNIVA",
			"Parcel locker and courier delivery provider");


		// -------------------------
		// DELIVERY SERVICES
		// -------------------------

		DeliveryService lockerService =
			await GetOrCreateDeliveryServiceAsync(
				context,
				balticParcel.Id,
				"Parcel Locker",
				DeliveryServiceType.ParcelLocker,
				20,
				60,
				40,
				40,
				1,
				2);

		DeliveryService courierService =
			await GetOrCreateDeliveryServiceAsync(
				context,
				fastShip.Id,
				"Courier Delivery",
				DeliveryServiceType.Courier,
				30,
				100,
				60,
				60,
				1,
				2);

		DeliveryService pickupService =
			await GetOrCreateDeliveryServiceAsync(
				context,
				quickBox.Id,
				"Pickup Point",
				DeliveryServiceType.PickupPoint,
				15,
				50,
				40,
				40,
				2,
				3);

		DeliveryService northCourierService =
			await GetOrCreateDeliveryServiceAsync(
				context,
				northCourier.Id,
				"Standard Courier",
				DeliveryServiceType.Courier,
				25,
				80,
				50,
				50,
				1,
				3);

		DeliveryService parcelGoLocker =
			await GetOrCreateDeliveryServiceAsync(
				context,
				parcelGo.Id,
				"Smart Locker",
				DeliveryServiceType.ParcelLocker,
				10,
				45,
				35,
				35,
				2,
				3);

		DeliveryService dpdCourier =
			await GetOrCreateDeliveryServiceAsync(
				context,
				dpd.Id,
				"DPD Courier",
				DeliveryServiceType.Courier,
				31.5m,
				100,
				60,
				60,
				1,
				2);

		DeliveryService omnivaLocker =
			await GetOrCreateDeliveryServiceAsync(
				context,
				omniva.Id,
				"Omniva Parcel Locker",
				DeliveryServiceType.ParcelLocker,
				30,
				64,
				38,
				39,
				1,
				3);


		// -------------------------
		// PRICING RULES
		// -------------------------

		await GetOrCreatePricingRuleAsync(
			context,
			lockerService.Id,
			0,
			2,
			2.99m);

		await GetOrCreatePricingRuleAsync(
			context,
			lockerService.Id,
			2,
			5,
			3.49m);

		await GetOrCreatePricingRuleAsync(
			context,
			courierService.Id,
			0,
			5,
			5.99m);

		await GetOrCreatePricingRuleAsync(
			context,
			pickupService.Id,
			0,
			5,
			3.20m);

		await GetOrCreatePricingRuleAsync(
			context,
			parcelGoLocker.Id,
			0,
			5,
			3.79m);

		await GetOrCreatePricingRuleAsync(
			context,
			dpdCourier.Id,
			0,
			5,
			5.49m);

		await GetOrCreatePricingRuleAsync(
			context,
			omnivaLocker.Id,
			0,
			5,
			3.99m);


		// -------------------------
		// USERS
		// -------------------------

		await GetOrCreateUserAsync(
			context,
			passwordHasher,
			"admin@test.lt",
			"Admin123!",
			UserRole.Administrator,
			null);

		await GetOrCreateUserAsync(
			context,
			passwordHasher,
			"dpd.manager@test.lt",
			"Manager123!",
			UserRole.CarrierManager,
			dpd.Id);

		await GetOrCreateUserAsync(
			context,
			passwordHasher,
			"omniva.manager@test.lt",
			"Manager123!",
			UserRole.CarrierManager,
			omniva.Id);

		await context.SaveChangesAsync();
	}


	// -------------------------
	// HELPERS
	// -------------------------

	private static async Task<Carrier> GetOrCreateCarrierAsync(
		ShippingQuoteDbContext context,
		string name,
		string code,
		string description)
	{
		Carrier? existing = await context.Carriers
			.FirstOrDefaultAsync(c => c.Code == code);

		if (existing != null)
		{
			return existing;
		}

		Carrier carrier = new Carrier
		{
			Name = name,
			Code = code,
			Description = description,
			IsActive = true
		};

		context.Carriers.Add(carrier);

		await context.SaveChangesAsync();

		return carrier;
	}


	private static async Task<DeliveryService> GetOrCreateDeliveryServiceAsync(
		ShippingQuoteDbContext context,
		int carrierId,
		string name,
		DeliveryServiceType type,
		decimal maxWeightKg,
		decimal maxLengthCm,
		decimal maxWidthCm,
		decimal maxHeightCm,
		int minDeliveryDays,
		int maxDeliveryDays)
	{
		DeliveryService? existing =
			await context.DeliveryServices
				.FirstOrDefaultAsync(d =>
					d.CarrierId == carrierId &&
					d.Name == name);

		if (existing != null)
		{
			return existing;
		}

		DeliveryService service = new DeliveryService
		{
			CarrierId = carrierId,
			Name = name,
			Type = type,
			MaxWeightKg = maxWeightKg,
			MaxLengthCm = maxLengthCm,
			MaxWidthCm = maxWidthCm,
			MaxHeightCm = maxHeightCm,
			MinDeliveryDays = minDeliveryDays,
			MaxDeliveryDays = maxDeliveryDays,
			IsActive = true
		};

		context.DeliveryServices.Add(service);

		await context.SaveChangesAsync();

		return service;
	}


	private static async Task GetOrCreatePricingRuleAsync(
		ShippingQuoteDbContext context,
		int deliveryServiceId,
		decimal minWeightKg,
		decimal maxWeightKg,
		decimal price)
	{
		bool exists = await context.PricingRules
			.AnyAsync(p =>
				p.DeliveryServiceId == deliveryServiceId &&
				p.MinWeightKg == minWeightKg &&
				p.MaxWeightKg == maxWeightKg);

		if (exists)
		{
			return;
		}

		PricingRule rule = new PricingRule
		{
			DeliveryServiceId = deliveryServiceId,
			MinWeightKg = minWeightKg,
			MaxWeightKg = maxWeightKg,
			Price = price,
			ValidFrom = DateTimeOffset.UtcNow,
			IsActive = true
		};

		context.PricingRules.Add(rule);

		await context.SaveChangesAsync();
	}


	private static async Task GetOrCreateUserAsync(
		ShippingQuoteDbContext context,
		IPasswordHasher<User> passwordHasher,
		string email,
		string password,
		UserRole role,
		int? carrierId)
	{
		bool exists = await context.Users
			.AnyAsync(u => u.Email == email);

		if (exists)
		{
			return;
		}

		User user = new User
		{
			Email = email,
			Role = role,
			CarrierId = carrierId
		};

		user.PasswordHash =
			passwordHasher.HashPassword(user, password);

		context.Users.Add(user);

		await context.SaveChangesAsync();
	}
}