using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Accounts.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrgRequest6 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "EmailVerificationToken",
            table: "OrganizationRegisterRequest",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "EmailVerificationTokenExpiry",
            table: "OrganizationRegisterRequest",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsEmailVerified",
            table: "OrganizationRegisterRequest",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EmailVerificationToken",
            table: "OrganizationRegisterRequest");

        migrationBuilder.DropColumn(
            name: "EmailVerificationTokenExpiry",
            table: "OrganizationRegisterRequest");

        migrationBuilder.DropColumn(
            name: "IsEmailVerified",
            table: "OrganizationRegisterRequest");
    }
}
