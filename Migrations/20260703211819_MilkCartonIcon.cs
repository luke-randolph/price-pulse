using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PricePulse.Migrations
{
    /// <inheritdoc />
    public partial class MilkCartonIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                column: "Icon",
                value: "svg:milk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                column: "Icon",
                value: "local_drink");
        }
    }
}
