using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateAccount7 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Organization_ActivationKey",
            table: "Organization");

        migrationBuilder.DropColumn(
            name: "ActivationKey",
            table: "Organization");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ActivationKey",
            table: "Organization",
            type: "VARCHAR(255)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Organization_ActivationKey",
            table: "Organization",
            column: "ActivationKey",
            unique: true);
    }
}
