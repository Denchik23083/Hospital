using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hospital.Db.Migrations
{
    /// <inheritdoc />
    public partial class DoctorPatientSpecialty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GenderType = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patient_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Specialty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    GenderType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctor_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctor_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Specialty",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Терапия", 40m },
                    { 2, "Кардиология", 80m },
                    { 3, "Неврология", 75m },
                    { 4, "Офтальмология", 50m },
                    { 5, "Ортопедия", 70m },
                    { 6, "Эндокринология", 65m },
                    { 7, "Пульмонология", 70m },
                    { 8, "Психиатрия", 90m },
                    { 9, "Стоматология", 85m }
                });

            migrationBuilder.InsertData(
                table: "Doctor",
                columns: new[] { "Id", "ExperienceYears", "FirstName", "GenderType", "LastName", "SpecialtyId", "UserId" },
                values: new object[,]
                {
                    { 1, 2, "Глеб", 1, "Романенко", 1, null },
                    { 2, 3, "Семен", 1, "Лобанов", 1, null },
                    { 3, 2, "Борис", 1, "Левин", 1, null },
                    { 4, 1, "Варвара", 2, "Черноус", 1, null },
                    { 5, 3, "Мария", 2, "Колисниченко", 2, null },
                    { 6, 1, "Светлана", 2, "Чернышова", 2, null },
                    { 7, 5, "Вячеслав", 1, "Селезнев", 2, null },
                    { 8, 7, "Станислав", 1, "Башницен", 3, null },
                    { 9, 3, "Васелиса", 2, "Шмид", 3, null },
                    { 10, 4, "Дарья", 2, "Зайченко", 4, null },
                    { 11, 1, "Анатолий", 1, "Войченко", 4, null },
                    { 12, 5, "Евгений", 1, "Шевчук", 5, null },
                    { 13, 2, "Катерина", 2, "Главко", 5, null },
                    { 14, 3, "Елизавета", 2, "Сидорчук", 6, null },
                    { 15, 8, "Петр", 1, "Иващенко", 6, null },
                    { 16, 2, "Тарас", 1, "Гайдар", 7, null },
                    { 17, 5, "Анастасия", 2, "Громова", 7, null },
                    { 18, 4, "Вероника", 2, "Борова", 8, null },
                    { 19, 2, "Оксана", 2, "Свиридова", 9, null },
                    { 20, 3, "Полина", 2, "Ушакова", 9, null },
                    { 21, 6, "Денис", 1, "Никифоров", 9, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_SpecialtyId",
                table: "Doctor",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_UserId",
                table: "Doctor",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_UserId",
                table: "Patient",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "Specialty");
        }
    }
}
