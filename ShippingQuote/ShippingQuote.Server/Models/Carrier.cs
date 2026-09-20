namespace ShippingQuote.Server.Models;

public class Carrier
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Code { get; set; } = string.Empty;

	public string? Description { get; set; }

	public string? Website { get; set; }

	public bool IsActive { get; set; } = true;

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

	public ICollection<DeliveryService> DeliveryServices { get; set; }
		= new List<DeliveryService>();

	public ICollection<User> Users { get; set; }
		= new List<User>();
}