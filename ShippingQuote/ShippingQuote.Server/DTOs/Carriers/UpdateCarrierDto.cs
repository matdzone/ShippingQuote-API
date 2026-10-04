using System.ComponentModel.DataAnnotations;

namespace ShippingQuote.Server.DTOs.Carriers
{
	public class UpdateCarrierDto
	{
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[StringLength(20)]
		public string Code { get; set; } = string.Empty;

		[StringLength(500)]
		public string? Description { get; set; }
		public bool IsActive { get; set; } 
	}
}
