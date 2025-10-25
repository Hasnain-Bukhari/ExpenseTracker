using System;

namespace ExpenseTracker.Dtos.Models
{
    // AccountType entity (replaces enum)
    public class AccountType
    {
        public virtual Guid Id { get; set; }
        public virtual Guid? UserId { get; set; }
        public virtual string Name { get; set; } = null!;
        // optional flag to indicate card-like types (we won't store CVV)
        public virtual bool IsCard { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        public AccountType() { }
        public AccountType(Guid id, Guid? userId, string name, bool isCard, DateTime createdAt, DateTime updatedAt)
        {
            Id = id; UserId = userId; Name = name; IsCard = isCard; CreatedAt = createdAt; UpdatedAt = updatedAt;
        }
    }


    // Authentication-related enum
    public enum AuthProvider { Local, Google, Facebook, Mixed }

    // Goal-related enums
    public enum GoalStatus 
    { 
        Active = 1, 
        Paused = 2, 
        Completed = 3, 
        Cancelled = 4 
    }

    public enum GoalPriority 
    { 
        Low = 1, 
        Medium = 2, 
        High = 3 
    }

    // User entity - converted from record to class for NHibernate proxy compatibility
    public class User
    {
        public virtual Guid Id { get; set; }
        public virtual string Email { get; set; } = null!;
        public virtual string NormalizedEmail { get; set; } = null!;
        public virtual string? PasswordHash { get; set; }
        public virtual string? FullName { get; set; }
        // Default currency is now a FK to Currency entity
        public virtual Guid? DefaultCurrencyId { get; set; }
        public virtual Currency? DefaultCurrency { get; set; }
        public virtual string? Locale { get; set; }
        public virtual string? Timezone { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsEmailVerified { get; set; }
        public virtual string? Phone { get; set; }
        public virtual string? ProfileImage { get; set; }
        public virtual Guid? DefaultAccountId { get; set; }
        public virtual Account? DefaultAccount { get; set; }
        public virtual AuthProvider Provider { get; set; }
        public virtual string? ProviderId { get; set; }
        public virtual DateTime? LastLoginAt { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        public User() { }

        public User(Guid id, string email, string normalizedEmail, string? passwordHash, string? fullName, Guid? defaultCurrencyId, string? locale, string? timezone, bool isActive, bool isEmailVerified, string? phone, string? profileImage, Guid? defaultAccountId, AuthProvider provider, string? providerId, DateTime? lastLoginAt, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Email = email;
            NormalizedEmail = normalizedEmail;
            PasswordHash = passwordHash;
            FullName = fullName;
            DefaultCurrencyId = defaultCurrencyId;
            Locale = locale;
            Timezone = timezone;
            IsActive = isActive;
            IsEmailVerified = isEmailVerified;
            Phone = phone;
            ProfileImage = profileImage;
            DefaultAccountId = defaultAccountId;
            Provider = provider;
            ProviderId = providerId;
            LastLoginAt = lastLoginAt;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Currency
    {
        public virtual Guid Id { get; set; }
        public virtual Guid? UserId { get; set; }
        public virtual string Code { get; set; } = null!;
        public virtual string? Symbol { get; set; }
        public virtual string? Name { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        public Currency() { }
        public Currency(Guid id, Guid? userId, string code, string? symbol, string? name, DateTime createdAt, DateTime updatedAt)
        {
            Id = id; UserId = userId; Code = code; Symbol = symbol; Name = name; CreatedAt = createdAt; UpdatedAt = updatedAt;
        }
    }

    public class Account
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string Name { get; set; } = null!;
        // Foreign key properties for easier API usage
        public virtual Guid AccountTypeId { get; set; }
        public virtual Guid CurrencyId { get; set; }
        // Navigation properties
        public virtual AccountType? AccountType { get; set; }
        public virtual Currency? Currency { get; set; }
        public virtual bool IsSavings { get; set; }
        public virtual decimal OpeningBalance { get; set; }
        public virtual bool IncludeInNetworth { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        public Account() { }
        public Account(Guid id, Guid userId, string name, Guid accountTypeId, Guid currencyId, bool isSavings, decimal openingBalance, bool includeInNetworth, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            AccountTypeId = accountTypeId;
            CurrencyId = currencyId;
            IsSavings = isSavings;
            OpeningBalance = openingBalance;
            IncludeInNetworth = includeInNetworth;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Category
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string Name { get; set; } = null!;
        public virtual string Description { get; set; } = null!;
        public virtual Guid? ParentId { get; set; }
        
        // Protected property for NHibernate mapping
        protected virtual string CategoryTypeString { get; set; } = null!;
        
        // Public enum property for API
        public virtual CategoryType CategoryType 
        { 
            get => CategoryTypeString switch
            {
                "Income" => CategoryType.Income,
                "Expense" => CategoryType.Expense,
                "TargetedSavingsGoal" => CategoryType.TargetedSavingsGoal,
                _ => throw new ArgumentException($"Unknown category type: {CategoryTypeString}")
            };
            set => CategoryTypeString = value switch
            {
                CategoryType.Income => "Income",
                CategoryType.Expense => "Expense",
                CategoryType.TargetedSavingsGoal => "TargetedSavingsGoal",
                _ => throw new ArgumentException($"Unknown category type: {value}")
            };
        }
        
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        // Navigation property for NHibernate one-to-many mapping
        public virtual System.Collections.Generic.IList<SubCategory> SubCategories { get; set; } = new System.Collections.Generic.List<SubCategory>();

        public Category() { }
        public Category(Guid id, Guid userId, string name, string description, Guid? parentId, CategoryType categoryType, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            ParentId = parentId;
            CategoryType = categoryType;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class SubCategory
    {
        public virtual Guid Id { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual string Name { get; set; } = null!;
        public virtual string? Description { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        // Back-reference for NHibernate many-to-one mapping
        public virtual Category? Category { get; set; }

        public SubCategory() { }

        public SubCategory(Guid id, Guid categoryId, string name, string? description, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            CategoryId = categoryId;
            Name = name;
            Description = description;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Category = new Category() {Id = CategoryId};
        }
    }

    public class Goal
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string Name { get; set; } = null!;
        public virtual string? Description { get; set; }
        public virtual decimal TargetAmount { get; set; }
        public virtual decimal CurrentAmount { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string? Tag { get; set; }
        
        // Public properties for NHibernate enum mapping
        public virtual string GoalStatusString { get; set; } = "Active";
        public virtual string GoalPriorityString { get; set; } = "Medium";
        
        // Public enum properties for API
        public virtual GoalStatus Status 
        { 
            get => Enum.Parse<GoalStatus>(GoalStatusString);
            set => GoalStatusString = value.ToString();
        }
        
        public virtual GoalPriority Priority 
        { 
            get => Enum.Parse<GoalPriority>(GoalPriorityString);
            set => GoalPriorityString = value.ToString();
        }
        
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        public Goal() { }
        public Goal(Guid id, Guid userId, string name, string? description, decimal targetAmount, decimal currentAmount, Guid categoryId, DateTime startDate, DateTime? endDate, string? tag, GoalStatus status, GoalPriority priority, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            TargetAmount = targetAmount;
            CurrentAmount = currentAmount;
            CategoryId = categoryId;
            StartDate = startDate;
            EndDate = endDate;
            Tag = tag;
            Status = status;
            Priority = priority;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Transaction
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual Guid AccountId { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual Guid SubCategoryId { get; set; }
        public virtual string? Description { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Account? Account { get; set; }
        public virtual Category? Category { get; set; }
        public virtual SubCategory? SubCategory { get; set; }

        public Transaction() { }
        public Transaction(Guid id, Guid userId, Guid accountId, Guid categoryId, Guid subCategoryId, string? description, decimal amount, DateTime transactionDate, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            AccountId = accountId;
            Account = new Account() {Id = AccountId};
            CategoryId = categoryId;
            Category = new Category() {Id = CategoryId};
            SubCategoryId = subCategoryId;
            SubCategory = new SubCategory() {Id = SubCategoryId};
            Description = description;
            Amount = amount;
            TransactionDate = transactionDate;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Budget
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime EffectiveFrom { get; set; }
        public virtual DateTime? EffectiveTo { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Category? Category { get; set; }

        public Budget() { }
        public Budget(Guid id, Guid userId, Guid categoryId, decimal amount, DateTime effectiveFrom, DateTime? effectiveTo, bool isActive, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            UserId = userId;
            CategoryId = categoryId;
            Amount = amount;
            EffectiveFrom = effectiveFrom;
            EffectiveTo = effectiveTo;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    // Refresh token entity for rotation and revocation handling
    public class RefreshToken
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string TokenHash { get; set; } = null!;
        public virtual string? DeviceInfo { get; set; }
        public virtual DateTimeOffset ExpiresAt { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset? RevokedAt { get; set; }

        public RefreshToken() { }
        public RefreshToken(Guid id, Guid userId, string tokenHash, string? deviceInfo, DateTimeOffset expiresAt, DateTimeOffset createdAt, DateTimeOffset? revokedAt)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash;
            DeviceInfo = deviceInfo;
            ExpiresAt = expiresAt;
            CreatedAt = createdAt;
            RevokedAt = revokedAt;
        }
    }

    // Password reset token entity
    public class PasswordResetToken
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string TokenHash { get; set; } = null!;
        public virtual DateTimeOffset ExpiresAt { get; set; }
        public virtual bool Used { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }

        public PasswordResetToken() { }
        public PasswordResetToken(Guid id, Guid userId, string tokenHash, DateTimeOffset expiresAt, bool used, DateTimeOffset createdAt)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            Used = used;
            CreatedAt = createdAt;
        }
    }
}
