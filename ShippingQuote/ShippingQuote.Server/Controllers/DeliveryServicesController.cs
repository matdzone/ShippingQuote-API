using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingQuote.Server.DTOs.Common;
using ShippingQuote.Server.DTOs.DeliveryServices;
using ShippingQuote.Server.DTOs.PricingRules;
using ShippingQuote.Server.Enums;
using ShippingQuote.Server.Services;
namespace ShippingQuote.Server.Controllers;

[ApiController]
[Route("api/delivery-services")]
[Authorize]
public class DeliveryServicesController : ControllerBase
{
	private readonly DeliveryServiceService _service;
	private readonly ResourceAccessService _access;

	public DeliveryServicesController(DeliveryServiceService service, ResourceAccessService access)
	{
		_service = service;
		_access = access;
	}

	/*[HttpGet]
	public async Task<ActionResult<List<DeliveryServiceDto>>> GetAll()
	{
		List<DeliveryServiceDto> services =
			await _service.GetAllAsync();

		return Ok(services);
	}*/
	[HttpGet]
	public async Task<ActionResult<PagedResult<DeliveryServiceDto>>> GetAll(
	[FromQuery] int page = 1,
	[FromQuery] int pageSize = 10,
	[FromQuery] int? carrierId = null,
	[FromQuery] DeliveryServiceType? type = null,
	[FromQuery] bool? isActive = null)
	{
		if (page < 1 || pageSize < 1 || pageSize > 100)
		{
			return BadRequest();
		}

		PagedResult<DeliveryServiceDto> result =
			await _service.GetAllAsync(
				page,
				pageSize,
				carrierId,
				type,
				isActive);

		foreach (DeliveryServiceDto service in result.Items)
		{
			AddLinks(service);
		}

		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<DeliveryServiceDto>> GetById(int id)
	{
		DeliveryServiceDto? service =
			await _service.GetByIdAsync(id);

		if (service == null)
		{
			return NotFound();
		}
		AddLinks(service);
		return Ok(service);
	}
	[Authorize(Roles = "Administrator,CarrierManager")]
	[HttpPost]
	public async Task<ActionResult<DeliveryServiceDto>> Create(
		CreateDeliveryServiceDto dto)
	{
		if (!await _service.CarrierExistsAsync(dto.CarrierId))
		{
			return BadRequest("Carrier does not exist.");
		}
		if (!await _access.CanManageCarrierAsync(User, dto.CarrierId))
		{
			return Forbid();
		}
		if (!_service.IsDeliveryWindowValid(
			dto.MinDeliveryDays,
			dto.MaxDeliveryDays))
		{
			return BadRequest(
				"MinDeliveryDays cannot be greater than MaxDeliveryDays.");
		}

		DeliveryServiceDto service =
			await _service.CreateAsync(dto);

		return CreatedAtAction(
			nameof(GetById),
			new { id = service.Id },
			service);
	}
	[Authorize(Roles = "Administrator,CarrierManager")]
	[HttpPut("{id}")]
	public async Task<ActionResult<DeliveryServiceDto>> Update(
		int id,
		UpdateDeliveryServiceDto dto)
	{
		DeliveryServiceDto? existing =
		await _service.GetByIdAsync(id);

		if (existing == null)
		{
			return NotFound();
		}
		if (!await _access.CanManageDeliveryServiceAsync(User, id))
		{
			return Forbid();
		}
		if (!await _access.CanManageCarrierAsync(User, dto.CarrierId))
		{
			return Forbid();
		}
		if (!await _service.CarrierExistsAsync(dto.CarrierId))
		{
			return BadRequest("Carrier does not exist.");
		}

		if (!_service.IsDeliveryWindowValid(
			dto.MinDeliveryDays,
			dto.MaxDeliveryDays))
		{
			return BadRequest(
				"MinDeliveryDays cannot be greater than MaxDeliveryDays.");
		}

		DeliveryServiceDto? service =
			await _service.UpdateAsync(id, dto);

		if (service == null)
		{
			return NotFound();
		}

		return Ok(service);
	}
	[Authorize(Roles = "Administrator,CarrierManager")]
	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		DeliveryServiceDto? existing =
		await _service.GetByIdAsync(id);

		if (existing == null)
		{
			return NotFound();
		}
		if (!await _access.CanManageDeliveryServiceAsync(User, id))
		{
			return Forbid();
		}
		bool deleted = await _service.DeleteAsync(id);

		if (!deleted)
		{
			return NotFound();
		}

		return NoContent();
	}
	private void AddLinks(DeliveryServiceDto service)
	{
		service.Links =
		[
			new()
		{
			Rel = "self",
			Href = $"/api/delivery-services/{service.Id}",
			Method = "GET"
		},
		new()
		{
			Rel = "carrier",
			Href = $"/api/carriers/{service.CarrierId}",
			Method = "GET"
		},
		new()
		{
			Rel = "pricing-rules",
			Href = $"/api/carriers/{service.CarrierId}/services/{service.Id}/pricing-rules",
			Method = "GET"
		},
		new()
		{
			Rel = "update",
			Href = $"/api/delivery-services/{service.Id}",
			Method = "PUT"
		},
		new()
		{
			Rel = "delete",
			Href = $"/api/delivery-services/{service.Id}",
			Method = "DELETE"
		}
		];
	}
}