using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Catalogs.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RenameLocationId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ObjectLocation_TaskLocation_LocationId",
            table: "ObjectLocation");

        migrationBuilder.RenameColumn(
            name: "LocationId",
            table: "ObjectLocation",
            newName: "TaskLocationId");

        migrationBuilder.RenameIndex(
            name: "IX_ObjectLocation_LocationId",
            table: "ObjectLocation",
            newName: "IX_ObjectLocation_TaskLocationId");

        migrationBuilder.AddForeignKey(
            name: "FK_ObjectLocation_TaskLocation_TaskLocationId",
            table: "ObjectLocation",
            column: "TaskLocationId",
            principalTable: "TaskLocation",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ObjectLocation_TaskLocation_TaskLocationId",
            table: "ObjectLocation");

        migrationBuilder.RenameColumn(
            name: "TaskLocationId",
            table: "ObjectLocation",
            newName: "LocationId");

        migrationBuilder.RenameIndex(
            name: "IX_ObjectLocation_TaskLocationId",
            table: "ObjectLocation",
            newName: "IX_ObjectLocation_LocationId");

        migrationBuilder.AddForeignKey(
            name: "FK_ObjectLocation_TaskLocation_LocationId",
            table: "ObjectLocation",
            column: "LocationId",
            principalTable: "TaskLocation",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
