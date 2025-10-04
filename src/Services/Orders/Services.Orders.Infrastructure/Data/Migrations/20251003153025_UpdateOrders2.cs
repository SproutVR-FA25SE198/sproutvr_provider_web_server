using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;
/// <inheritdoc />
public partial class UpdateOrders2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ImageUrl",
            table: "OrderItem",
            type: "varchar(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "MapCode",
            table: "OrderItem",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "MapName",
            table: "OrderItem",
            type: "varchar(300)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<decimal>(
            name: "Price",
            table: "OrderItem",
            type: "numeric(18,2)",
            nullable: false,
            defaultValue: 0m);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ImageUrl",
            table: "OrderItem");

        migrationBuilder.DropColumn(
            name: "MapCode",
            table: "OrderItem");

        migrationBuilder.DropColumn(
            name: "MapName",
            table: "OrderItem");

        migrationBuilder.DropColumn(
            name: "Price",
            table: "OrderItem");
    }
}

