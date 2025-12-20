using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlatrafClinic.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueDateToWoundedAndDisabledCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Expiration",
                table: "WoundedCards",
                newName: "IssueDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpirationDate",
                table: "WoundedCards",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "IssueDate",
                table: "DisabledCards",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 1,
                column: "IssueDate",
                value: new DateOnly(2025, 1, 1));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 2,
                columns: new[] { "ExpirationDate", "IssueDate" },
                values: new object[] { new DateOnly(2025, 1, 11), new DateOnly(2025, 1, 1) });

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 3,
                columns: new[] { "ExpirationDate", "IssueDate" },
                values: new object[] { new DateOnly(2025, 1, 31), new DateOnly(2025, 1, 1) });

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 1,
                columns: new[] { "ExpirationDate", "IssueDate" },
                values: new object[] { new DateOnly(2025, 4, 11), new DateOnly(2025, 1, 1) });

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 2,
                columns: new[] { "ExpirationDate", "IssueDate" },
                values: new object[] { new DateOnly(2025, 1, 11), new DateOnly(2025, 1, 1) });

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 3,
                columns: new[] { "ExpirationDate", "IssueDate" },
                values: new object[] { new DateOnly(2025, 1, 31), new DateOnly(2025, 1, 1) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "WoundedCards");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "DisabledCards");

            migrationBuilder.RenameColumn(
                name: "IssueDate",
                table: "WoundedCards",
                newName: "Expiration");

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 2,
                column: "ExpirationDate",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "DisabledCards",
                keyColumn: "DisabledCardId",
                keyValue: 3,
                column: "ExpirationDate",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 1,
                column: "Expiration",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 2,
                column: "Expiration",
                value: new DateOnly(2025, 4, 11));

            migrationBuilder.UpdateData(
                table: "WoundedCards",
                keyColumn: "WoundedCardId",
                keyValue: 3,
                column: "Expiration",
                value: new DateOnly(2025, 4, 11));
        }
    }
}
