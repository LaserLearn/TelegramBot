using System.Linq.Expressions;

namespace TelegramBot.contract.Database.generic
{
    public interface IGenericRepository<T> where T : class
    {
        // متدهای CRUD پایه برای دیتابیس پیش‌فرض
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteByIdAsync(string id);
        Task DeleteByIdsAsync(IEnumerable<string> ids);

        // متدهای اورلود شده برای دیتابیس‌های دیگر
        Task<T> GetByIdAsync(string id, string databaseName);
        Task<IEnumerable<T>> GetAllAsync(
            string databaseName,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<bool> ExistsAsync(string databaseName, Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity, string databaseName);
        Task AddRangeAsync(IEnumerable<T> entities, string databaseName);
        Task UpdateAsync(T entity, string databaseName);
        Task DeleteByIdAsync(string id, string databaseName);
        Task DeleteByIdsAsync(IEnumerable<string> ids, string databaseName);
    }

}
