using InterComCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterComInfrastructure.Data
{
    public class InterComDbContext : IdentityDbContext<IdentityUser>
    {
        public InterComDbContext(DbContextOptions<InterComDbContext> options)
            : base(options)
        {
        }

        public DbSet<Template> Templates { get; set; }
        public DbSet<Placeholder> Placeholders { get; set; }
        public DbSet<Contract> Contracts { get; set; }

        // Надалі тут можна налаштувати Fluent API для зв’язків Contract–PlaceholderValue тощо
    }
}
