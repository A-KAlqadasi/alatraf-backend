using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatesalesopration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_ExchangeOrders_ExchangeOrderId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ExchangeOrderId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ExchangeOrderId",
                table: "Sales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExchangeOrderId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ExchangeOrderId",
                table: "Sales",
                column: "ExchangeOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_ExchangeOrders_ExchangeOrderId",
                table: "Sales",
                column: "ExchangeOrderId",
                principalTable: "ExchangeOrders",
                principalColumn: "Id");
        }
    }
}
