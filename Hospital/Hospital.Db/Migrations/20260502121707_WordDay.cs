using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Db.Migrations
{
    /// <inheritdoc />
    public partial class WordDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkDayEnd",
                table: "Doctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkDayStart",
                table: "Doctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkDayEnd",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "WorkDayStart",
                table: "Doctors");
        }
    }
}
