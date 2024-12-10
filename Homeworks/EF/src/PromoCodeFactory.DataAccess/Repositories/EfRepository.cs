using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    private DatabaseContext _ctx;
    private readonly DbSet<T> _entities;

    public EfRepository(DatabaseContext ctx)
    {
        _ctx = ctx;
        _entities = _ctx.Set<T>();
    }
    
    public async Task<List<T>> GetAllAsync()
    {
        var allData = await _entities.ToListAsync();
        return allData;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _ctx.Set<T>().FirstOrDefaultAsync(x=>x.Id==id);
    }

    public async Task CreateNewRecordAsync(T entity)
    {
        await _ctx.Set<T>().AddAsync(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateRecordAsync(T entity)
    {
       await _ctx.SaveChangesAsync();
    }

    public async Task DeleteRecordAsync(T entity)
    {
        _ctx.Set<T>().Remove(entity);
        await _ctx.SaveChangesAsync();
    }
}