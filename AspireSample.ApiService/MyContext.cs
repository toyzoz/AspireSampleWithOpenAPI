using Microsoft.EntityFrameworkCore;

namespace AspireSample.ApiService
{
    public class MyContext(DbContextOptions<MyContext> options)
        : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var products = Enumerable.Range(1, 100).Select(x => new Product()
            {

                Id = x,
                Name = $"product-{x}"
            });

            modelBuilder.Entity<Product>().HasData(products);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
