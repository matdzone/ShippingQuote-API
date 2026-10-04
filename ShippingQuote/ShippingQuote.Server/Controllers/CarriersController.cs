using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShippingQuote.Server.DTOs.Carriers;
using ShippingQuote.Server.DTOs.Common;
using ShippingQuote.Server.Services;

namespace ShippingQuote.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CarriersController : ControllerBase
	{
		private readonly CarrierService _service;
		public CarriersController(CarrierService service)
		{
			_service = service;
		}
		/*[HttpGet]
		public async Task <ActionResult<List<CarrierDto>>> GetAll(){
			List<CarrierDto> carriers = await _service.GetAllAsync();
			return Ok(carriers);
		}*/
		[HttpGet]
		public async Task<ActionResult<PagedResult<CarrierDto>>> GetAll(
	[FromQuery] int page = 1,
	[FromQuery] int pageSize = 10,
	[FromQuery] string? name = null,
	[FromQuery] bool? isActive = null)
		{
			if (page < 1 || pageSize < 1 || pageSize > 100)
			{
				return BadRequest();
			}

			PagedResult<CarrierDto> result =
				await _service.GetAllAsync(
					page,
					pageSize,
					name,
					isActive);

			foreach (CarrierDto carrier in result.Items)
			{
				AddLinks(carrier);
			}

			return Ok(result);
		}
		[HttpGet("{id}")]
		public async Task <ActionResult<CarrierDto>> GetById(int id){
			CarrierDto? carrier = await _service.GetByIdAsync(id);
			if(carrier == null)
			{
				return NotFound();
			}
			AddLinks(carrier);
			return Ok(carrier);
		}
		[HttpPost]
		public async Task <ActionResult<CarrierDto>> Create(CreateCarrierDto dto)
		{
			if (await _service.CodeExistsAsync(dto.Code))
			{
				return Conflict("Carrier with this code already exists.");
			}
			CarrierDto carrier = await _service.CreateAsync(dto);
			AddLinks(carrier);
			return CreatedAtAction(nameof(GetById),new { id = carrier.Id }, 
			carrier);

		}
		[HttpPut("{id}")]
		public async Task <ActionResult<CarrierDto>> Update(int id, UpdateCarrierDto dto){
			if (await _service.CodeExistsAsync(dto.Code, id))
			{
				return Conflict("Carrier with this code already exists.");
			}
			CarrierDto? carrier = await _service.UpdateAsync(id, dto);
			if (carrier == null) {
				return NotFound();
			}
			AddLinks(carrier);
			return Ok(carrier);
		}
		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			bool deleted = await _service.DeleteAsync(id);
			if(!deleted){
				return NotFound();
			}
			return NoContent();
		}
		private void AddLinks(CarrierDto carrier)
		{
			carrier.Links =
			[
				new()
		{
			Rel = "self",
			Href = $"/api/carriers/{carrier.Id}",
			Method = "GET"
		},
		new()
		{
			Rel = "update",
			Href = $"/api/carriers/{carrier.Id}",
			Method = "PUT"
		},
		new()
		{
			Rel = "delete",
			Href = $"/api/carriers/{carrier.Id}",
			Method = "DELETE"
		}
			];
		}

	}
}
