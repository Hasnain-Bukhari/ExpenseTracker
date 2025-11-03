using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Auth;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IPasswordResetRepository _passwordResetRepo;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly AuthOptions _options;

        public AuthService(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshRepo,
            IPasswordResetRepository passwordResetRepo,
            ITokenService tokenService,
            IEmailService emailService,
            AuthOptions options)
        {
            _userRepo = userRepo;
            _refreshRepo = refreshRepo;
            _passwordResetRepo = passwordResetRepo;
            _tokenService = tokenService;
            _emailService = emailService;
            _options = options;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existing = await _userRepo.GetByEmailAsync(request.Email);
            if (existing != null) throw new InvalidOperationException("Email already exists");

            var now = DateTime.UtcNow;
            var user = new User(
                Guid.NewGuid(),
                request.Email,
                request.Email.ToUpperInvariant(),
                HashPassword(request.Password),
                request.Name,
                null, // DefaultCurrencyId - wiring to Currency entity will be added later
                _options.DefaultLocale,
                _options.DefaultTimezone,
                true,
                false,
                request.Phone,
                null, // ProfileImage
                null, // DefaultAccountId
                AuthProvider.Local,
                null,
                null,
                now,
                now
            );

            await _userRepo.CreateAsync(user);

            var access = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();
            var refreshHash = _tokenService.HashRefreshToken(refresh);

            var refreshEntity = new RefreshToken(Guid.NewGuid(), user.Id, refreshHash, null, DateTimeOffset.UtcNow.AddDays(_options.RefreshDays), DateTimeOffset.UtcNow, null);
            await _refreshRepo.CreateAsync(refreshEntity);

            await _emailService.SendWelcomeEmailAsync(user);

            return new AuthResponse(true, new AuthUserDto(user.Id, user.FullName ?? "", user.Email), access, refresh, _options.AccessMinutes * 60);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");
            if (user.PasswordHash == null) throw new UnauthorizedAccessException("No local password set for this user");
            if (!VerifyPassword(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid credentials");

            var access = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();
            var refreshHash = _tokenService.HashRefreshToken(refresh);
            var refreshEntity = new RefreshToken(Guid.NewGuid(), user.Id, refreshHash, null, DateTimeOffset.UtcNow.AddDays(_options.RefreshDays), DateTimeOffset.UtcNow, null);
            await _refreshRepo.CreateAsync(refreshEntity);

            return new AuthResponse(true, new AuthUserDto(user.Id, user.FullName ?? "", user.Email), access, refresh, _options.AccessMinutes * 60);
        }

        public async Task<AuthResponse> SocialSignInAsync(SocialLoginRequest request)
        {
            // Mock mode handling
            var provider = request.Provider.ToLowerInvariant();
            SocialVerificationResult? verification = null;
            if (_options.SocialMock && request.Token.StartsWith("mock-"))
            {
                // token format: mock-google:email
                var parts = request.Token.Split(':', 2);
                var email = parts.Length > 1 ? parts[1] : $"{Guid.NewGuid()}@example.com";
                verification = new SocialVerificationResult(email, Guid.NewGuid().ToString());
            }
            else
            {
                verification = await SocialVerifier.VerifyAsync(provider, request.Token, _options);
            }

            if (verification == null) throw new UnauthorizedAccessException("Invalid social token");

            var user = await _userRepo.GetByProviderAsync(provider, verification.ProviderId);
            if (user == null)
            {
                var byEmail = await _userRepo.GetByEmailAsync(verification.Email);
                if (byEmail != null)
                {
                    if (_options.AllowSocialAutoLink)
                    {
                        // link account - set properties directly on mutable User class
                        byEmail.Provider = provider == "google" ? AuthProvider.Google : AuthProvider.Facebook;
                        byEmail.ProviderId = verification.ProviderId;
                        await _userRepo.UpdateAsync(byEmail);
                        user = byEmail;
                    }
                    else
                    {
                        throw new InvalidOperationException("Email exists without social link");
                    }
                }
            }

            if (user == null)
            {
                var now = DateTimeOffset.UtcNow;
                var newUser = new User(
                    Guid.NewGuid(), 
                    verification.Email, 
                    verification.Email.ToUpperInvariant(), 
                    null, 
                    verification.Name, 
                    null, 
                    _options.DefaultLocale, 
                    _options.DefaultTimezone, 
                    true, 
                    true, 
                    null, 
                    null, // ProfileImage
                    null, // DefaultAccountId
                    provider == "google" ? AuthProvider.Google : AuthProvider.Facebook, 
                    verification.ProviderId, 
                    DateTime.Now, 
                    now.DateTime, 
                    now.DateTime
                );
                await _userRepo.CreateAsync(newUser);
                user = newUser;
            }

            var access = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();
            var refreshHash = _tokenService.HashRefreshToken(refresh);
            var refreshEntity = new RefreshToken(Guid.NewGuid(), user.Id, refreshHash, null, DateTimeOffset.UtcNow.AddDays(_options.RefreshDays), DateTimeOffset.UtcNow, null);
            await _refreshRepo.CreateAsync(refreshEntity);

            return new AuthResponse(true, new AuthUserDto(user.Id, user.FullName ?? "", user.Email), access, refresh, _options.AccessMinutes * 60);
        }

        public async Task LogoutAsync(Guid userId, string refreshToken)
        {
            var hash = _tokenService.HashRefreshToken(refreshToken);
            var existing = await _refreshRepo.GetByTokenHashAsync(hash);
            if (existing != null)
            {
                await _refreshRepo.RevokeAsync(existing.Id);
            }
        }

        public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
        {
            var hash = _tokenService.HashRefreshToken(request.RefreshToken);
            var existing = await _refreshRepo.GetByTokenHashAsync(hash);
            if (existing == null) throw new UnauthorizedAccessException("Invalid refresh token");
            if (existing.ExpiresAt < DateTimeOffset.UtcNow || existing.RevokedAt != null) throw new UnauthorizedAccessException("Invalid refresh token");

            var user = await _userRepo.GetByIdAsync(existing.UserId);
            if (user == null) throw new UnauthorizedAccessException("Invalid refresh token");

            // rotate
            var newRefresh = _tokenService.GenerateRefreshToken();
            var newHash = _tokenService.HashRefreshToken(newRefresh);
            var newEntity = new RefreshToken(Guid.NewGuid(), user.Id, newHash, existing.DeviceInfo, DateTimeOffset.UtcNow.AddDays(_options.RefreshDays), DateTimeOffset.UtcNow, null);
            await _refreshRepo.RotateAsync(existing.Id, newEntity);

            var access = _tokenService.GenerateAccessToken(user);
            return new AuthResponse(true, new AuthUserDto(user.Id, user.FullName ?? "", user.Email), access, newRefresh, _options.AccessMinutes * 60);
        }

        public async Task RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null) return; // avoid enumeration

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            var hash = _tokenService.HashRefreshToken(token);
            var pr = new PasswordResetToken(Guid.NewGuid(), user.Id, hash, DateTime.UtcNow.AddHours(_options.ResetHours), false, DateTime.UtcNow);
            await _passwordResetRepo.CreateAsync(pr);
            await _emailService.SendPasswordResetAsync(user, token);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Handle token lookup - tokens can come from different sources:
            // 1. JSON body (already decoded by JSON parser)
            // 2. URL query parameter (might be URL-encoded)
            // Try token as-is first, then try decoded versions
            
            var hash = _tokenService.HashRefreshToken(request.Token);
            var existing = await _passwordResetRepo.GetByTokenHashAsync(hash);
            
            // If not found, try decoding the token (it might be URL-encoded)
            if (existing == null)
            {
                // Try Uri.UnescapeDataString (preserves + as +, not space - important for base64)
                try
                {
                    var decodedToken = Uri.UnescapeDataString(request.Token);
                    if (decodedToken != request.Token)
                    {
                        var decodedHash = _tokenService.HashRefreshToken(decodedToken);
                        existing = await _passwordResetRepo.GetByTokenHashAsync(decodedHash);
                    }
                }
                catch
                {
                    // Ignore decoding errors
                }
            }
            
            // If still not found and token contains %XX encoding, try WebUtility.UrlDecode
            if (existing == null && request.Token.Contains('%'))
            {
                try
                {
                    var webDecoded = System.Net.WebUtility.UrlDecode(request.Token);
                    // Only use if it's different and doesn't contain spaces (which would break base64)
                    if (webDecoded != request.Token && !webDecoded.Contains(' '))
                    {
                        var webDecodedHash = _tokenService.HashRefreshToken(webDecoded);
                        existing = await _passwordResetRepo.GetByTokenHashAsync(webDecodedHash);
                    }
                }
                catch
                {
                    // Ignore decoding errors
                }
            }
            
            if (existing == null) throw new InvalidOperationException("Invalid or expired token");
            if (existing.ExpiresAt < DateTimeOffset.UtcNow || existing.Used) throw new InvalidOperationException("Invalid or expired token");

            var user = await _userRepo.GetByIdAsync(existing.UserId);
            if (user == null) throw new InvalidOperationException("Invalid token");

            // Update mutable user instance instead of using record 'with' expression
            user.PasswordHash = HashPassword(request.NewPassword);
            await _userRepo.UpdateAsync(user);
            await _passwordResetRepo.MarkUsedAsync(existing.Id);
        }

        private static string HashPassword(string? password)
        {
            if (password == null) return string.Empty;
            // PBKDF2 (HMACSHA512) with salt
            var salt = RandomNumberGenerator.GetBytes(16);
            const int iterations = 100_000;
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
            var hash = pbkdf2.GetBytes(64);
            return $"{iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyPassword(string password, string stored)
        {
            try
            {
                var parts = stored.Split(':');
                if (parts.Length != 3) return false;
                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var hash = Convert.FromBase64String(parts[2]);
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
                var computed = pbkdf2.GetBytes(hash.Length);
                return CryptographicOperations.FixedTimeEquals(hash, computed);
            }
            catch
            {
                return false;
            }
        }
    }
}
