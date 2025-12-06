using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Catalogs.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddMetadataStoragePath : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "MetadataStoragePath",
            table: "Map",
            type: "varchar(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MetadataStoragePath",
            table: "Map");
    }
}
