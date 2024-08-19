using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSoftware.Migrations
{
    public partial class ne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasurement",
                table: "TaskMaterialUsages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfIssue",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "ID", "ManufacturerCode", "PartNumber", "Price", "UnitOfIssue" },
                values: new object[] { new Guid("3237c2c6-bf43-4bdd-9f64-ab3d4e520bd0"), 123, "M001", 50, "Liter" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "ID",
                keyValue: new Guid("3237c2c6-bf43-4bdd-9f64-ab3d4e520bd0"));

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfMeasurement",
                table: "TaskMaterialUsages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfIssue",
                table: "Materials",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
