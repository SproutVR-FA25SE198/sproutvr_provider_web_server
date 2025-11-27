using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Catalogs.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RemoveUniqueConstraintInMapObject : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_MapObject_ObjectCode_MapId",
            table: "MapObject");
    }
    private static readonly string[] columns = new[] { "ObjectCode", "MapId" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_MapObject_ObjectCode_MapId",
            table: "MapObject",
            columns: columns,
            unique: true);
    }
}
