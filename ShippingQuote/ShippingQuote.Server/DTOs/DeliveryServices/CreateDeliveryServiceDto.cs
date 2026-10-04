using System.ComponentModel.DataAnnotations;
using ShippingQuote.Server.Enums;

namespace ShippingQuote.Server.DTOs.DeliveryServices;

public class CreateDeliveryServiceDto
{
	[Range(1, int.MaxValue)]
	public int CarrierId { get; set; }

	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;

	[EnumDataType(typeof(DeliveryServiceType))]
	public DeliveryServiceType Type { get; set; }

	[Range(typeof(decimal), "0.01", "1000")]
	public decimal MaxWeightKg { get; set; }

	[Range(typeof(decimal), "0.01", "1000")]
	public decimal MaxLengthCm { get; set; }

	[Range(typeof(decimal), "0.01", "1000")]
	public decimal MaxWidthCm { get; set; }

	[Range(typeof(decimal), "0.01", "1000")]
	public decimal MaxHeightCm { get; set; }

	[Range(1, 365)]
	public int MinDeliveryDays { get; set; }

	[Range(1, 365)]
	public int MaxDeliveryDays { get; set; }
}