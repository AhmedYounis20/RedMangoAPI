using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RedMangoAPI;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    Id = 1,
                    Name = "Plate 1",
                    Description = "Nice one you can try",
                    Image = "https://asshole.blob.core.windows.net/plates/plate1.jpeg",
                    Price = 3.99,
                    Category = "Dessert",
                    SpecialTag = "Top Rated"
                },
            new MenuItem
            {
                Id = 2,
                Name = "Plate 2",
                Description = "Delicious and tempting",
                Image = "https://asshole.blob.core.windows.net/plates/plate2.jpeg",
                Price = 5.99,
                Category = "Appetizer",
                SpecialTag = "Chef's Recommendation"
            },
            new MenuItem
            {
                Id = 3,
                Name = "Plate 3",
                Description = "A savory delight for your taste buds",
                Image = "https://asshole.blob.core.windows.net/plates/plate3.jpeg",
                Price = 7.99,
                Category = "Main Course",
                SpecialTag = "Spicy Special"
            },
            new MenuItem
            {
                Id = 4,
                Name = "Plate 4",
                Description = "Irresistible flavor combination",
                Image = "https://asshole.blob.core.windows.net/plates/plate4.jpeg",
                Price = 9.99,
                Category = "Seafood",
                SpecialTag = "Fresh Catch"
            },
            new MenuItem
            {
                Id = 5,
                Name = "Plate 5",
                Description = "Sweet indulgence for dessert lovers",
                Image = "https://asshole.blob.core.windows.net/plates/plate5.jpeg",
                Price = 4.99,
                Category = "Dessert",
                SpecialTag = "Decadent Delight"
            },
            new MenuItem
            {
                Id = 6,
                Name = "Plate 6",
                Description = "A classic choice with a modern twist",
                Image = "https://asshole.blob.core.windows.net/plates/plate6.jpeg",
                Price = 6.99,
                Category = "Appetizer",
                SpecialTag = "Vegetarian Delight"
            }

            );
    }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
}
