using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.DTOs.Common;
using ShippingQuote.Server.DTOs.PricingRules;
using ShippingQuote.Server.Models;

namespace ShippingQuote.Server.Services;

public class PricingRuleService
{
	private readonly ShippingQuoteDbContext _context;

	public PricingRuleService(ShippingQuoteDbContext context)
	{
		_context = context;
	}

	/*public async Task<List<PricingRuleDto>> GetAllAsync()
	{
		return await _context.PricingRules
			.OrderBy(p => p.DeliveryServiceId)
			.ThenBy(p => p.MinWeightKg)
			.Select(p => new PricingRuleDto
			{
				Id = p.Id,
				DeliveryServiceId = p.DeliveryServiceId,
				MinWeightKg = p.MinWeightKg,
				MaxWeightKg = p.MaxWeightKg,
				Price = p.Price,
				FreeFromOrderValue = p.FreeFromOrderValue,
				ValidFrom = p.ValidFrom,
				ValidTo = p.ValidTo,
				IsActive = p.IsActive
			})
			.ToListAsync();
	}*/
	public async Task<PagedResult<PricingRuleDto>> GetAllAsync(
	int page,
	int pageSize,
	int? deliveryServiceId,
	bool? isActive)
	{
		IQueryable<PricingRule> query =
			_context.PricingRules.AsNoTracking();

		if (deliveryServiceId.HasValue)
		{
			query = query.Where(p =>
				p.DeliveryServiceId == deliveryServiceId.Value);
		}

		if (isActive.HasValue)
		{
			query = query.Where(p =>
				p.IsActive == isActive.Value);
		}

		int totalCount = await query.CountAsync();

		List<PricingRuleDto> items = await query
			.OrderBy(p => p.DeliveryServiceId)
			.ThenBy(p => p.MinWeightKg)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.Select(p => new PricingRuleDto
			{
				Id = p.Id,
				DeliveryServiceId = p.DeliveryServiceId,
				MinWeightKg = p.MinWeightKg,
				MaxWeightKg = p.MaxWeightKg,
				Price = p.Price,
				FreeFromOrderValue = p.FreeFromOrderValue,
				ValidFrom = p.ValidFrom,
				ValidTo = p.ValidTo,
				IsActive = p.IsActive
			})
			.ToListAsync();

		return new PagedResult<PricingRuleDto>
		{
			Items = items,
			Page = page,
			PageSize = pageSize,
			TotalCount = totalCount,
			TotalPages = (int)Math.Ceiling(
				totalCount / (double)pageSize)
		};
	}

	public async Task<PricingRuleDto?> GetByIdAsync(int id)
	{
		return await _context.PricingRules
			.Where(p => p.Id == id)
			.Select(p => new PricingRuleDto
			{
				Id = p.Id,
				DeliveryServiceId = p.DeliveryServiceId,
				MinWeightKg = p.MinWeightKg,
				MaxWeightKg = p.MaxWeightKg,
				Price = p.Price,
				FreeFromOrderValue = p.FreeFromOrderValue,
				ValidFrom = p.ValidFrom,
				ValidTo = p.ValidTo,
				IsActive = p.IsActive
			})
			.FirstOrDefaultAsync();
	}

	public async Task<bool> DeliveryServiceExistsAsync(int deliveryServiceId)
	{
		return await _context.DeliveryServices
			.AnyAsync(d => d.Id == deliveryServiceId);
	}

	public bool IsWeightRangeValid(decimal minWeight, decimal maxWeight)
	{
		return minWeight < maxWeight;
	}

	public bool IsDateRangeValid(DateTimeOffset validFrom, DateTimeOffset? validTo)
	{
		return validTo == null || validFrom <= validTo;
	}

	public async Task<bool> HasOverlapAsync(
		int deliveryServiceId,
		decimal minWeight,
		decimal maxWeight,
		int? excludeId = null)
	{
		return await _context.PricingRules.AnyAsync(p =>
			p.DeliveryServiceId == deliveryServiceId &&
			(!excludeId.HasValue || p.Id != excludeId.Value) &&
			p.MinWeightKg < maxWeight &&
			minWeight < p.MaxWeightKg);
	}

	public async Task<PricingRuleDto> CreateAsync(CreatePricingRuleDto dto)
	{
		PricingRule rule = new PricingRule
		{
			DeliveryServiceId = dto.DeliveryServiceId,
			MinWeightKg = dto.MinWeightKg,
			MaxWeightKg = dto.MaxWeightKg,
			Price = dto.Price,
			FreeFromOrderValue = dto.FreeFromOrderValue,
			ValidFrom = dto.ValidFrom,
			ValidTo = dto.ValidTo
		};

		_context.PricingRules.Add(rule);
		await _context.SaveChangesAsync();

		return ToDto(rule);
	}

	public async Task<PricingRuleDto?> UpdateAsync(
		int id,
		UpdatePricingRuleDto dto)
	{
		PricingRule? rule = await _context.PricingRules
			.FirstOrDefaultAsync(p => p.Id == id);

		if (rule == null)
		{
			return null;
		}

		rule.DeliveryServiceId = dto.DeliveryServiceId;
		rule.MinWeightKg = dto.MinWeightKg;
		rule.MaxWeightKg = dto.MaxWeightKg;
		rule.Price = dto.Price;
		rule.FreeFromOrderValue = dto.FreeFromOrderValue;
		rule.ValidFrom = dto.ValidFrom;
		rule.ValidTo = dto.ValidTo;
		rule.IsActive = dto.IsActive;

		await _context.SaveChangesAsync();

		return ToDto(rule);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		PricingRule? rule = await _context.PricingRules
			.FirstOrDefaultAsync(p => p.Id == id);

		if (rule == null)
		{
			return false;
		}

		_context.PricingRules.Remove(rule);
		await _context.SaveChangesAsync();

		return true;
	}

	private static PricingRuleDto ToDto(PricingRule p)
	{
		return new PricingRuleDto
		{
			Id = p.Id,
			DeliveryServiceId = p.DeliveryServiceId,
			MinWeightKg = p.MinWeightKg,
			MaxWeightKg = p.MaxWeightKg,
			Price = p.Price,
			FreeFromOrderValue = p.FreeFromOrderValue,
			ValidFrom = p.ValidFrom,
			ValidTo = p.ValidTo,
			IsActive = p.IsActive
		};
	}
	public async Task<List<PricingRuleDto>?> GetByHierarchyAsync(int carrierId,int serviceId)
	{
		bool validScope = await _context.DeliveryServices
			.AnyAsync(d =>
				d.Id == serviceId &&
				d.CarrierId == carrierId);

		if (!validScope)
		{
			return null;
		}

		return await _context.PricingRules
			.Where(p => p.DeliveryServiceId == serviceId)
			.OrderBy(p => p.MinWeightKg)
			.Select(p => new PricingRuleDto
			{
				Id = p.Id,
				DeliveryServiceId = p.DeliveryServiceId,
				MinWeightKg = p.MinWeightKg,
				MaxWeightKg = p.MaxWeightKg,
				Price = p.Price,
				FreeFromOrderValue = p.FreeFromOrderValue,
				ValidFrom = p.ValidFrom,
				ValidTo = p.ValidTo,
				IsActive = p.IsActive
			})
			.ToListAsync();
	}

}