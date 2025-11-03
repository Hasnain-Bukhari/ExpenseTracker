using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Accounts;
using ExpenseTracker.Dtos.AccountTypes;
using ExpenseTracker.Dtos.Categories;
using ExpenseTracker.Dtos.Currencies;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Transactions;
using ExpenseTracker.Dtos.Common;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IAccountRepository _accountRepo;
        private readonly ICategoryRepository _categoryRepo;

        public TransactionService(ITransactionRepository repo, IAccountRepository accountRepo, ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto, Guid userId)
        {
            // Validate account exists and belongs to user
            var account = await _accountRepo.GetByIdAsync(dto.AccountId);
            if (account == null || account.UserId != userId)
                throw new InvalidOperationException("Account not found");

            // Validate category exists
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found");

            // Validate subcategory if provided
            if (dto.SubCategoryId.HasValue)
            {
                var subCategory = await _categoryRepo.GetSubByIdAsync(dto.SubCategoryId.Value);
                if (subCategory == null || subCategory.CategoryId != dto.CategoryId)
                    throw new InvalidOperationException("Subcategory not found or doesn't belong to the selected category");
            }

            // Validate amount is positive
            if (dto.Amount <= 0)
                throw new InvalidOperationException("Amount must be positive");

            var now = DateTime.UtcNow;
            var transaction = new Transaction(
                Guid.NewGuid(),
                userId,
                dto.AccountId,
                dto.CategoryId,
                dto.SubCategoryId,
                dto.Description,
                dto.Amount,
                dto.TransactionDate,
                now,
                now
            );

            await _repo.CreateAsync(transaction);

            // Use the same approach as GetByIdAsync method
            var createdTransaction = await _repo.GetByIdAsync(transaction.Id);
            if (createdTransaction == null)
                throw new InvalidOperationException("Failed to retrieve created transaction");

            // Load navigation properties exactly like GetByIdAsync method
            createdTransaction.Account = await _accountRepo.GetByIdAsync(createdTransaction.AccountId);
            createdTransaction.Category = await _categoryRepo.GetByIdAsync(createdTransaction.CategoryId);
            if (createdTransaction.SubCategoryId.HasValue)
            {
                createdTransaction.SubCategory = await _categoryRepo.GetSubByIdAsync(createdTransaction.SubCategoryId.Value);
            }

            try
            {
                return MapToDto(createdTransaction);
            }
            catch (Exception ex)
            {
                // Fallback to simple DTO if MapToDto fails
                return new TransactionDto(
                    createdTransaction.Id,
                    createdTransaction.UserId,
                    createdTransaction.AccountId,
                    createdTransaction.CategoryId,
                    createdTransaction.SubCategoryId,
                    createdTransaction.Description ?? string.Empty,
                    createdTransaction.Amount,
                    createdTransaction.TransactionDate,
                    createdTransaction.CreatedAt,
                    createdTransaction.UpdatedAt,
                    null, // Account
                    null, // Category  
                    null  // SubCategory
                );
            }
        }

        public async Task<IList<TransactionDto>> ListByUserAsync(Guid userId)
        {
            var transactions = await _repo.ListByUserAsync(userId);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<TransactionDto>> ListByUserWithFiltersAsync(Guid userId, Guid? accountId, Guid? categoryId, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 50)
        {
            var transactions = await _repo.ListByUserWithFiltersAsync(userId, accountId, categoryId, startDate, endDate, page, pageSize);
            var total = await _repo.CountByUserWithFiltersAsync(userId, accountId, categoryId, startDate, endDate);

            var transactionDtos = transactions.Select(MapToDto).ToList();

            return new PagedResult<TransactionDto>(transactionDtos, page, pageSize, total);
        }

        public async Task<TransactionDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var transaction = await _repo.GetByIdAsync(id);
            if (transaction == null || transaction.UserId != userId)
                return null;

            // Load navigation properties
            transaction.Account = await _accountRepo.GetByIdAsync(transaction.AccountId);
            transaction.Category = await _categoryRepo.GetByIdAsync(transaction.CategoryId);
            if (transaction.SubCategoryId.HasValue)
            {
                transaction.SubCategory = await _categoryRepo.GetSubByIdAsync(transaction.SubCategoryId.Value);
            }

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
                throw new KeyNotFoundException("Transaction not found");

            // Validate account exists and belongs to user
            var account = await _accountRepo.GetByIdAsync(dto.AccountId);
            if (account == null || account.UserId != userId)
                throw new InvalidOperationException("Account not found");

            // Validate category exists
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found");

            // Validate subcategory if provided
            if (dto.SubCategoryId.HasValue)
            {
                var subCategory = await _categoryRepo.GetSubByIdAsync(dto.SubCategoryId.Value);
                if (subCategory == null || subCategory.CategoryId != dto.CategoryId)
                    throw new InvalidOperationException("Subcategory not found or doesn't belong to the selected category");
            }

            // Validate amount is positive
            if (dto.Amount <= 0)
                throw new InvalidOperationException("Amount must be positive");

            existing.AccountId = dto.AccountId;
            existing.CategoryId = dto.CategoryId;
            existing.SubCategoryId = dto.SubCategoryId;
            existing.Description = dto.Description;
            existing.Amount = dto.Amount;
            existing.TransactionDate = dto.TransactionDate;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);

            // Load navigation properties for response
            existing.Account = account;
            existing.Category = category;
            if (dto.SubCategoryId.HasValue)
            {
                existing.SubCategory = await _categoryRepo.GetSubByIdAsync(dto.SubCategoryId.Value);
            }
            else
            {
                existing.SubCategory = null;
            }

            return MapToDto(existing);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
                throw new KeyNotFoundException("Transaction not found");

            await _repo.DeleteAsync(id);
        }

        private TransactionDto MapToDto(Transaction transaction)
        {
            var accountDto = new AccountDto(
                transaction.Account.Id,
                transaction.Account.UserId,
                transaction.Account.Name,
                transaction.Account.AccountTypeId,
                transaction.Account.CurrencyId,
                transaction.Account.IsSavings,
                transaction.Account.OpeningBalance,
                transaction.Account.IncludeInNetworth,
                transaction.Account.CreatedAt,
                transaction.Account.UpdatedAt,
                new AccountTypeDto(
                    transaction.Account.AccountType.Id,
                    transaction.Account.AccountType.Name,
                    transaction.Account.AccountType.IsCard,
                    transaction.Account.AccountType.CreatedAt,
                    transaction.Account.AccountType.UpdatedAt
                ),
                new CurrencyDto(
                    transaction.Account.Currency.Id,
                    transaction.Account.Currency.UserId,
                    transaction.Account.Currency.Code,
                    transaction.Account.Currency.Symbol,
                    transaction.Account.Currency.Name,
                    transaction.Account.Currency.CreatedAt,
                    transaction.Account.Currency.UpdatedAt
                )
            );

            var categoryDto = new CategoryDto(
                transaction.Category.Id,
                transaction.Category.UserId,
                transaction.Category.Name,
                transaction.Category.CategoryType,
                transaction.Category.Description,
                transaction.Category.CreatedAt,
                transaction.Category.UpdatedAt,
                null
            );

            SubCategoryDto? subCategoryDto = null;
            if (transaction.SubCategory != null)
            {
                subCategoryDto = new SubCategoryDto(
                    transaction.SubCategory.Id,
                    transaction.SubCategory.CategoryId,
                    transaction.SubCategory.Name,
                    transaction.SubCategory.Description,
                    transaction.SubCategory.CreatedAt,
                    transaction.SubCategory.UpdatedAt
                );
            }

            return new TransactionDto(
                transaction.Id,
                transaction.UserId,
                transaction.AccountId,
                transaction.CategoryId,
                transaction.SubCategoryId,
                transaction.Description ?? string.Empty,
                transaction.Amount,
                transaction.TransactionDate,
                transaction.CreatedAt,
                transaction.UpdatedAt,
                accountDto,
                categoryDto,
                subCategoryDto
            );
        }
    }
}
