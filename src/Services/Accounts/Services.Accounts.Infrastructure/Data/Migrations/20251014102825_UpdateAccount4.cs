using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateAccount4 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RepresentativeName",
            table: "OrganizationRegisterRequest");

        migrationBuilder.DropColumn(
            name: "RepresentativeName",
            table: "Organization");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RepresentativeName",
            table: "OrganizationRegisterRequest",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "RepresentativeName",
            table: "Organization",
            type: "VARCHAR(255)",
            nullable: false,
            defaultValue: "");
    }
}
