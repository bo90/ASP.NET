using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task CreateNewEmployeeAsync(T emp)
        {
            var newRec = Data.Append(emp);
            Data = newRec;
            return Task.FromResult(Data);
        }

        public Task UpdateEmployeeAsync(T emp)
        {
            Data = Data.Where(x=> x.Id != emp.Id).Append(emp);
            return Task.CompletedTask;
        }

        public Task DeleteEmployeeAsync(Guid id)
        {
            Data = Data.Where(x => x.Id != id);
            return Task.CompletedTask;
        }
    }
}