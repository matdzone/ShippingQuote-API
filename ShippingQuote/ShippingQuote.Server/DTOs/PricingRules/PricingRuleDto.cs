namespace ShippingQuote.Server.DTOs.PricingRules;
using System.Text.Json.Serialization;
using ShippingQuote.Server.DTOs.Common;
public class PricingRuleDto
{
	public int Id { get; set; }
	public int DeliveryServiceId { get; set; }

	public decimal MinWeightKg { get; set; }
	public decimal MaxWeightKg { get; set; }

	public decimal Price { get; set; }
	public decimal? FreeFromOrderValue { get; set; }

	public DateTimeOffset ValidFrom { get; set; }
	public DateTimeOffset? ValidTo { get; set; }

	public bool IsActive { get; set; }
	[JsonPropertyName("_links")]
	public List<LinkDto> Links { get; set; } = [];
}