using Microsoft.AspNetCore.Mvc;
using ShippingQuote.Server.Services;
using ShippingQuote.Server.DTOs.Auth;
namespace ShippingQuote.Server.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly AuthService _service;
		public AuthController(AuthService service)
		{
			_service = service;
		}
		[HttpPost("register")]
		public async Task<ActionResult<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
		{
			RegisterResponseDto? result = await _service.RegisterAsync(request);
			if (result == null)
			{
				return Conflict("User with this email already exists.");
			}
			return StatusCode(StatusCodes.Status201Created, result);
		}
		[HttpPost("login")]
		public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
		{
			AuthResponseDto? result = await _service.LoginAsync(request);
			if (result == null)
			{
				return Unauthorized("Invalid email or password.");
			}
			return Ok(result);
		}
		[HttpPost("refresh")]
		public async Task<ActionResult<AuthResponseDto>> RefreshAsync(RefreshTokenRequestDto request)
		{
			AuthResponseDto? result = await _service.RefreshAsync(request);

			if (result == null)
			{
				return Unauthorized("Invalid or expired refresh token.");
			}

			return Ok(result);
		}
		[HttpPost("logout")]
		public async Task<ActionResult> LogoutAsync(RefreshTokenRequestDto request)
		{
			bool loggedOut = await _service.LogoutAsync(request);

			if (!loggedOut)
			{
				return Unauthorized("Invalid refresh token.");
			}

			return NoContent();
		}
	}
}
