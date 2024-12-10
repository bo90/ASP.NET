using System;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<CustomerPreference> CustomerPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
         optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //PromoCode имеет ссылку на Preference 
        modelBuilder.Entity<PromoCode>()
            .HasOne<Preference>(x => x.Preference)
            .WithMany(p => p.PromoCodes);

        //Employee имеет ссылку на Role
        modelBuilder.Entity<Employee>()
            .HasOne<Role>(p => p.Role)
            .WithMany(q => q.Employees);
        
        modelBuilder.Entity<CustomerPreference>()
            .HasKey(cp => new { cp.CustomerId, cp.PreferenceId });
        modelBuilder.Entity<CustomerPreference>()
            .HasOne<Customer>(cp => cp.Customer)
            .WithMany(c => c.CustomerPreferences);

        modelBuilder.Entity<CustomerPreference>()
            .HasOne<Preference>(cp => cp.Preference)
            .WithMany(p => p.CustomerPreferences);


        //Связь Customer и Promocode реализовать через One-To-Many, промокод может быть выдан только одному клиенту.
        modelBuilder.Entity<PromoCode>()
            .HasOne<Customer>( p => p.Customer)
            .WithMany(c => c.Promocodes);
        
        modelBuilder.Entity<Employee>().Property(c => c.FirstName).HasMaxLength(150);
        modelBuilder.Entity<Employee>().Property(c => c.LastName).HasMaxLength(200);            
        modelBuilder.Entity<Employee>().Property(c => c.Email).HasMaxLength(250);

        modelBuilder.Entity<Role>().Property(c => c.Name).HasMaxLength(150);
        modelBuilder.Entity<Role>().Property(c => c.Description).HasMaxLength(300);

        modelBuilder.Entity<Customer>().Property(c => c.FirstName).HasMaxLength(150);
        modelBuilder.Entity<Customer>().Property(c => c.LastName).HasMaxLength(200);
        modelBuilder.Entity<Customer>().Property(c => c.Email).HasMaxLength(250);

        modelBuilder.Entity<Preference>().Property(c => c.Name).HasMaxLength(150);

        modelBuilder.Entity<PromoCode>().Property(c => c.Code).HasMaxLength(100);
        modelBuilder.Entity<PromoCode>().Property(c => c.ServiceInfo).HasMaxLength(300);
        modelBuilder.Entity<PromoCode>().Property(c => c.PartnerName).HasMaxLength(300);
    }
}