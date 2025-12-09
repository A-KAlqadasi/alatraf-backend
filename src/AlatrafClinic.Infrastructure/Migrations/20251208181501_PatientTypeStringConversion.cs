using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatientTypeStringConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_People_PersonId",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "PatientType",
                table: "Patients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 1,
                column: "PatientType",
                value: "Normal");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2,
                column: "PatientType",
                value: "Wounded");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 3,
                column: "PatientType",
                value: "Disabled");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_People_PersonId",
                table: "Patients",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_People_PersonId",
                table: "Patients");

            migrationBuilder.AlterColumn<byte>(
                name: "PatientType",
                table: "Patients",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 1,
                column: "PatientType",
                value: (byte)0);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2,
                column: "PatientType",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 3,
                column: "PatientType",
                value: (byte)2);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_People_PersonId",
                table: "Patients",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
