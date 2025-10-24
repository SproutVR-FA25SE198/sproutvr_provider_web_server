using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Payments.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdatePayment1 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "PaymentTransaction",
            type: "varchar(50)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AlterColumn<string>(
            name: "Currency",
            table: "PaymentTransaction",
            type: "varchar(10)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<string>(
            name: "BankCode",
            table: "PaymentTransaction",
            type: "varchar(20)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BankCode",
            table: "PaymentTransaction");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "PaymentTransaction",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(50)");

        migrationBuilder.AlterColumn<string>(
            name: "Currency",
            table: "PaymentTransaction",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(10)");
    }
}
