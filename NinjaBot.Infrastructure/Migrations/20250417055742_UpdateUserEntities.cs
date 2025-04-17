using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NinjaBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "User",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locale",
                table: "User");
        }
    }
}
