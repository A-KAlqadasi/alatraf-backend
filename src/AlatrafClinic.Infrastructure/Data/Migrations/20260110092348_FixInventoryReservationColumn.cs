using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixInventoryReservationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReservations_StoreItemUnits_StoreItemUnitId1",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_StoreItemUnitId1",
                table: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "StoreItemUnitId1",
                table: "InventoryReservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreItemUnitId1",
                table: "InventoryReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_StoreItemUnitId1",
                table: "InventoryReservations",
                column: "StoreItemUnitId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReservations_StoreItemUnits_StoreItemUnitId1",
                table: "InventoryReservations",
                column: "StoreItemUnitId1",
                principalTable: "StoreItemUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
