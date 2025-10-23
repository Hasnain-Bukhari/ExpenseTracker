using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class CategoryTypeService
    {
        private readonly ICategoryTypeRepository _repo;

        public CategoryTypeService(ICategoryTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<CategoryType?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<IList<CategoryType>> ListAsync()
        {
            return await _repo.ListAsync();
        }

        public async Task<IList<CategoryType>> ListActiveAsync()
        {
            return await _repo.ListActiveAsync();
        }

        public async Task<CategoryType> CreateAsync(CategoryType categoryType)
        {
            // Validate category type exists
            if (string.IsNullOrWhiteSpace(categoryType.Name))
                throw new InvalidOperationException("Category type name is required");

            categoryType.Id = Guid.NewGuid();
            categoryType.CreatedAt = DateTime.UtcNow;
            categoryType.UpdatedAt = DateTime.UtcNow;
            await _repo.CreateAsync(categoryType);
            return categoryType;
        }

        public async Task UpdateAsync(CategoryType categoryType)
        {
            var existing = await _repo.GetByIdAsync(categoryType.Id);
            if (existing == null) 
                throw new InvalidOperationException("Category type not found");

            if (string.IsNullOrWhiteSpace(categoryType.Name))
                throw new InvalidOperationException("Category type name is required");

            existing.Name = categoryType.Name;
            existing.Description = categoryType.Description;
            existing.Color = categoryType.Color;
            existing.IsActive = categoryType.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) 
                throw new InvalidOperationException("Category type not found");

            await _repo.DeleteAsync(id);
        }
    }
}
