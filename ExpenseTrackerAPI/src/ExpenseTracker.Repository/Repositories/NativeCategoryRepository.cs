using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeCategoryRepository : ICategoryRepository
    {
        private readonly ISessionFactory _sf;
        public NativeCategoryRepository(ISessionFactory sf) { _sf = sf; }

        public async Task CreateAsync(Category category)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.SaveAsync(category);
            await tx.CommitAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<Category>(id);
        }

        public async Task<Category?> GetByNameAndUserAsync(Guid userId, string name, string type)
        {
            using var s = _sf.OpenSession();
            if (!Enum.TryParse<CategoryType>(type, true, out var parsedType))
            {
                return null;
            }
            return await s.Query<Category>().FirstOrDefaultAsync(c => c.UserId == userId && c.Type == parsedType && c.Name.ToLower() == name.ToLower());
        }

        public async Task UpdateAsync(Category category)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.UpdateAsync(category);
            await tx.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            var c = await s.GetAsync<Category>(id);
            if (c != null) await s.DeleteAsync(c);
            await tx.CommitAsync();
        }

        public async Task<IList<Category>> ListByUserAsync(Guid userId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<Category>().Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task CreateSubAsync(SubCategory subCategory)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.SaveAsync(subCategory);
            await tx.CommitAsync();
        }

        public async Task<SubCategory?> GetSubByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<SubCategory>(id);
        }

        public async Task<SubCategory?> GetSubByNameAsync(Guid categoryId, string name)
        {
            using var s = _sf.OpenSession();
            return await s.Query<SubCategory>().FirstOrDefaultAsync(sc => sc.CategoryId == categoryId && sc.Name.ToLower() == name.ToLower());
        }

        public async Task UpdateSubAsync(SubCategory subCategory)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            await s.UpdateAsync(subCategory);
            await tx.CommitAsync();
        }

        public async Task DeleteSubAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            var sc = await s.GetAsync<SubCategory>(id);
            if (sc != null) await s.DeleteAsync(sc);
            await tx.CommitAsync();
        }

        public async Task<IList<SubCategory>> ListSubsByCategoryAsync(Guid categoryId)
        {
            using var s = _sf.OpenSession();
            return await s.Query<SubCategory>().Where(sc => sc.CategoryId == categoryId).ToListAsync();
        }
    }
}
