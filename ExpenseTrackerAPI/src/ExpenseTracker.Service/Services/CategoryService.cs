using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Categories;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Repository.Repositories;

namespace ExpenseTracker.Service.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ICategoryTypeRepository _categoryTypeRepo;

        public CategoryService(ICategoryRepository repo, ICategoryTypeRepository categoryTypeRepo) 
        { 
            _repo = repo;
            _categoryTypeRepo = categoryTypeRepo;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, Guid userId)
        {
            // Validate category type exists
            var categoryType = await _categoryTypeRepo.GetByIdAsync(dto.CategoryTypeId);
            if (categoryType == null) throw new InvalidOperationException("Category type not found");

            // check duplicate
            var existing = await _repo.GetByNameAndUserAsync(userId, dto.Name);
            if (existing != null) throw new InvalidOperationException("Category already exists");

            var now = DateTime.UtcNow;
            var category = new Category(Guid.NewGuid(), userId, dto.Name, dto.Description ?? "", null, dto.CategoryTypeId, now, now);
            await _repo.CreateAsync(category);
            
            var categoryTypeDto = new ExpenseTracker.Dtos.CategoryTypes.CategoryTypeDto(
                categoryType.Id, categoryType.Name, categoryType.Description, 
                categoryType.Color, categoryType.IsActive, categoryType.CreatedAt, categoryType.UpdatedAt
            );
            
            return new CategoryDto(category.Id, category.UserId, category.Name, category.CategoryTypeId, category.Description, category.CreatedAt, category.UpdatedAt, categoryTypeDto, null);
        }

        public async Task<IList<CategoryDto>> ListByUserAsync(Guid userId)
        {
            var items = await _repo.ListByUserAsync(userId);
            var list = new List<CategoryDto>();
            foreach (var c in items)
            {
                var subs = await _repo.ListSubsByCategoryAsync(c.Id);
                var subDtos = new List<SubCategoryDto>();
                foreach (var s in subs)
                {
                    subDtos.Add(new SubCategoryDto(s.Id, s.CategoryId, s.Name, s.Description, s.CreatedAt, s.UpdatedAt));
                }
                
                var categoryTypeDto = new ExpenseTracker.Dtos.CategoryTypes.CategoryTypeDto(
                    c.CategoryType!.Id, c.CategoryType.Name, c.CategoryType.Description, 
                    c.CategoryType.Color, c.CategoryType.IsActive, c.CategoryType.CreatedAt, c.CategoryType.UpdatedAt
                );
                
                list.Add(new CategoryDto(c.Id, c.UserId, c.Name, c.CategoryTypeId, c.Description, c.CreatedAt, c.UpdatedAt, categoryTypeDto, subDtos));
            }
            return list;
        }

        public async Task<CategoryDto?> GetCategoryAsync(Guid id, Guid userId, bool includeSubs = false)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;
            if (c.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");
            
            List<SubCategoryDto>? subs = null;
            if (includeSubs)
            {
                var sItems = await _repo.ListSubsByCategoryAsync(c.Id);
                subs = new List<SubCategoryDto>();
                foreach (var s in sItems) subs.Add(new SubCategoryDto(s.Id, s.CategoryId, s.Name, s.Description, s.CreatedAt, s.UpdatedAt));
            }
            
            var categoryTypeDto = new ExpenseTracker.Dtos.CategoryTypes.CategoryTypeDto(
                c.CategoryType!.Id, c.CategoryType.Name, c.CategoryType.Description, 
                c.CategoryType.Color, c.CategoryType.IsActive, c.CategoryType.CreatedAt, c.CategoryType.UpdatedAt
            );
            
            return new CategoryDto(c.Id, c.UserId, c.Name, c.CategoryTypeId, c.Description, c.CreatedAt, c.UpdatedAt, categoryTypeDto, subs);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, Guid userId)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) throw new KeyNotFoundException("Category not found");
            if (c.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");

            // Validate category type exists
            var categoryType = await _categoryTypeRepo.GetByIdAsync(dto.CategoryTypeId);
            if (categoryType == null) throw new InvalidOperationException("Category type not found");

            // check duplicate name
            var dup = await _repo.GetByNameAndUserAsync(userId, dto.Name);
            if (dup != null && dup.Id != id) throw new InvalidOperationException("Category name already in use");

            c.Name = dto.Name;
            c.CategoryTypeId = dto.CategoryTypeId;
            c.Description = dto.Description ?? "";
            c.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(c);
            
            var categoryTypeDto = new ExpenseTracker.Dtos.CategoryTypes.CategoryTypeDto(
                categoryType.Id, categoryType.Name, categoryType.Description, 
                categoryType.Color, categoryType.IsActive, categoryType.CreatedAt, categoryType.UpdatedAt
            );
            
            return new CategoryDto(c.Id, c.UserId, c.Name, c.CategoryTypeId, c.Description, c.CreatedAt, c.UpdatedAt, categoryTypeDto, null);
        }

        public async Task DeleteCategoryAsync(Guid id, Guid userId)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) throw new KeyNotFoundException("Category not found");
            if (c.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");

            await _repo.DeleteAsync(id);
        }

        public async Task<SubCategoryDto> CreateSubCategoryAsync(Guid categoryId, CreateSubCategoryDto dto, Guid userId)
        {
            var cat = await _repo.GetByIdAsync(categoryId);
            if (cat == null) throw new KeyNotFoundException("Category not found");
            if (cat.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");

            var existing = await _repo.GetSubByNameAsync(categoryId, dto.Name);
            if (existing != null) throw new InvalidOperationException("Subcategory already exists");

            var now = DateTime.UtcNow;
            var sc = new SubCategory(Guid.NewGuid(), categoryId, dto.Name, dto.Description, now, now);
            await _repo.CreateSubAsync(sc);
            return new SubCategoryDto(sc.Id, sc.CategoryId, sc.Name, sc.Description, sc.CreatedAt, sc.UpdatedAt);
        }

        public async Task<IList<SubCategoryDto>> ListSubCategoriesAsync(Guid categoryId, Guid userId)
        {
            var cat = await _repo.GetByIdAsync(categoryId);
            if (cat == null) throw new KeyNotFoundException("Category not found");
            if (cat.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");

            var subs = await _repo.ListSubsByCategoryAsync(categoryId);
            var list = new List<SubCategoryDto>();
            foreach (var s in subs) list.Add(new SubCategoryDto(s.Id, s.CategoryId, s.Name, s.Description, s.CreatedAt, s.UpdatedAt));
            return list;
        }

        public async Task<SubCategoryDto> UpdateSubCategoryAsync(Guid id, UpdateSubCategoryDto dto, Guid userId)
        {
            var sc = await _repo.GetSubByIdAsync(id);
            if (sc == null) throw new KeyNotFoundException("Subcategory not found");
            var cat = await _repo.GetByIdAsync(sc.CategoryId);
            if (cat == null) throw new KeyNotFoundException("Category not found");
            if (cat.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");

            // duplicate check
            var dup = await _repo.GetSubByNameAsync(sc.CategoryId, dto.Name);
            if (dup != null && dup.Id != id) throw new InvalidOperationException("Subcategory name already in use");

            sc.Name = dto.Name;
            sc.Description = dto.Description;
            sc.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateSubAsync(sc);
            return new SubCategoryDto(sc.Id, sc.CategoryId, sc.Name, sc.Description, sc.CreatedAt, sc.UpdatedAt);
        }

        public async Task DeleteSubCategoryAsync(Guid id, Guid userId)
        {
            var sc = await _repo.GetSubByIdAsync(id);
            if (sc == null) throw new KeyNotFoundException("Subcategory not found");
            var cat = await _repo.GetByIdAsync(sc.CategoryId);
            if (cat == null) throw new KeyNotFoundException("Category not found");
            if (cat.UserId != userId) throw new UnauthorizedAccessException("Not owner of category");

            await _repo.DeleteSubAsync(id);
        }
    }
}
