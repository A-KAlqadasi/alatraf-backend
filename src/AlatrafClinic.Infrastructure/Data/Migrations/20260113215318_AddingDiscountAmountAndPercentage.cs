using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingDiscountAmountAndPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "Payments",
                newName: "DiscountAmount");

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "Payments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Payments",
                newName: "Discount");
        }
    }
}
