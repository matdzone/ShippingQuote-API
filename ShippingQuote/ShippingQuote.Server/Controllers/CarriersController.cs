using Microsoft.AspNetCore.Mvc;
using ShippingQuote.Server.DTOs.Carriers;
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
		[HttpGet]
		public async Task <ActionResult<List<CarrierDto>>> GetAll(){
			List<CarrierDto> carriers = await _service.GetAllAsync();
			return Ok(carriers);
		}
		[HttpGet("{id}")]
		public async Task <ActionResult<CarrierDto>> GetById(int id){
			CarrierDto? carrier = await _service.GetByIdAsync(id);
			if(carrier == null)
			{
				return NotFound();
			}
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

	}
}
