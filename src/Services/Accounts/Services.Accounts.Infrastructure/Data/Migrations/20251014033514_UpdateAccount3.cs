using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateAccount3 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "AvatarUrl",
            table: "AspNetUsers",
            type: "varchar(255)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(255)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "AvatarUrl",
            table: "AspNetUsers",
            type: "varchar(255)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "varchar(255)",
            oldNullable: true);
    }
}
