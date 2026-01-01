using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWoundedCardFromWoundedPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WoundedPayments_WoundedCards_WoundedCardId",
                table: "WoundedPayments");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "DisabledCards");

            migrationBuilder.AlterColumn<int>(
                name: "WoundedCardId",
                table: "WoundedPayments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DisabilityType",
                table: "DisabledCards",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 1,
                column: "DisabilityType",
                value: "بتر القدم اليمنى");

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 2,
                column: "DisabilityType",
                value: "شلل نصفي");

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 3,
                column: "DisabilityType",
                value: "فقدان التوازن الحركي");

            migrationBuilder.UpdateData(
                table: "WoundedPayments",
                keyColumn: "WoundedPaymentId",
                keyValue: 11,
                column: "WoundedCardId",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_WoundedPayments_WoundedCards_WoundedCardId",
                table: "WoundedPayments",
                column: "WoundedCardId",
                principalTable: "WoundedCards",
                principalColumn: "WoundedCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WoundedPayments_WoundedCards_WoundedCardId",
                table: "WoundedPayments");

            migrationBuilder.DropColumn(
                name: "DisabilityType",
                table: "DisabledCards");

            migrationBuilder.AlterColumn<int>(
                name: "WoundedCardId",
                table: "WoundedPayments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpirationDate",
                table: "DisabledCards",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

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
                value: new DateOnly(2025, 1, 11));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 3,
                column: "ExpirationDate",
                value: new DateOnly(2025, 1, 31));

            migrationBuilder.UpdateData(
                table: "WoundedPayments",
                keyColumn: "WoundedPaymentId",
                keyValue: 11,
                column: "WoundedCardId",
                value: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_WoundedPayments_WoundedCards_WoundedCardId",
                table: "WoundedPayments",
                column: "WoundedCardId",
                principalTable: "WoundedCards",
                principalColumn: "WoundedCardId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
