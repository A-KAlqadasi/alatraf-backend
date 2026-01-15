using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ResolveTheRealationInStoreItemUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreItemUnits_StoreId",
                table: "StoreItemUnits");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StoreItemUnits",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.CreateIndex(
                name: "IX_StoreItemUnits_StoreId_ItemUnitId",
                table: "StoreItemUnits",
                columns: new[] { "StoreId", "ItemUnitId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoreItemUnits_StoreId_ItemUnitId",
                table: "StoreItemUnits");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "StoreItemUnits",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_StoreItemUnits_StoreId",
                table: "StoreItemUnits",
                column: "StoreId");
        }
    }
}
