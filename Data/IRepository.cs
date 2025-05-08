using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Data
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}