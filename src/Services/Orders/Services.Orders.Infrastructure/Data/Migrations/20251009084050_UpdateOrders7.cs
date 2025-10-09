using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;
/// <inheritdoc />
public partial class UpdateOrders7 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_OrderItem_Order_OrderId",
            table: "OrderItem");

        migrationBuilder.AddForeignKey(
            name: "FK_OrderItem_Order_OrderId",
            table: "OrderItem",
            column: "OrderId",
            principalTable: "Order",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_OrderItem_Order_OrderId",
            table: "OrderItem");

        migrationBuilder.AddForeignKey(
            name: "FK_OrderItem_Order_OrderId",
            table: "OrderItem",
            column: "OrderId",
            principalTable: "Order",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
