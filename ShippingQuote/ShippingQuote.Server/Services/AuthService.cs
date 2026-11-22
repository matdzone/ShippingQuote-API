using ShippingQuote.Server.DTOs.Auth;
using ShippingQuote.Server.Models;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ShippingQuote.Server.Services
{
	public class AuthService
	{
		private readonly ShippingQuoteDbContext _context;
		private readonly IPasswordHasher<User> _passwordHasher;
		private readonly TokenService _tokenService;
		public AuthService(ShippingQuoteDbContext context, IPasswordHasher<User> passwordHasher, TokenService tokenService)
		{
			_context = context;
			_passwordHasher = passwordHasher;
			_tokenService = tokenService;
		}
		public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto dto)
		{
			bool emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
			if (emailExists)
			{
				return null;
			}
			User user = new User();
			user.Email = dto.Email;
			user.Role = UserRole.Customer;

			user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

			_context.Users.Add(user);

			await _context.SaveChangesAsync();

			RegisterResponseDto response = new RegisterResponseDto
			{
				Id = user.Id,
				Email = user.Email,
				Role = user.Role.ToString(),
				CreatedAt = user.CreatedAt
			};

			return response;

		}
		public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (user is null)
			{
				return null;
			}
			if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password)
				== PasswordVerificationResult.Failed)
			{
				return null;
			}
			var accessToken = _tokenService.CreateAccessToken(user);
			string refreshToken = _tokenService.GenerateRefreshToken();
			string refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
			RefreshToken storedRefreshToken = new RefreshToken
			{
				UserId = user.Id,
				TokenHash = refreshTokenHash,
				ExpiresAt = _tokenService.GetRefreshTokenExpiration()
			};
			_context.RefreshTokens.Add(storedRefreshToken);
			await _context.SaveChangesAsync();

			AuthResponseDto response = new AuthResponseDto
			{
				AccessToken = accessToken.Token,
				RefreshToken = refreshToken,
				AccessTokenExpiresAt = accessToken.ExpiresAt,
				UserId = user.Id,
				Email = user.Email,
				Role = user.Role.ToString(),
			};
			return response;
		}
		public async Task<AuthResponseDto?> RefreshAsync(RefreshTokenRequestDto dto)
		{
			string tokenHash = _tokenService.HashRefreshToken(dto.RefreshToken);
			RefreshToken? storedToken = await _context.RefreshTokens
				.Include(r => r.User)
				.FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
			if (storedToken == null || !storedToken.IsActive)
			{
				return null;
			}

			storedToken.RevokedAt = DateTime.UtcNow;

			var accessToken =
				_tokenService.CreateAccessToken(storedToken.User);

			string newRefreshToken =
				_tokenService.GenerateRefreshToken();

			string newRefreshTokenHash =
				_tokenService.HashRefreshToken(newRefreshToken);

			RefreshToken newStoredToken = new RefreshToken
			{
				UserId = storedToken.UserId,
				TokenHash = newRefreshTokenHash,
				ExpiresAt =
					_tokenService.GetRefreshTokenExpiration()
			};

			_context.RefreshTokens.Add(newStoredToken);

			await _context.SaveChangesAsync();

			return new AuthResponseDto
			{
				AccessToken = accessToken.Token,
				RefreshToken = newRefreshToken,
				AccessTokenExpiresAt = accessToken.ExpiresAt,
				UserId = storedToken.User.Id,
				Email = storedToken.User.Email,
				Role = storedToken.User.Role.ToString()
			};
		}
		public async Task<bool> LogoutAsync(RefreshTokenRequestDto dto)
		{
			string tokenHash = _tokenService.HashRefreshToken(dto.RefreshToken);

			RefreshToken? storedToken =
				await _context.RefreshTokens
					.FirstOrDefaultAsync(r =>
						r.TokenHash == tokenHash);

			if (storedToken == null)
			{
				return false;
			}

			if (storedToken.RevokedAt != null)
			{
				return false;
			}

			storedToken.RevokedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return true;
		} 
	}
}
