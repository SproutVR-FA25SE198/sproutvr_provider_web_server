using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrders4 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<long>(
            name: "PayosOrderCode",
            table: "Order",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Order_OrderCode",
            table: "Order",
            column: "PayosOrderCode",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Order_OrderCode",
            table: "Order");

        migrationBuilder.AlterColumn<int>(
            name: "PayosOrderCode",
            table: "Order",
            type: "integer",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);
    }
}
