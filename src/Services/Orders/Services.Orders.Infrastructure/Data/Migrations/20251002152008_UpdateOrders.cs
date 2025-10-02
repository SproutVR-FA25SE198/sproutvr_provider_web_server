using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Orders.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class UpdateOrders : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "TransactionCode",
            table: "Order",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(
            name: "PaymentMethod",
            table: "Order",
            type: "varchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "BundleUrl",
            table: "Order",
            type: "varchar(300)",
            maxLength: 300,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(300)",
            oldMaxLength: 300);

        migrationBuilder.AlterColumn<string>(
            name: "Bank",
            table: "Order",
            type: "varchar(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "TransactionCode",
            table: "Order",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PaymentMethod",
            table: "Order",
            type: "varchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "varchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "BundleUrl",
            table: "Order",
            type: "varchar(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "varchar(300)",
            oldMaxLength: 300,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Bank",
            table: "Order",
            type: "varchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "varchar(100)",
            oldMaxLength: 100,
            oldNullable: true);
    }
}

