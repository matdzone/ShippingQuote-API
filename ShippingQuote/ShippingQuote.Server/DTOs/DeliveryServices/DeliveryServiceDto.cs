using ShippingQuote.Server.Enums;
using System.Text.Json.Serialization;
using ShippingQuote.Server.DTOs.Common;
namespace ShippingQuote.Server.DTOs.DeliveryServices;

public class DeliveryServiceDto
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

	public bool IsActive { get; set; }
	[JsonPropertyName("_links")]
	public List<LinkDto> Links { get; set; } = [];
}