using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetPaymentIdAsNeverGenerated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
