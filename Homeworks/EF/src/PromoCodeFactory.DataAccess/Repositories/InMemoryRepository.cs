using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = (List<T>)data;
        }

        public Task<List<T>> GetAllAsync()
        {
            return Task.FromResult<List<T>>(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task CreateNewRecordAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRecordAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRecordAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}