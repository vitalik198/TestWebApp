using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTestWebApp.Migrations
{
    public partial class useridtoname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Ads",
                newName: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Ads",
                newName: "UserId");
        }
    }
}
