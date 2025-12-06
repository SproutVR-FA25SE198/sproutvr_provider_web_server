using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Catalogs.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateMapPreview : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PreviewUrl",
            table: "Map",
            type: "varchar(300)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PreviewUrl",
            table: "Map");
    }
}
