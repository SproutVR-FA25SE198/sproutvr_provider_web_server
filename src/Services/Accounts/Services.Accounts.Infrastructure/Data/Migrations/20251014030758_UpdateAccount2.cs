using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateAccount2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Organization_ContactEmail",
            table: "Organization");

        migrationBuilder.DropIndex(
            name: "IX_Organization_ContactPhone",
            table: "Organization");

        migrationBuilder.DropColumn(
            name: "ContactEmail",
            table: "Organization");

        migrationBuilder.DropColumn(
            name: "ContactPhone",
            table: "Organization");

        migrationBuilder.AlterColumn<string>(
            name: "ApprovalStatus",
            table: "OrganizationRegisterRequest",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "Unverified",
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldDefaultValue: "Pending");

        migrationBuilder.AddColumn<string>(
            name: "RepresentativeName",
            table: "OrganizationRegisterRequest",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<string>(
            name: "PhoneNumber",
            table: "AspNetUsers",
            type: "VARCHAR(20)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "AspNetUsers",
            type: "VARCHAR(255)",
            maxLength: 256,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "character varying(256)",
            oldMaxLength: 256,
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RepresentativeName",
            table: "OrganizationRegisterRequest");

        migrationBuilder.AlterColumn<string>(
            name: "ApprovalStatus",
            table: "OrganizationRegisterRequest",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "Pending",
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldDefaultValue: "Unverified");

        migrationBuilder.AddColumn<string>(
            name: "ContactEmail",
            table: "Organization",
            type: "VARCHAR(100)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ContactPhone",
            table: "Organization",
            type: "VARCHAR(20)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<string>(
            name: "PhoneNumber",
            table: "AspNetUsers",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "VARCHAR(20)");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "AspNetUsers",
            type: "character varying(256)",
            maxLength: 256,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "VARCHAR(255)",
            oldMaxLength: 256);

        migrationBuilder.CreateIndex(
            name: "IX_Organization_ContactEmail",
            table: "Organization",
            column: "ContactEmail",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Organization_ContactPhone",
            table: "Organization",
            column: "ContactPhone",
            unique: true);
    }
}
