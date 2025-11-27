using ExpenseTracker.Dtos.Profile;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Dtos.AccountTypes;
using ExpenseTracker.Dtos.Currencies;
using ExpenseTracker.Repository.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseTracker.Service.Services
{
    public class ProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IAccountRepository _accountRepository;

        public ProfileService(
            IUserRepository userRepository,
            ICurrencyRepository currencyRepository,
            IAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _currencyRepository = currencyRepository;
            _accountRepository = accountRepository;
        }

        public async Task<ProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            Console.WriteLine($"ProfileService: Retrieved user {userId} with email {user.Email}");
            
            // Load navigation properties separately to avoid lazy loading issues
            CurrencyDto? defaultCurrency = null;
            if (user.DefaultCurrencyId.HasValue)
            {
                var currency = await _currencyRepository.GetByIdAsync(user.DefaultCurrencyId.Value);
                if (currency != null)
                {
                    defaultCurrency = new CurrencyDto(currency.Id, currency.UserId, currency.Code, currency.Symbol, currency.Name, currency.CreatedAt, currency.UpdatedAt);
                }
            }
            
            AccountDto? defaultAccount = null;
            if (user.DefaultAccountId.HasValue)
            {
                var account = await _accountRepository.GetByIdAsync(user.DefaultAccountId.Value);
                if (account != null)
                {
                    // For AccountDto, we need to load AccountType and Currency separately
                    var accountType = account.AccountType != null 
                        ? new AccountTypeDto(account.AccountType.Id, account.AccountType.Name, account.AccountType.IsCard, account.AccountType.CreatedAt, account.AccountType.UpdatedAt)
                        : null;
                        
                    var currency = account.Currency != null
                        ? new CurrencyDto(account.Currency.Id, account.Currency.UserId, account.Currency.Code, account.Currency.Symbol, account.Currency.Name, account.Currency.CreatedAt, account.Currency.UpdatedAt)
                        : null;
                        
                    defaultAccount = new AccountDto(
                        account.Id, 
                        account.UserId, 
                        account.Name, 
                        account.AccountTypeId, 
                        account.CurrencyId, 
                        account.IsSavings, 
                        0, // OpeningBalance - not available in User entity
                        true, // IncludeInNetworth - default value
                        account.CreatedAt, 
                        account.UpdatedAt,
                        accountType,
                        currency
                    );
                }
            }
            
            var dto = new ProfileDto(
                user.Id,
                user.Email,
                user.FullName,
                user.PreferredName,
                user.Phone,
                user.ProfileImage,
                user.DefaultCurrencyId,
                defaultCurrency,
                user.DefaultAccountId,
                defaultAccount,
                user.Locale,
                user.Timezone,
                user.IsEmailVerified,
                user.Provider,
                user.ProviderId,
                user.LastLoginAt,
                user.CreatedAt,
                user.UpdatedAt
            );
            
            Console.WriteLine($"MapToProfileDto: Created DTO with FullName={dto.FullName}, Phone={dto.Phone}, DefaultCurrencyId={dto.DefaultCurrencyId}, DefaultAccountId={dto.DefaultAccountId}");
            return dto;
        }

        public async Task<ProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Validate currency if provided
            if (dto.DefaultCurrencyId.HasValue)
            {
                var currency = await _currencyRepository.GetByIdAsync(dto.DefaultCurrencyId.Value);
                if (currency == null || currency.UserId != userId)
                    throw new ArgumentException("Invalid currency selected");
            }

            // Validate account if provided
            if (dto.DefaultAccountId.HasValue)
            {
                var account = await _accountRepository.GetByIdAsync(dto.DefaultAccountId.Value);
                if (account == null || account.UserId != userId)
                    throw new ArgumentException("Invalid account selected");
            }

            // Update user properties
            user.FullName = dto.FullName;
            user.PreferredName = dto.PreferredName;
            user.Phone = dto.Phone;
            user.ProfileImage = dto.ProfileImage;
            user.DefaultCurrencyId = dto.DefaultCurrencyId;
            user.DefaultAccountId = dto.DefaultAccountId;
            user.Locale = dto.Locale;
            user.Timezone = dto.Timezone;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // Return the updated profile data directly instead of reloading
            // Load navigation properties separately to avoid lazy loading issues
            CurrencyDto? defaultCurrency = null;
            if (user.DefaultCurrencyId.HasValue)
            {
                var currency = await _currencyRepository.GetByIdAsync(user.DefaultCurrencyId.Value);
                if (currency != null)
                {
                    defaultCurrency = new CurrencyDto(currency.Id, currency.UserId, currency.Code, currency.Symbol, currency.Name, currency.CreatedAt, currency.UpdatedAt);
                }
            }
            
            AccountDto? defaultAccount = null;
            if (user.DefaultAccountId.HasValue)
            {
                var account = await _accountRepository.GetByIdAsync(user.DefaultAccountId.Value);
                if (account != null)
                {
                    // For AccountDto, we need to load AccountType and Currency separately
                    var accountType = account.AccountType != null 
                        ? new AccountTypeDto(account.AccountType.Id, account.AccountType.Name, account.AccountType.IsCard, account.AccountType.CreatedAt, account.AccountType.UpdatedAt)
                        : null;
                        
                    var currency = account.Currency != null
                        ? new CurrencyDto(account.Currency.Id, account.Currency.UserId, account.Currency.Code, account.Currency.Symbol, account.Currency.Name, account.Currency.CreatedAt, account.Currency.UpdatedAt)
                        : null;
                        
                    defaultAccount = new AccountDto(
                        account.Id, 
                        account.UserId, 
                        account.Name, 
                        account.AccountTypeId, 
                        account.CurrencyId, 
                        account.IsSavings, 
                        0, // OpeningBalance - not available in User entity
                        true, // IncludeInNetworth - default value
                        account.CreatedAt, 
                        account.UpdatedAt,
                        accountType,
                        currency
                    );
                }
            }
            
            var profileDto = new ProfileDto(
                user.Id,
                user.Email,
                user.FullName,
                user.PreferredName,
                user.Phone,
                user.ProfileImage,
                user.DefaultCurrencyId,
                defaultCurrency,
                user.DefaultAccountId,
                defaultAccount,
                user.Locale,
                user.Timezone,
                user.IsEmailVerified,
                user.Provider,
                user.ProviderId,
                user.LastLoginAt,
                user.CreatedAt,
                user.UpdatedAt
            );
            
            Console.WriteLine($"UpdateProfileAsync: Updated DTO with FullName={profileDto.FullName}, Phone={profileDto.Phone}, DefaultCurrencyId={profileDto.DefaultCurrencyId}, DefaultAccountId={profileDto.DefaultAccountId}");
            return profileDto;
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Check if user can change password (only local users)
            if (user.Provider != AuthProvider.Local)
                throw new UnauthorizedAccessException("Password change not allowed for social login users");

            // Validate current password
            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new ArgumentException("Current password is required");

            if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new ArgumentException("Current password is incorrect");

            // Validate new password
            if (string.IsNullOrEmpty(dto.NewPassword) || dto.NewPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters long");

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new ArgumentException("New password and confirmation do not match");

            // Hash new password
            user.PasswordHash = HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task<ProfileImageDto> UpdateProfileImageAsync(Guid userId, string imageData)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (string.IsNullOrEmpty(imageData))
                throw new ArgumentException("Image data is required");

            // Store the image data (can be base64 blob or URL)
            user.ProfileImage = imageData;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return new ProfileImageDto(imageData);
        }

        private static string HashPassword(string? password)
        {
            if (password == null) return string.Empty;
            // PBKDF2 (HMACSHA512) with salt - same as AuthService
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

        public async Task<IEnumerable<AccountDto>> GetAccountsByCurrencyAsync(Guid userId, Guid currencyId)
        {
            var accounts = await _accountRepository.ListByUserIdAndCurrencyAsync(userId, currencyId);
            var accountDtos = new List<AccountDto>();

            foreach (var account in accounts)
            {
                var accountTypeDto = account.AccountType != null 
                    ? new AccountTypeDto(account.AccountType.Id, account.AccountType.Name, account.AccountType.IsCard, account.AccountType.CreatedAt, account.AccountType.UpdatedAt)
                    : null;
                    
                var currencyDto = account.Currency != null
                    ? new CurrencyDto(account.Currency.Id, account.Currency.UserId, account.Currency.Code, account.Currency.Symbol, account.Currency.Name, account.Currency.CreatedAt, account.Currency.UpdatedAt)
                    : null;
                    
                accountDtos.Add(new AccountDto(
                    account.Id, 
                    account.UserId, 
                    account.Name, 
                    account.AccountTypeId, 
                    account.CurrencyId, 
                    account.IsSavings, 
                    account.OpeningBalance, 
                    account.IncludeInNetworth, 
                    account.CreatedAt, 
                    account.UpdatedAt,
                    accountTypeDto,
                    currencyDto
                ));
            }

            return accountDtos;
        }
    }
}
