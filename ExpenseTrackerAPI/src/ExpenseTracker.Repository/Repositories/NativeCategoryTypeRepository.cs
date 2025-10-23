using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Dtos.Models;
using NHibernate;
using NHibernate.Linq;

namespace ExpenseTracker.Repository.Repositories
{
    public class NativeCategoryTypeRepository : ICategoryTypeRepository
    {
        private readonly ISessionFactory _sf;

        public NativeCategoryTypeRepository(ISessionFactory sf)
        {
            _sf = sf;
        }

        public async Task<CategoryType?> GetByIdAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            return await s.GetAsync<CategoryType>(id);
        }

        public async Task<CategoryType?> GetByNameAsync(string name)
        {
            using var s = _sf.OpenSession();
            return await s.Query<CategoryType>()
                .FirstOrDefaultAsync(ct => ct.Name.ToLower() == name.ToLower());
        }

        public async Task<IList<CategoryType>> ListAsync()
        {
            using var s = _sf.OpenSession();
            return await s.Query<CategoryType>()
                .OrderBy(ct => ct.Name)
                .ToListAsync();
        }

        public async Task<IList<CategoryType>> ListActiveAsync()
        {
            using var s = _sf.OpenSession();
            return await s.Query<CategoryType>()
                .Where(ct => ct.IsActive)
                .OrderBy(ct => ct.Name)
                .ToListAsync();
        }

        public async Task CreateAsync(CategoryType categoryType)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                await s.SaveAsync(categoryType);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(CategoryType categoryType)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                await s.UpdateAsync(categoryType);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using var s = _sf.OpenSession();
            using var tx = s.BeginTransaction();
            try
            {
                var categoryType = await s.GetAsync<CategoryType>(id);
                if (categoryType != null)
                {
                    await s.DeleteAsync(categoryType);
                }
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
