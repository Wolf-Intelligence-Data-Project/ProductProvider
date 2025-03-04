using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductProvider.Migrations
{
    /// <inheritdoc />
    public partial class reservationsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CitiesByRegion",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CitiesByRegion",
                table: "Reservations");
        }
    }
}
