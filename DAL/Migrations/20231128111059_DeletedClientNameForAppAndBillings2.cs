using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class DeletedClientNameForAppAndBillings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedClientName",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "DeletedClientName",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "DeletedClientId",
                table: "Billings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedClientId",
                table: "Appointments",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedClientId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "DeletedClientId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "DeletedClientName",
                table: "Billings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedClientName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
