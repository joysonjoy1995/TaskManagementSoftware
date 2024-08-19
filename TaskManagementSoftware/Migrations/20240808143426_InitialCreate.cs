using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSoftware.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskMaterialUsages",
                table: "TaskMaterialUsages");

            migrationBuilder.DropIndex(
                name: "IX_TaskMaterialUsages_TaskID",
                table: "TaskMaterialUsages");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskMaterialUsages",
                table: "TaskMaterialUsages",
                columns: new[] { "TaskID", "MaterialID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskMaterialUsages",
                table: "TaskMaterialUsages");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskMaterialUsages",
                table: "TaskMaterialUsages",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskMaterialUsages_TaskID",
                table: "TaskMaterialUsages",
                column: "TaskID");
        }
    }
}
