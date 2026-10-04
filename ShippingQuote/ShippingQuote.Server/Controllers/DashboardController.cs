using Microsoft.AspNetCore.Mvc;
using ShippingQuote.Server.DTOs.Dashboard;
using ShippingQuote.Server.Services;

namespace ShippingQuote.Server.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
	private readonly DashboardService _service;

	public DashboardController(DashboardService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<ActionResult<DashboardDto>> Get()
	{
		DashboardDto dashboard =
			await _service.GetDashboardAsync();

		return Ok(dashboard);
	}
}