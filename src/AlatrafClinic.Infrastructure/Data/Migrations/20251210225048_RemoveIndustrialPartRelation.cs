using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndustrialPartRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisIndustrialParts_IndustrialParts_IndustrialPartId",
                table: "DiagnosisIndustrialParts");

            migrationBuilder.DropIndex(
                name: "IX_DiagnosisIndustrialParts_IndustrialPartId",
                table: "DiagnosisIndustrialParts");

            migrationBuilder.DropColumn(
                name: "IndustrialPartId",
                table: "DiagnosisIndustrialParts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndustrialPartId",
                table: "DiagnosisIndustrialParts",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 1,
                column: "IndustrialPartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 2,
                column: "IndustrialPartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DiagnosisIndustrialParts",
                keyColumn: "DiagnosisIndustrialPartId",
                keyValue: 3,
                column: "IndustrialPartId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosisIndustrialParts_IndustrialPartId",
                table: "DiagnosisIndustrialParts",
                column: "IndustrialPartId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisIndustrialParts_IndustrialParts_IndustrialPartId",
                table: "DiagnosisIndustrialParts",
                column: "IndustrialPartId",
                principalTable: "IndustrialParts",
                principalColumn: "IndustrialPartId");
        }
    }
}
