using System.ComponentModel.DataAnnotations;

namespace ShippingQuote.Server.DTOs.PricingRules;

public class CreatePricingRuleDto
{
	[Range(1, int.MaxValue)]
	public int DeliveryServiceId { get; set; }

	[Range(typeof(decimal), "0", "1000")]
	public decimal MinWeightKg { get; set; }

	[Range(typeof(decimal), "0.01", "1000")]
	public decimal MaxWeightKg { get; set; }

	[Range(typeof(decimal), "0", "100000")]
	public decimal Price { get; set; }

	[Range(typeof(decimal), "0", "1000000")]
	public decimal? FreeFromOrderValue { get; set; }

	public DateTimeOffset ValidFrom { get; set; }

	public DateTimeOffset? ValidTo { get; set; }
}