using Microsoft.AspNetCore.Mvc;
using ShippingQuote.Server.DTOs.Common;
using ShippingQuote.Server.DTOs.PricingRules;
using ShippingQuote.Server.Services;

namespace ShippingQuote.Server.Controllers;

[ApiController]
[Route("api/pricing-rules")]
public class PricingRulesController : ControllerBase
{
	private readonly PricingRuleService _service;

	public PricingRulesController(PricingRuleService service)
	{
		_service = service;
	}

	/*[HttpGet]
	public async Task<ActionResult<List<PricingRuleDto>>> GetAll()
	{
		return Ok(await _service.GetAllAsync());
	}*/
	[HttpGet]
	public async Task<ActionResult<PagedResult<PricingRuleDto>>> GetAll(
	[FromQuery] int page = 1,
	[FromQuery] int pageSize = 10,
	[FromQuery] int? deliveryServiceId = null,
	[FromQuery] bool? isActive = null)
	{
		if (page < 1 || pageSize < 1 || pageSize > 100)
		{
			return BadRequest();
		}

		PagedResult<PricingRuleDto> result =
			await _service.GetAllAsync(
				page,
				pageSize,
				deliveryServiceId,
				isActive);

		foreach (PricingRuleDto rule in result.Items)
		{
			AddLinks(rule);
		}

		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<PricingRuleDto>> GetById(int id)
	{
		PricingRuleDto? rule = await _service.GetByIdAsync(id);

		if (rule == null)
		{
			return NotFound();
		}
		AddLinks(rule);
		return Ok(rule);
	}

	[HttpPost]
	public async Task<ActionResult<PricingRuleDto>> Create(
		CreatePricingRuleDto dto)
	{
		if (!await _service.DeliveryServiceExistsAsync(dto.DeliveryServiceId))
		{
			return BadRequest("Delivery service does not exist.");
		}

		if (!_service.IsWeightRangeValid(
			dto.MinWeightKg,
			dto.MaxWeightKg))
		{
			return BadRequest(
				"MinWeightKg must be less than MaxWeightKg.");
		}

		if (!_service.IsDateRangeValid(dto.ValidFrom, dto.ValidTo))
		{
			return BadRequest(
				"ValidTo cannot be earlier than ValidFrom.");
		}

		if (await _service.HasOverlapAsync(
			dto.DeliveryServiceId,
			dto.MinWeightKg,
			dto.MaxWeightKg))
		{
			return Conflict(
				"Pricing rule weight range overlaps an existing rule.");
		}

		PricingRuleDto rule = await _service.CreateAsync(dto);

		return CreatedAtAction(
			nameof(GetById),
			new { id = rule.Id },
			rule);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult<PricingRuleDto>> Update(
		int id,
		UpdatePricingRuleDto dto)
	{
		if (!await _service.DeliveryServiceExistsAsync(dto.DeliveryServiceId))
		{
			return BadRequest("Delivery service does not exist.");
		}

		if (!_service.IsWeightRangeValid(
			dto.MinWeightKg,
			dto.MaxWeightKg))
		{
			return BadRequest(
				"MinWeightKg must be less than MaxWeightKg.");
		}

		if (!_service.IsDateRangeValid(dto.ValidFrom, dto.ValidTo))
		{
			return BadRequest(
				"ValidTo cannot be earlier than ValidFrom.");
		}

		if (await _service.HasOverlapAsync(
			dto.DeliveryServiceId,
			dto.MinWeightKg,
			dto.MaxWeightKg,
			id))
		{
			return Conflict(
				"Pricing rule weight range overlaps an existing rule.");
		}

		PricingRuleDto? rule = await _service.UpdateAsync(id, dto);

		if (rule == null)
		{
			return NotFound();
		}

		return Ok(rule);
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		bool deleted = await _service.DeleteAsync(id);

		if (!deleted)
		{
			return NotFound();
		}

		return NoContent();
	}
	[HttpGet("~/api/carriers/{carrierId:int}/services/{serviceId:int}/pricing-rules")]
	public async Task<ActionResult<List<PricingRuleDto>>> GetByHierarchy(
	int carrierId, int serviceId)
	{
		List<PricingRuleDto>? rules =
			await _service.GetByHierarchyAsync(carrierId, serviceId);

		if (rules == null)
		{
			return NotFound();
		}

		return Ok(rules);
	}
	private void AddLinks(PricingRuleDto rule)
	{
		rule.Links =
		[
			new()
		{
			Rel = "self",
			Href = $"/api/pricing-rules/{rule.Id}",
			Method = "GET"
		},
		new()
		{
			Rel = "delivery-service",
			Href = $"/api/delivery-services/{rule.DeliveryServiceId}",
			Method = "GET"
		},
		new()
		{
			Rel = "update",
			Href = $"/api/pricing-rules/{rule.Id}",
			Method = "PUT"
		},
		new()
		{
			Rel = "delete",
			Href = $"/api/pricing-rules/{rule.Id}",
			Method = "DELETE"
		}
		];
	}
}