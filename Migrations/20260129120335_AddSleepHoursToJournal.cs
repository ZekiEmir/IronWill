using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronWill.Migrations
{
    /// <inheritdoc />
    public partial class AddSleepHoursToJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SleepHours",
                table: "JournalEntries",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SleepHours",
                table: "JournalEntries");
        }
    }
}
