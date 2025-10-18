using System;

namespace ExpenseTracker.Dtos.Models
{
    public enum AccountType { Cash, Bank, CreditCard, Savings, Other }
    public enum CategoryType { Expense, Income }
    public enum TransactionType { Expense, Income, Transfer, Saving }
    public enum TransactionStatus { Pending, Cleared, Reconciled }
    public enum BudgetPeriod { Monthly, Weekly, Yearly, Custom }

    // Authentication-related enum
    public enum AuthProvider { Local, Google, Facebook, Mixed }

    // User entity - converted from record to class for NHibernate proxy compatibility
    public class User
    {
        public virtual Guid Id { get; set; }
        public virtual string Email { get; set; } = null!;
        public virtual string NormalizedEmail { get; set; } = null!;
        public virtual string? PasswordHash { get; set; }
        public virtual string? FullName { get; set; }
        public virtual string DefaultCurrency { get; set; } = null!;
        public virtual string? Locale { get; set; }
        public virtual string? Timezone { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsEmailVerified { get; set; }
        public virtual string? Phone { get; set; }
        public virtual AuthProvider Provider { get; set; }
        public virtual string? ProviderId { get; set; }
        public virtual DateTime? LastLoginAt { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }

        public User() { }

        public User(Guid id, string email, string normalizedEmail, string? passwordHash, string? fullName, string defaultCurrency, string? locale, string? timezone, bool isActive, bool isEmailVerified, string? phone, AuthProvider provider, string? providerId, DateTime? lastLoginAt, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Email = email;
            NormalizedEmail = normalizedEmail;
            PasswordHash = passwordHash;
            FullName = fullName;
            DefaultCurrency = defaultCurrency;
            Locale = locale;
            Timezone = timezone;
            IsActive = isActive;
            IsEmailVerified = isEmailVerified;
            Phone = phone;
            Provider = provider;
            ProviderId = providerId;
            LastLoginAt = lastLoginAt;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Currency
    {
        public virtual string Code { get; set; } = null!;
        public virtual string? Symbol { get; set; }
        public virtual string? Name { get; set; }

        public Currency() { }
        public Currency(string code, string? symbol, string? name)
        {
            Code = code;
            Symbol = symbol;
            Name = name;
        }
    }

    public class Account
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string Name { get; set; } = null!;
        public virtual AccountType Type { get; set; }
        public virtual string Currency { get; set; } = null!;
        public virtual bool IsSavings { get; set; }
        public virtual decimal OpeningBalance { get; set; }
        public virtual bool IncludeInNetworth { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset UpdatedAt { get; set; }

        public Account() { }
        public Account(Guid id, Guid userId, string name, AccountType type, string currency, bool isSavings, decimal openingBalance, bool includeInNetworth, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Type = type;
            Currency = currency;
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
        public virtual Guid? ParentId { get; set; }
        public virtual CategoryType Type { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset UpdatedAt { get; set; }

        public Category() { }
        public Category(Guid id, Guid userId, string name, Guid? parentId, CategoryType type, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            ParentId = parentId;
            Type = type;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Goal
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string Name { get; set; } = null!;
        public virtual decimal TargetAmount { get; set; }
        public virtual string Currency { get; set; } = null!;
        public virtual Guid? AccountId { get; set; }
        public virtual DateTime? Deadline { get; set; }
        public virtual string? Note { get; set; }
        public virtual bool Archived { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset UpdatedAt { get; set; }

        public Goal() { }
        public Goal(Guid id, Guid userId, string name, decimal targetAmount, string currency, Guid? accountId, DateTime? deadline, string? note, bool archived, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            Id = id;
            UserId = userId;
            Name = name;
            TargetAmount = targetAmount;
            Currency = currency;
            AccountId = accountId;
            Deadline = deadline;
            Note = note;
            Archived = archived;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Transaction
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual Guid AccountId { get; set; }
        public virtual TransactionType Type { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Currency { get; set; } = null!;
        public virtual decimal? OriginalAmount { get; set; }
        public virtual string? OriginalCurrency { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual DateTimeOffset? SettledAt { get; set; }
        public virtual Guid? CategoryId { get; set; }
        public virtual Guid? GoalId { get; set; }
        public virtual string? Notes { get; set; }
        public virtual TransactionStatus Status { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset UpdatedAt { get; set; }

        public Transaction() { }
        public Transaction(Guid id, Guid userId, Guid accountId, TransactionType type, decimal amount, string currency, decimal? originalAmount, string? originalCurrency, DateTime date, DateTimeOffset? settledAt, Guid? categoryId, Guid? goalId, string? notes, TransactionStatus status, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            Id = id;
            UserId = userId;
            AccountId = accountId;
            Type = type;
            Amount = amount;
            Currency = currency;
            OriginalAmount = originalAmount;
            OriginalCurrency = originalCurrency;
            Date = date;
            SettledAt = settledAt;
            CategoryId = categoryId;
            GoalId = goalId;
            Notes = notes;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }

    public class Budget
    {
        public virtual Guid Id { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual Guid CategoryId { get; set; }
        public virtual BudgetPeriod Period { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual DateTimeOffset CreatedAt { get; set; }
        public virtual DateTimeOffset UpdatedAt { get; set; }

        public Budget() { }
        public Budget(Guid id, Guid userId, Guid categoryId, BudgetPeriod period, decimal amount, DateTime startDate, DateTime? endDate, DateTimeOffset createdAt, DateTimeOffset updatedAt)
        {
            Id = id;
            UserId = userId;
            CategoryId = categoryId;
            Period = period;
            Amount = amount;
            StartDate = startDate;
            EndDate = endDate;
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
