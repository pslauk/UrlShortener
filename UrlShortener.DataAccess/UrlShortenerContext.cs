using Microsoft.EntityFrameworkCore;
using UrlShortener.DomainModel.Entities;

namespace UrlShortener.DataAccess
{
    public class UrlShortenerContext : DbContext
    {
        public DbSet<Url> Urls { get; set; }

        public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
