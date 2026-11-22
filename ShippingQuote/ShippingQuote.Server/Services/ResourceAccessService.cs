using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.Enums;
using ShippingQuote.Server.Models;
using System.Security.Claims;

namespace ShippingQuote.Server.Services;

public class ResourceAccessService
{
	private readonly ShippingQuoteDbContext _context;

	public ResourceAccessService(ShippingQuoteDbContext context)
	{
		_context = context;
	}

	private async Task<User?> GetCurrentUserAsync(
		ClaimsPrincipal principal)
	{
		string? userIdClaim =
			principal.FindFirstValue(ClaimTypes.NameIdentifier);

		if (!int.TryParse(userIdClaim, out int userId))
		{
			return null;
		}

		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == userId);
	}
	public async Task<bool> CanManageCarrierAsync(
	ClaimsPrincipal principal,
	int carrierId)
	{
		User? user = await GetCurrentUserAsync(principal);

		if (user == null)
		{
			return false;
		}

		if (user.Role == UserRole.Administrator)
		{
			return true;
		}

		if (user.Role != UserRole.CarrierManager)
		{
			return false;
		}

		return user.CarrierId == carrierId;
	}
	public async Task<bool> CanManageDeliveryServiceAsync(
	ClaimsPrincipal principal,
	int deliveryServiceId)
	{
		User? user = await GetCurrentUserAsync(principal);

		if (user == null)
		{
			return false;
		}

		if (user.Role == UserRole.Administrator)
		{
			return true;
		}

		if (user.Role != UserRole.CarrierManager ||
			!user.CarrierId.HasValue)
		{
			return false;
		}

		return await _context.DeliveryServices
			.AnyAsync(d =>
				d.Id == deliveryServiceId &&
				d.CarrierId == user.CarrierId.Value);
	}
}
