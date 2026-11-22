using System.ComponentModel.DataAnnotations;
namespace ShippingQuote.Server.DTOs.Auth
{
	public class RefreshTokenRequestDto
	{
		[Required]
		public string RefreshToken { get; set; } = string.Empty;
	}
}
