using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRemoveWoundedCardIdFromWoundedPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WoundedPayments_WoundedCards_WoundedCardId",
                table: "WoundedPayments");

            migrationBuilder.DropIndex(
                name: "IX_WoundedPayments_WoundedCardId",
                table: "WoundedPayments");

            migrationBuilder.DropColumn(
                name: "WoundedCardId",
                table: "WoundedPayments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WoundedCardId",
                table: "WoundedPayments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "WoundedPayments",
                keyColumn: "WoundedPaymentId",
                keyValue: 11,
                column: "WoundedCardId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_WoundedPayments_WoundedCardId",
                table: "WoundedPayments",
                column: "WoundedCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_WoundedPayments_WoundedCards_WoundedCardId",
                table: "WoundedPayments",
                column: "WoundedCardId",
                principalTable: "WoundedCards",
                principalColumn: "WoundedCardId");
        }
    }
}
