using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrders10 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "AssignedSystemAdminId",
            table: "Order",
            type: "uuid",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AssignedSystemAdminId",
            table: "Order");
    }
}
