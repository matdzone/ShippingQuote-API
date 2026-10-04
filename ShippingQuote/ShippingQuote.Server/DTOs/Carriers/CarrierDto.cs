using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ShippingQuote.Server.DTOs.Common;
namespace ShippingQuote.Server.DTOs.Carriers
{
	public class CarrierDto
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Code { get; set; } = string.Empty;

		public string? Description { get; set; }
		public bool IsActive { get; set; } 

		public DateTimeOffset CreatedAt { get; set; }
		[JsonPropertyName("_links")]
		public List<LinkDto> Links { get; set; } = [];
	}
}
