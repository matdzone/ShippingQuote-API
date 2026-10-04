using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.DTOs.Dashboard;

namespace ShippingQuote.Server.Services;

public class DashboardService
{
	private readonly ShippingQuoteDbContext _context;

	public DashboardService(ShippingQuoteDbContext context)
	{
		_context = context;
	}

	public async Task<DashboardDto> GetDashboardAsync()
	{
		return new DashboardDto
		{
			TotalCarriers = await _context.Carriers.CountAsync(),
			ActiveCarriers = await _context.Carriers
				.CountAsync(c => c.IsActive),

			TotalDeliveryServices =
				await _context.DeliveryServices.CountAsync(),

			ActiveDeliveryServices =
				await _context.DeliveryServices
					.CountAsync(d => d.IsActive),

			TotalPricingRules =
				await _context.PricingRules.CountAsync(),

			ActivePricingRules =
				await _context.PricingRules
					.CountAsync(p => p.IsActive)
		};
	}
}