
using Orders.Shared.Entities;

namespace Orders.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context=context;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.AddRange(new List<Category>
                {
                    new Category { Name = "Bebidas" },
                    new Category { Name = "Abarrotes" },
                    new Category { Name = "Lácteos" },
                    new Category { Name = "Carnes" },
                    new Category { Name = "Verduras" }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.AddRange(new List<Country>
                {
                    new Country { Name = "Colombia" },
                    new Country { Name = "Perú" },
                    new Country { Name = "Ecuador" },
                    new Country { Name = "Chile" },
                    new Country { Name = "Argentina" }
                });
                await _context.SaveChangesAsync();
            }
        }
    }
}
