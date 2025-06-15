using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductProvider.Migrations
{
    /// <inheritdoc />
    public partial class fixingrevenuedecimalforbulk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.AlterColumn<decimal>(
                name: "Revenue",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Revenue",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

        }
    }
}
