using ShippingQuote.Server.Enums;

namespace ShippingQuote.Server.Models;

public class DeliveryService
{
	public int Id { get; set; }

	public int CarrierId { get; set; }

	public string Name { get; set; } = string.Empty;

	public DeliveryServiceType Type { get; set; }

	public decimal MaxWeightKg { get; set; }

	public decimal MaxLengthCm { get; set; }

	public decimal MaxWidthCm { get; set; }

	public decimal MaxHeightCm { get; set; }

	public int MinDeliveryDays { get; set; }

	public int MaxDeliveryDays { get; set; }

	public bool IsActive { get; set; } = true;

	public Carrier Carrier { get; set; } = null!;

	public ICollection<PricingRule> PricingRules { get; set; }
		= new List<PricingRule>();
}