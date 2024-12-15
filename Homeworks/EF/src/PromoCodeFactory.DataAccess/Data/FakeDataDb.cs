using PromoCodeFactory.Core.Abstractions.Repositories;

namespace PromoCodeFactory.DataAccess.Data;

public class FakeDataDb : IFakeDataDb
{
    private readonly DatabaseContext _ctx;

    public FakeDataDb(DatabaseContext ctx)
    {
        _ctx = ctx;
    }
    
    public void SeedDatabase()
    {
        //чистим и создаем
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();
        //Наполняем данными
        _ctx.AddRange(FakeDataFactory.Employees);
        _ctx.AddRange(FakeDataFactory.Customers);
        _ctx.AddRange(FakeDataFactory.Preferences);
        _ctx.SaveChanges();
    }
}