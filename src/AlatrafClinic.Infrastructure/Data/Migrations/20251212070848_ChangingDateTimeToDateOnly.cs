using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangingDateTimeToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "AttendDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "Diagnoses",
                keyColumn: "DiagnosisId",
                keyValue: 1,
                column: "InjuryDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "Diagnoses",
                keyColumn: "DiagnosisId",
                keyValue: 2,
                column: "InjuryDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "Diagnoses",
                keyColumn: "DiagnosisId",
                keyValue: 3,
                column: "InjuryDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 1,
                column: "DoctorAssignDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 2,
                column: "DoctorAssignDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 3,
                column: "DoctorAssignDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 1,
                column: "ExpirationDate",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 2,
                column: "ExpirationDate",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 3,
                column: "ExpirationDate",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 1,
                columns: new[] { "AssignDate", "EndDate" },
                values: new object[] { new DateOnly(2025, 1, 1), null });

            migrationBuilder.UpdateData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 2,
                columns: new[] { "AssignDate", "EndDate" },
                values: new object[] { new DateOnly(2025, 1, 1), null });

            migrationBuilder.UpdateData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 3,
                columns: new[] { "AssignDate", "EndDate" },
                values: new object[] { new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 1) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 3,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateOnly(1, 5, 1) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 4,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateOnly(1, 5, 22) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 5,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateOnly(1, 9, 26) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 6,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateOnly(1, 10, 14) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 7,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateOnly(1, 11, 30) });

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 1,
                column: "Birthdate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 2,
                column: "Birthdate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 3,
                column: "Birthdate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 4,
                column: "Birthdate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 1,
                column: "SessionDate",
                value: new DateOnly(2025, 1, 10));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 2,
                column: "SessionDate",
                value: new DateOnly(2025, 1, 11));

            migrationBuilder.UpdateData(
                table: "TherapyCards",
                keyColumn: "TherapyCardId",
                keyValue: 1,
                columns: new[] { "ProgramEndDate", "ProgramStartDate" },
                values: new object[] { new DateOnly(2025, 1, 20), new DateOnly(2025, 1, 1) });

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 1,
                column: "Expiration",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 2,
                column: "Expiration",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 3,
                column: "Expiration",
                value: new DateOnly(2025, 4, 11));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                column: "AttendDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Diagnoses",
                keyColumn: "DiagnosisId",
                keyValue: 1,
                column: "InjuryDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Diagnoses",
                keyColumn: "DiagnosisId",
                keyValue: 2,
                column: "InjuryDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Diagnoses",
                keyColumn: "DiagnosisId",
                keyValue: 3,
                column: "InjuryDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 1,
                column: "DoctorAssignDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 2,
                column: "DoctorAssignDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 3,
                column: "DoctorAssignDate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 1,
                column: "ExpirationDate",
                value: new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 2,
                column: "ExpirationDate",
                value: new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 3,
                column: "ExpirationDate",
                value: new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 1,
                columns: new[] { "AssignDate", "EndDate" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 2,
                columns: new[] { "AssignDate", "EndDate" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "DoctorSectionRooms",
                keyColumn: "DoctorSectionRoomId",
                keyValue: 3,
                columns: new[] { "AssignDate", "EndDate" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 3,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(1, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 4,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(1, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 5,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(1, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 6,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(1, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "HolidayId",
                keyValue: 7,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(1, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 1,
                column: "Birthdate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 2,
                column: "Birthdate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 3,
                column: "Birthdate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "People",
                keyColumn: "PersonId",
                keyValue: 4,
                column: "Birthdate",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 1,
                column: "SessionDate",
                value: new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "SessionId",
                keyValue: 2,
                column: "SessionDate",
                value: new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "TherapyCards",
                keyColumn: "TherapyCardId",
                keyValue: 1,
                columns: new[] { "ProgramEndDate", "ProgramStartDate" },
                values: new object[] { new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 1,
                column: "Expiration",
                value: new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 2,
                column: "Expiration",
                value: new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 3,
                column: "Expiration",
                value: new DateTime(2025, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
