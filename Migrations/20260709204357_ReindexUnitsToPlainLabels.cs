using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PricePulse.Migrations
{
    /// <inheritdoc />
    public partial class ReindexUnitsToPlainLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAPPNS",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAUCNS",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIMEDNS",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEEB",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEEE",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEHA",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SERE01",
                column: "Units",
                value: "price index");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "OPHNFB",
                column: "Units",
                value: "output per hour");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAPPNS",
                column: "Units",
                value: "index (1982-84=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIAUCNS",
                column: "Units",
                value: "index (1982-84=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CPIMEDNS",
                column: "Units",
                value: "index (1982-84=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEEB",
                column: "Units",
                value: "index (1982-84=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEEE",
                column: "Units",
                value: "index (Dec 2024=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SEHA",
                column: "Units",
                value: "index (1982-84=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "CUUR0000SERE01",
                column: "Units",
                value: "index (1982-84=100)");

            migrationBuilder.UpdateData(
                table: "Series",
                keyColumn: "Id",
                keyValue: "OPHNFB",
                column: "Units",
                value: "output/hour · index (2017=100)");
        }
    }
}
