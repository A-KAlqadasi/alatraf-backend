using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editExchangeOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrders_RelatedOrderId",
                table: "ExchangeOrders",
                column: "RelatedOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeOrders_RelatedSaleId",
                table: "ExchangeOrders",
                column: "RelatedSaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeOrders_Orders_RelatedOrderId",
                table: "ExchangeOrders",
                column: "RelatedOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeOrders_Sales_RelatedSaleId",
                table: "ExchangeOrders",
                column: "RelatedSaleId",
                principalTable: "Sales",
                principalColumn: "SaleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeOrders_Orders_RelatedOrderId",
                table: "ExchangeOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeOrders_Sales_RelatedSaleId",
                table: "ExchangeOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeOrders_RelatedOrderId",
                table: "ExchangeOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeOrders_RelatedSaleId",
                table: "ExchangeOrders");
        }
    }
}
