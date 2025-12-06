using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateAccount5 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "NumberOfPendingOrders",
            table: "SystemAdmin",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NumberOfPendingOrders",
            table: "SystemAdmin");
    }
}
