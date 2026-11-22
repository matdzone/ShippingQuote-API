using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShippingQuote.Server.Configuration;
using ShippingQuote.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShippingQuote.Server.Services
{
	public class TokenService
	{
		private readonly JwtOptions _options;
		public TokenService(IOptions<JwtOptions> options)
		{ 
			_options = options.Value;
		}
		public (string Token, DateTime ExpiresAt) CreateAccessToken(User user)
		{
			DateTime expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

			List<Claim> claims =
			[
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

				new Claim(ClaimTypes.Email, user.Email),

				new Claim(ClaimTypes.Role, user.Role.ToString()),

				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			];
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

			SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken token = new JwtSecurityToken(
				issuer: _options.Issuer,
				audience: _options.Audience,
				claims: claims,
				expires: expiresAt,
				signingCredentials: credentials
			);
			string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
			return (tokenString, expiresAt);

		}
		public string GenerateRefreshToken() 
		{
			byte[] bytes = RandomNumberGenerator.GetBytes(64);
			return Convert.ToBase64String(bytes);
		}
		public string HashRefreshToken(string token)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(token);

			byte[] hash = SHA256.HashData(bytes);

			return Convert.ToHexString(hash);
		}
		public DateTime GetRefreshTokenExpiration()
		{
			return DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
		}
	}
}
