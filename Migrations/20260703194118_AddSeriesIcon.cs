using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PricePulse.Migrations
{
    /// <inheritdoc />
    public partial class AddSeriesIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Series",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000702111",
                column: "Icon",
                value: "bakery_dining");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000703112",
                column: "Icon",
                value: "lunch_dining");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000708111",
                column: "Icon",
                value: "egg");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU0000709112",
                column: "Icon",
                value: "local_drink");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU000072610",
                column: "Icon",
                value: "bolt");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "APU000074714",
                column: "Icon",
                value: "local_gas_station");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Series");
        }
    }
}
