using Microsoft.EntityFrameworkCore;
using urunsatis.Models;

namespace urunsatis.Utility
{
    public class UygulamaDbContext: DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options): base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
