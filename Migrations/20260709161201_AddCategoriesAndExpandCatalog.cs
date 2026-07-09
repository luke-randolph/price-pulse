using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PricePulse.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesAndExpandCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Series",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Series",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000702111",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000703112",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000708111",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU000072610",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 1, 0 });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU000074714",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 1, 0 });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CEU0500000008",
                columns: new[] { "Category", "IsReference", "Kind", "Name" },
                values: new object[] { 6, false, 2, "Non-Executive Wage" });

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAUCNS",
                columns: new[] { "Category", "Kind" },
                values: new object[] { 2, 1 });

            migrationBuilder.InsertData(
                table: "Series",
                columns: new[] { "Id", "Category", "Icon", "IsReference", "Kind", "Name", "Units" },
                values: new object[,]
                {
                    { "APU0000701312", 0, "rice_bowl", false, 0, "Rice", "per pound" },
                    { "APU0000706111", 0, "set_meal", false, 0, "Chicken", "per pound" },
                    { "APU0000711211", 0, "nutrition", false, 0, "Bananas", "per pound" },
                    { "APU0000712311", 0, "eco", false, 0, "Tomatoes", "per pound" },
                    { "APU0000717311", 0, "coffee", false, 0, "Coffee", "per pound" },
                    { "CPIAPPNS", 5, "checkroom", false, 1, "Clothing", "index (1982-84=100)" },
                    { "CPIMEDNS", 4, "medical_services", false, 1, "Medical Care", "index (1982-84=100)" },
                    { "CUUR0000SEEB", 3, "school", false, 1, "Tuition", "index (1982-84=100)" },
                    { "CUUR0000SEEE", 5, "devices", false, 1, "Electronics", "index (Dec 2024=100)" },
                    { "CUUR0000SEHA", 2, "apartment", false, 1, "Rent", "index (1982-84=100)" },
                    { "CUUR0000SERE01", 5, "toys", false, 1, "Toys", "index (1982-84=100)" },
                    { "MSPUS", 2, "home", false, 0, "Median Home Price", "U.S. median · MSPUS" },
                    { "OPHNFB", 7, "speed", false, 3, "Productivity", "output/hour · index (2017=100)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000701312");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000706111");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000711211");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000712311");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000717311");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAPPNS");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIMEDNS");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEEB");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEEE");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEHA");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SERE01");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "MSPUS");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "OPHNFB");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Series");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CEU0500000008",
                columns: new[] { "IsReference", "Name" },
                values: new object[] { true, "Average Hourly Earnings" });
        }
    }
}
