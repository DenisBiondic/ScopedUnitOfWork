using Microsoft.EntityFrameworkCore;
using ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain;

namespace ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.Infrastructure
{
    public class SampleContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost,1433;Database=Chinook;User=sa;Trusted_Connection=False;Password=<YourStrong!Passw0rd>");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .Property(x => x.Name).IsRequired();
        }
    }
}