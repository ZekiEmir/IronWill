using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronWill.Migrations
{
    /// <inheritdoc />
    public partial class FixProjectSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AgreedPrice",
                table: "Projects",
                newName: "Deadline");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Topics",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ContractAmount",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "ContractAmount",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "Projects",
                newName: "AgreedPrice");
        }
    }
}
