using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDoctorsToSectionAndRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TherapyCards_Payments_PaymentId",
                table: "TherapyCards");

            migrationBuilder.DropIndex(
                name: "IX_TherapyCards_PaymentId",
                table: "TherapyCards");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "TherapyCards");

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "DoctorId", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "DepartmentId", "IsActive", "IsDeleted", "LastModifiedBy", "LastModifiedUtc", "PersonId", "Specialization" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Seed", null, null, 1, true, false, "Seed", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "أخصائي علاج طبيعي" },
                    { 2, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Seed", null, null, 2, true, false, "Seed", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, "اخصائي اطراف صناعية" },
                    { 3, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Seed", null, null, 1, false, false, "Seed", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, "أخصائية أعصاب" }
                });

            migrationBuilder.InsertData(
                table: "DoctorSectionRooms",
                columns: new[] { "DoctorSectionRoomId", "AssignDate", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DeletedBy", "DoctorId", "EndDate", "IsActive", "IsDeleted", "LastModifiedBy", "LastModifiedUtc", "Notes", "RoomId", "SectionId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Seed", null, null, 1, null, true, false, "Seed", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "تكليف أساسي للطبيب في القسم الأول", 1, 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Seed", null, null, 2, null, true, false, "Seed", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "تكليف للطبيب بقسم الحديد", null, 8 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Seed", null, null, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "Seed", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "تكليف منتهي للطبيبة في قسم الحرارة", 3, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "TherapyCards",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TherapyCards",
                keyColumn: "TherapyCardId",
                keyValue: 1,
                column: "PaymentId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_TherapyCards_PaymentId",
                table: "TherapyCards",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TherapyCards_Payments_PaymentId",
                table: "TherapyCards",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId");
        }
    }
}
