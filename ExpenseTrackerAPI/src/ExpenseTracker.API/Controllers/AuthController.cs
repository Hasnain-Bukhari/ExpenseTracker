using System;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Auth;
using ExpenseTracker.Service.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!req.AcceptTerms) return BadRequest(new { ok = false, error = "Terms must be accepted" });
            try
            {
                var result = await _authService.RegisterAsync(req);
                return Created(string.Empty, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { ok = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var result = await _authService.LoginAsync(req);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { ok = false, error = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("social")]
        public async Task<IActionResult> Social([FromBody] SocialLoginRequest req)
        {
            try
            {
                var result = await _authService.SocialSignInAsync(req);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { ok = false, error = "Invalid social token" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { ok = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            try
            {
                var result = await _authService.RefreshAsync(req);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { ok = false, error = "Invalid refresh token" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
        {
            try
            {
                var sub = User.FindFirst("sub")?.Value;
                if (sub == null) return Unauthorized();
                await _authService.LogoutAsync(Guid.Parse(sub), req.RefreshToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> Forgot([FromBody] ForgotPasswordRequest req)
        {
            try
            {
                await _authService.RequestPasswordResetAsync(req);
                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest req)
        {
            try
            {
                await _authService.ResetPasswordAsync(req);
                return Ok(new { ok = true });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { ok = false, error = "Invalid or expired token" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
