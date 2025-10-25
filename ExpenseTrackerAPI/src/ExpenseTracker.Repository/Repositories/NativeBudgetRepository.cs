using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;
using ExpenseTracker.Dtos.Budgets;
using ExpenseTracker.Dtos.Categories;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeBudgetRepository : IBudgetRepository
    {
        private readonly ISessionFactory _sf;

        public NativeBudgetRepository(ISessionFactory sf)
        {
            _sf = sf;
        }

        public async Task CreateAsync(Budget budget)
        {
            using var s = _sf.OpenSession();
            await s.SaveAsync(budget);
            await s.FlushAsync();
        }

        public async Task<Budget?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<Budget>(id);
        }

        public async Task<Budget?> GetActiveBudgetAsync(Guid userId, Guid categoryId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Budget>()
                .Where(b => b.UserId == userId && b.CategoryId == categoryId && b.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Budget>> GetActiveBudgetsByUserAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Budget>()
                .Where(b => b.UserId == userId && b.IsActive)
                .OrderBy(b => b.Category.Name)
                .ToListAsync();
        }

        public async Task<List<Budget>> GetBudgetsByUserAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Budget>()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.EffectiveFrom)
                .ThenBy(b => b.Category.Name)
                .ToListAsync();
        }

        public async Task<List<Budget>> GetBudgetsByCategoryAsync(Guid userId, Guid categoryId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Budget>()
                .Where(b => b.UserId == userId && b.CategoryId == categoryId)
                .OrderByDescending(b => b.EffectiveFrom)
                .ToListAsync();
        }

        public async Task UpdateAsync(Budget budget)
        {
            using var s = _sf.OpenSession();
            await s.UpdateAsync(budget);
            await s.FlushAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            var budget = await s.GetAsync<Budget>(id);
            if (budget != null)
            {
                await s.DeleteAsync(budget);
                await s.FlushAsync();
            }
        }

        public async Task<bool> HasActiveBudgetAsync(Guid userId, Guid categoryId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Budget>()
                .AnyAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.IsActive);
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto(
                category.Id,
                category.UserId,
                category.Name,
                category.CategoryType,
                category.Description,
                category.CreatedAt,
                category.UpdatedAt,
                null
            );
        }
    }
}
