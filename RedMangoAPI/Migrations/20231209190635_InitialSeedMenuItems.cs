using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RedMangoAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Category", "Description", "Image", "Name", "Price", "SpecialTag" },
                values: new object[,]
                {
                    { 1, "Dessert", "Nice one you can try", "https://asshole.blob.core.windows.net/plates/plate1.jpeg", "Plate 1", 3.9900000000000002, "Top Rated" },
                    { 2, "Appetizer", "Delicious and tempting", "https://asshole.blob.core.windows.net/plates/plate2.jpeg", "Plate 2", 5.9900000000000002, "Chef's Recommendation" },
                    { 3, "Main Course", "A savory delight for your taste buds", "https://asshole.blob.core.windows.net/plates/plate3.jpeg", "Plate 3", 7.9900000000000002, "Spicy Special" },
                    { 4, "Seafood", "Irresistible flavor combination", "https://asshole.blob.core.windows.net/plates/plate4.jpeg", "Plate 4", 9.9900000000000002, "Fresh Catch" },
                    { 5, "Dessert", "Sweet indulgence for dessert lovers", "https://asshole.blob.core.windows.net/plates/plate5.jpeg", "Plate 5", 4.9900000000000002, "Decadent Delight" },
                    { 6, "Appetizer", "A classic choice with a modern twist", "https://asshole.blob.core.windows.net/plates/plate6.jpeg", "Plate 6", 6.9900000000000002, "Vegetarian Delight" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
