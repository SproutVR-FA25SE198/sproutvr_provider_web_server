using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrders9 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RepresentativeName",
            table: "Order",
            type: "VARCHAR(100)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "RepresentativePhone",
            table: "Order",
            type: "VARCHAR(20)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RepresentativeName",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "RepresentativePhone",
            table: "Order");
    }
}
