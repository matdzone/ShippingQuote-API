namespace ShippingQuote.Server.Models;

public class PricingRule
{
	public int Id { get; set; }

	public int DeliveryServiceId { get; set; }

	public decimal MinWeightKg { get; set; }

	public decimal MaxWeightKg { get; set; }

	public decimal Price { get; set; }

	public decimal? FreeFromOrderValue { get; set; }

	public string? DestinationCountryCode { get; set; }

	public DateTimeOffset ValidFrom { get; set; }

	public DateTimeOffset? ValidTo { get; set; }

	public int Priority { get; set; }

	public bool IsActive { get; set; } = true;

	public DeliveryService DeliveryService { get; set; } = null!;
}