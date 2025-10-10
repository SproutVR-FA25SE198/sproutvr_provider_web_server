using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateAccount1 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "MACAddress",
            table: "Organization",
            type: "VARCHAR(20)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "VARCHAR(20)");

        migrationBuilder.AddColumn<string>(
            name: "RepresentativeName",
            table: "Organization",
            type: "VARCHAR(255)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "AvatarUrl",
            table: "AspNetUsers",
            type: "varchar(255)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RepresentativeName",
            table: "Organization");

        migrationBuilder.DropColumn(
            name: "AvatarUrl",
            table: "AspNetUsers");

        migrationBuilder.AlterColumn<string>(
            name: "MACAddress",
            table: "Organization",
            type: "VARCHAR(20)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "VARCHAR(20)",
            oldNullable: true);
    }
}
