using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InterComInfrastructure.Data
{
    public class InterComDbContextFactory
        : IDesignTimeDbContextFactory<InterComDbContext>
    {
        public InterComDbContext CreateDbContext(string[] args)
        {
            // Завантажуємо appsettings.json з кореня проєкту (InterCom)
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../InterCom"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<InterComDbContext>();
            var connString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connString);

            return new InterComDbContext(optionsBuilder.Options);
        }
    }
}
