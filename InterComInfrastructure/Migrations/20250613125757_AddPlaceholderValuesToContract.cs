using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterComInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceholderValuesToContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlaceholderValuesJson",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceholderValuesJson",
                table: "Contracts");
        }
    }
}
