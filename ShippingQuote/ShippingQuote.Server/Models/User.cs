using ShippingQuote.Server.Enums;

namespace ShippingQuote.Server.Models;

public class User
{
	public int Id { get; set; }

	public string Email { get; set; } = string.Empty;

	public string PasswordHash { get; set; } = string.Empty;

	public UserRole Role { get; set; }

	public int? CarrierId { get; set; }

	public bool IsActive { get; set; } = true;

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

	public Carrier? Carrier { get; set; }
}