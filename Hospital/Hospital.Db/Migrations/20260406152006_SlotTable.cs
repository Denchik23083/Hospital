using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Db.Migrations
{
    /// <inheritdoc />
    public partial class SlotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_DoctorSlot_DoctorSlotId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Patients_PatientId",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSlot_Doctors_DoctorId",
                table: "DoctorSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorSlot",
                table: "DoctorSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.RenameTable(
                name: "DoctorSlot",
                newName: "DoctorSlots");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorSlot_DoctorId",
                table: "DoctorSlots",
                newName: "IX_DoctorSlots_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_PatientId",
                table: "Bookings",
                newName: "IX_Bookings_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_DoctorSlotId",
                table: "Bookings",
                newName: "IX_Bookings_DoctorSlotId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorSlots",
                table: "DoctorSlots",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_DoctorSlots_DoctorSlotId",
                table: "Bookings",
                column: "DoctorSlotId",
                principalTable: "DoctorSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Patients_PatientId",
                table: "Bookings",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSlots_Doctors_DoctorId",
                table: "DoctorSlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_DoctorSlots_DoctorSlotId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Patients_PatientId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSlots_Doctors_DoctorId",
                table: "DoctorSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorSlots",
                table: "DoctorSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "DoctorSlots",
                newName: "DoctorSlot");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorSlots_DoctorId",
                table: "DoctorSlot",
                newName: "IX_DoctorSlot_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_PatientId",
                table: "Booking",
                newName: "IX_Booking_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_DoctorSlotId",
                table: "Booking",
                newName: "IX_Booking_DoctorSlotId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorSlot",
                table: "DoctorSlot",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Booking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_DoctorSlot_DoctorSlotId",
                table: "Booking",
                column: "DoctorSlotId",
                principalTable: "DoctorSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Patients_PatientId",
                table: "Booking",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSlot_Doctors_DoctorId",
                table: "DoctorSlot",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
