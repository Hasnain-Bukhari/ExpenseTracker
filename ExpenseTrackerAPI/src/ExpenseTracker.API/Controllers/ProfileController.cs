using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Dtos.Profile;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Service.Services;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var profile = await _profileService.GetProfileAsync(userId);
                return Ok(profile);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Profile not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ProfileDto>> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var userId = GetUserId();
                var updatedProfile = await _profileService.UpdateProfileAsync(userId, dto);
                return Ok(updatedProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _profileService.ChangePasswordAsync(userId, dto);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("image")]
        public async Task<ActionResult<ProfileImageDto>> UpdateProfileImage([FromBody] ProfileImageDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _profileService.UpdateProfileImageAsync(userId, dto.ImageUrl);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("accounts/{currencyId}")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByCurrency(Guid currencyId)
        {
            try
            {
                var userId = GetUserId();
                var accounts = await _profileService.GetAccountsByCurrencyAsync(userId, currencyId);
                return Ok(accounts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private Guid GetUserId()
        {
            var sub = User.FindFirst("sub")?.Value
             ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
             ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return userId;
        }
    }
}
