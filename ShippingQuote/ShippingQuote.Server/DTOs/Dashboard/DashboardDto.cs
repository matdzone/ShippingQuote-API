namespace ShippingQuote.Server.DTOs.Dashboard;

public class DashboardDto
{
	public int TotalCarriers { get; set; }
	public int ActiveCarriers { get; set; }

	public int TotalDeliveryServices { get; set; }
	public int ActiveDeliveryServices { get; set; }

	public int TotalPricingRules { get; set; }
	public int ActivePricingRules { get; set; }
}