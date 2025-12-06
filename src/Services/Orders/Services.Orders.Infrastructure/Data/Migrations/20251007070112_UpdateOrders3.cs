using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;
/// <inheritdoc />
public partial class UpdateOrders3 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "OrderCode",
            table: "Order",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PaymentIntentId",
            table: "Order",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "OrderCode",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "PaymentIntentId",
            table: "Order");
    }
}

