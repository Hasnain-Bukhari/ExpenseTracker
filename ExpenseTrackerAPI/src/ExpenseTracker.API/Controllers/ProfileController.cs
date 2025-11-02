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
        public async Task<ActionResult<ProfileImageDto>> UpdateProfileImage([FromForm] IFormFile? file, [FromForm] string? imageUrl)
        {
            try
            {
                var userId = GetUserId();
                string imageData = null;

                if (file != null && file.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { error = "Invalid file type. Only JPG, PNG, GIF, and WebP are allowed." });
                    }

                    // Validate file size (max 5MB)
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(new { error = "File size exceeds 5MB limit." });
                    }

                    // Convert file to base64
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    var base64String = Convert.ToBase64String(fileBytes);
                    imageData = $"data:{file.ContentType};base64,{base64String}";
                }
                else if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Support legacy URL-based updates
                    imageData = imageUrl;
                }
                else
                {
                    return BadRequest(new { error = "Either a file or image URL is required." });
                }

                var result = await _profileService.UpdateProfileImageAsync(userId, imageData);
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
