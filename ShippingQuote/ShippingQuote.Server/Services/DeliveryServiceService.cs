using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.DTOs.Common;
using ShippingQuote.Server.DTOs.DeliveryServices;
using ShippingQuote.Server.DTOs.PricingRules;
using ShippingQuote.Server.Enums;
using ShippingQuote.Server.Models;

namespace ShippingQuote.Server.Services;

public class DeliveryServiceService
{
	private readonly ShippingQuoteDbContext _context;

	public DeliveryServiceService(ShippingQuoteDbContext context)
	{
		_context = context;
	}

	/*public async Task<List<DeliveryServiceDto>> GetAllAsync()
	{
		return await _context.DeliveryServices
			.OrderBy(d => d.Name)
			.Select(d => new DeliveryServiceDto
			{
				Id = d.Id,
				CarrierId = d.CarrierId,
				Name = d.Name,
				Type = d.Type,
				MaxWeightKg = d.MaxWeightKg,
				MaxLengthCm = d.MaxLengthCm,
				MaxWidthCm = d.MaxWidthCm,
				MaxHeightCm = d.MaxHeightCm,
				MinDeliveryDays = d.MinDeliveryDays,
				MaxDeliveryDays = d.MaxDeliveryDays,
				IsActive = d.IsActive
			})
			.ToListAsync();
	}*/
	public async Task<PagedResult<DeliveryServiceDto>> GetAllAsync(
	int page, int pageSize, int? carrierId, DeliveryServiceType? type,
	bool? isActive)
	{
		IQueryable<DeliveryService> query =
			_context.DeliveryServices.AsNoTracking();

		if (carrierId.HasValue)
		{
			query = query.Where(d =>
				d.CarrierId == carrierId.Value);
		}

		if (type.HasValue)
		{
			query = query.Where(d =>
				d.Type == type.Value);
		}

		if (isActive.HasValue)
		{
			query = query.Where(d =>
				d.IsActive == isActive.Value);
		}

		int totalCount = await query.CountAsync();

		List<DeliveryServiceDto> items = await query
			.OrderBy(d => d.Name)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.Select(d => new DeliveryServiceDto
			{
				Id = d.Id,
				CarrierId = d.CarrierId,
				Name = d.Name,
				Type = d.Type,
				MaxWeightKg = d.MaxWeightKg,
				MaxLengthCm = d.MaxLengthCm,
				MaxWidthCm = d.MaxWidthCm,
				MaxHeightCm = d.MaxHeightCm,
				MinDeliveryDays = d.MinDeliveryDays,
				MaxDeliveryDays = d.MaxDeliveryDays,
				IsActive = d.IsActive
			})
			.ToListAsync();

		return new PagedResult<DeliveryServiceDto>
		{
			Items = items,
			Page = page,
			PageSize = pageSize,
			TotalCount = totalCount,
			TotalPages = (int)Math.Ceiling(
				totalCount / (double)pageSize)
		};
	}

	public async Task<DeliveryServiceDto?> GetByIdAsync(int id)
	{
		return await _context.DeliveryServices
			.Where(d => d.Id == id)
			.Select(d => new DeliveryServiceDto
			{
				Id = d.Id,
				CarrierId = d.CarrierId,
				Name = d.Name,
				Type = d.Type,
				MaxWeightKg = d.MaxWeightKg,
				MaxLengthCm = d.MaxLengthCm,
				MaxWidthCm = d.MaxWidthCm,
				MaxHeightCm = d.MaxHeightCm,
				MinDeliveryDays = d.MinDeliveryDays,
				MaxDeliveryDays = d.MaxDeliveryDays,
				IsActive = d.IsActive
			})
			.FirstOrDefaultAsync();
	}

	public async Task<bool> CarrierExistsAsync(int carrierId)
	{
		return await _context.Carriers.AnyAsync(c => c.Id == carrierId);
	}

	public bool IsDeliveryWindowValid(int minDays, int maxDays)
	{
		return minDays <= maxDays;
	}

	public async Task<DeliveryServiceDto> CreateAsync(CreateDeliveryServiceDto dto)
	{
		DeliveryService deliveryService = new DeliveryService
		{
			CarrierId = dto.CarrierId,
			Name = dto.Name,
			Type = dto.Type,
			MaxWeightKg = dto.MaxWeightKg,
			MaxLengthCm = dto.MaxLengthCm,
			MaxWidthCm = dto.MaxWidthCm,
			MaxHeightCm = dto.MaxHeightCm,
			MinDeliveryDays = dto.MinDeliveryDays,
			MaxDeliveryDays = dto.MaxDeliveryDays
		};

		_context.DeliveryServices.Add(deliveryService);
		await _context.SaveChangesAsync();

		return ToDto(deliveryService);
	}

	public async Task<DeliveryServiceDto?> UpdateAsync(int id, 
	UpdateDeliveryServiceDto dto)
	{
		DeliveryService? deliveryService =
			await _context.DeliveryServices
				.FirstOrDefaultAsync(d => d.Id == id);

		if (deliveryService == null)
		{
			return null;
		}

		deliveryService.CarrierId = dto.CarrierId;
		deliveryService.Name = dto.Name;
		deliveryService.Type = dto.Type;
		deliveryService.MaxWeightKg = dto.MaxWeightKg;
		deliveryService.MaxLengthCm = dto.MaxLengthCm;
		deliveryService.MaxWidthCm = dto.MaxWidthCm;
		deliveryService.MaxHeightCm = dto.MaxHeightCm;
		deliveryService.MinDeliveryDays = dto.MinDeliveryDays;
		deliveryService.MaxDeliveryDays = dto.MaxDeliveryDays;
		deliveryService.IsActive = dto.IsActive;

		await _context.SaveChangesAsync();

		return ToDto(deliveryService);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		DeliveryService? deliveryService =
			await _context.DeliveryServices
				.FirstOrDefaultAsync(d => d.Id == id);

		if (deliveryService == null)
		{
			return false;
		}

		_context.DeliveryServices.Remove(deliveryService);
		await _context.SaveChangesAsync();

		return true;
	}

	private static DeliveryServiceDto ToDto(DeliveryService d)
	{
		return new DeliveryServiceDto
		{
			Id = d.Id,
			CarrierId = d.CarrierId,
			Name = d.Name,
			Type = d.Type,
			MaxWeightKg = d.MaxWeightKg,
			MaxLengthCm = d.MaxLengthCm,
			MaxWidthCm = d.MaxWidthCm,
			MaxHeightCm = d.MaxHeightCm,
			MinDeliveryDays = d.MinDeliveryDays,
			MaxDeliveryDays = d.MaxDeliveryDays,
			IsActive = d.IsActive
		};
	}

}