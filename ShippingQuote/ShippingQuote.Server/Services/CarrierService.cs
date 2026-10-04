using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.DTOs.Carriers;
using ShippingQuote.Server.Models;
using ShippingQuote.Server.DTOs.Common;
namespace ShippingQuote.Server.Services
{
	public class CarrierService
	{
		private readonly ShippingQuoteDbContext _context;
		public CarrierService(ShippingQuoteDbContext context){
			_context = context;
		}
		/*public async Task<List<CarrierDto>> GetAllAsync()
		{
			return await _context.Carriers.OrderBy(c  => c.Name)
			.Select(c => new CarrierDto{
				Id = c.Id,
				Name = c.Name,
				Code = c.Code,
				Description = c.Description,
				IsActive = c.IsActive,
				CreatedAt = c.CreatedAt
			}).ToListAsync();
		}*/
		public async Task<PagedResult<CarrierDto>> GetAllAsync(
	int page, int pageSize, string? name, bool? isActive)
		{
			IQueryable<Carrier> query =
				_context.Carriers.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(name))
			{
				query = query.Where(c => c.Name.Contains(name));
			}

			if (isActive.HasValue)
			{
				query = query.Where(c => c.IsActive == isActive.Value);
			}

			int totalCount = await query.CountAsync();

			List<CarrierDto> items = await query
				.OrderBy(c => c.Name)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(c => new CarrierDto
				{
					Id = c.Id,
					Name = c.Name,
					Code = c.Code,
					Description = c.Description,
					IsActive = c.IsActive,
					CreatedAt = c.CreatedAt
				})
				.ToListAsync();

			return new PagedResult<CarrierDto>
			{
				Items = items,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount,
				TotalPages = (int)Math.Ceiling(
					totalCount / (double)pageSize)
			};
		}
		public async Task<CarrierDto?> GetByIdAsync(int id)
		{
			return await _context.Carriers.Where(c => c.Id == id).
			Select(c => new CarrierDto
			{
				Id = c.Id,
				Name = c.Name,
				Code = c.Code,
				Description = c.Description,
				IsActive = c.IsActive,
				CreatedAt = c.CreatedAt
			}).FirstOrDefaultAsync();
		}
		public async Task<CarrierDto> CreateAsync(CreateCarrierDto dto)
		{
			Carrier carrier = new Carrier
			{
				Name = dto.Name,
				Code = dto.Code,
				Description = dto.Description,
			};
			/*bool codeExists = await _context.Carriers.
				AnyAsync(c => c.Code == dto.Code);*/
			_context.Carriers.Add(carrier);
			await _context.SaveChangesAsync();
			return new CarrierDto
			{
				Id = carrier.Id,
				Name = carrier.Name,
				Code = carrier.Code,
				Description = carrier.Description,
				IsActive = carrier.IsActive,
				CreatedAt = carrier.CreatedAt
			};
		}
		public async Task<CarrierDto?> UpdateAsync(int id, UpdateCarrierDto dto)
		{
			Carrier? carrier = await _context.Carriers.
				FirstOrDefaultAsync(c => c.Id == id);
			if (carrier == null)
			{
				return null;
			}
			/*bool codeExists = await _context.Carriers.
				AnyAsync(c => c.Code == dto.Code && c.Id != id);*/
			carrier.Name = dto.Name;
			carrier.Code = dto.Code;
			carrier.Description = dto.Description;
			carrier.IsActive = dto.IsActive;
			await _context.SaveChangesAsync();
			return new CarrierDto
			{
				Id = carrier.Id,
				Name = carrier.Name,
				Code = carrier.Code,
				Description = carrier.Description,
				IsActive = carrier.IsActive,
				CreatedAt = carrier.CreatedAt
			};
		}
		public async Task<bool> DeleteAsync(int id)
		{
			Carrier? carrier = await _context.Carriers.
				FirstOrDefaultAsync(c => c.Id == id);
			if(carrier == null){
				return false;
			}
			_context.Carriers.Remove(carrier);
			await _context.SaveChangesAsync();
			return true;
			
		}
		public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
		{
			return await _context.Carriers
				.AnyAsync(c =>
					c.Code == code &&
					(!excludeId.HasValue || c.Id != excludeId.Value));
		}
	}
}
