using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSagaIdFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InventoryReservationCompleted",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentRecorded",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SagaId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SagaId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SagaId",
                table: "InventoryReservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 10,
                column: "SagaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 11,
                column: "SagaId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 12,
                column: "SagaId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SagaId_DiagnosisId",
                table: "Sales",
                columns: new[] { "SagaId", "DiagnosisId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SagaId_PaymentReference_DiagnosisId",
                table: "Payments",
                columns: new[] { "SagaId", "PaymentReference", "DiagnosisId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_SagaId_SaleId",
                table: "InventoryReservations",
                columns: new[] { "SagaId", "SaleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sales_SagaId_DiagnosisId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SagaId_PaymentReference_DiagnosisId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_SagaId_SaleId",
                table: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "InventoryReservationCompleted",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PaymentRecorded",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SagaId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SagaId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SagaId",
                table: "InventoryReservations");
        }
    }
}
