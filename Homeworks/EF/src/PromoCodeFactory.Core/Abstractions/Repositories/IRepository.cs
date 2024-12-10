using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);
        
        Task CreateNewRecordAsync(T entity);
        Task UpdateRecordAsync(T entity);
        Task DeleteRecordAsync(T entity);
    }
}