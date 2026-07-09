using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PricePulse.Migrations
{
    /// <inheritdoc />
    public partial class SwitchGroceryIconsToLucide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000706111",
                column: "Icon",
                value: "lucide:drumstick");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                column: "Icon",
                value: "lucide:milk");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000711211",
                column: "Icon",
                value: "lucide:banana");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000706111",
                column: "Icon",
                value: "set_meal");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                column: "Icon",
                value: "svg:milk");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000711211",
                column: "Icon",
                value: "nutrition");
        }
    }
}
