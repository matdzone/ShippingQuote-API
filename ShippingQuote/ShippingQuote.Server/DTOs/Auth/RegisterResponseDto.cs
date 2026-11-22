namespace ShippingQuote.Server.DTOs.Auth
{
	public class RegisterResponseDto
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public DateTimeOffset CreatedAt { get; set; }
	}
}
