using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrders11 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Bank",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "PaymentMethod",
            table: "Order");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Bank",
            table: "Order",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PaymentMethod",
            table: "Order",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true);
    }
}
