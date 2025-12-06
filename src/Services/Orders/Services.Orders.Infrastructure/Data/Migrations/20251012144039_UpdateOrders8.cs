using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrders8 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "SubjectName",
            table: "OrderItem",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "TotalItems",
            table: "Order",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "SubjectName",
            table: "OrderItem");

        migrationBuilder.DropColumn(
            name: "TotalItems",
            table: "Order");
    }
}
