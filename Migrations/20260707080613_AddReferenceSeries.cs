using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PricePulse.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReference",
                table: "Series",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000702111",
                column: "IsReference",
                value: false);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000703112",
                column: "IsReference",
                value: false);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000708111",
                column: "IsReference",
                value: false);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                column: "IsReference",
                value: false);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU000072610",
                column: "IsReference",
                value: false);

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU000074714",
                column: "IsReference",
                value: false);

            migrationBuilder.InsertData(
                table: "Series",
                columns: new[] { "Id", "Icon", "IsReference", "Name", "Units" },
                values: new object[,]
                {
                    { "CEU0500000008", "payments", true, "Average Hourly Earnings", "per hour" },
                    { "CPIAUCNS", "trending_up", true, "Consumer Price Index", "index (1982-84=100)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CEU0500000008");

            migrationBuilder.DeleteData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAUCNS");

            migrationBuilder.DropColumn(
                name: "IsReference",
                table: "Series");
        }
    }
}
