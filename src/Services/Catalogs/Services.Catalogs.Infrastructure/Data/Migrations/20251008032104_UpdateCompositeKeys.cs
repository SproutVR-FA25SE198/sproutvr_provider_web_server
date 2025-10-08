using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Catalogs.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateCompositeKeys : Migration
{
    private static readonly string[] columns = new[] { "ObjectId", "LocationId" };
    private static readonly string[] columnsArray = new[] { "ActivityTypeId", "MapObjectId" };
    private static readonly string[] columnsArray0 = new[] { "ObjectId", "LocationId" };
    private static readonly string[] columnsArray1 = new[] { "ActivityTypeId", "MapObjectId" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_ObjectLocation",
            table: "ObjectLocation");

        migrationBuilder.DropIndex(
            name: "IX_ObjectLocation_ObjectId_LocationId",
            table: "ObjectLocation");

        migrationBuilder.DropPrimaryKey(
            name: "PK_ObjectActivityType",
            table: "ObjectActivityType");

        migrationBuilder.DropIndex(
            name: "IX_ObjectActivityType_ActivityTypeId_MapObjectId",
            table: "ObjectActivityType");

        migrationBuilder.DropColumn(
            name: "Id",
            table: "ObjectLocation");

        migrationBuilder.DropColumn(
            name: "Id",
            table: "ObjectActivityType");

        migrationBuilder.AddPrimaryKey(
            name: "PK_ObjectLocation",
            table: "ObjectLocation",
            columns: columns);

        migrationBuilder.AddPrimaryKey(
            name: "PK_ObjectActivityType",
            table: "ObjectActivityType",
            columns: columnsArray);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_ObjectLocation",
            table: "ObjectLocation");

        migrationBuilder.DropPrimaryKey(
            name: "PK_ObjectActivityType",
            table: "ObjectActivityType");

        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "ObjectLocation",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "ObjectActivityType",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddPrimaryKey(
            name: "PK_ObjectLocation",
            table: "ObjectLocation",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_ObjectActivityType",
            table: "ObjectActivityType",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_ObjectLocation_ObjectId_LocationId",
            table: "ObjectLocation",
            columns: columnsArray0,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ObjectActivityType_ActivityTypeId_MapObjectId",
            table: "ObjectActivityType",
            columns: columnsArray1,
            unique: true);
    }
}
