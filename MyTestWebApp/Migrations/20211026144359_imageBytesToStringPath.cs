using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyTestWebApp.Migrations
{
    public partial class imageBytesToStringPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Ads",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6ce15a2d-75cb-4bf9-b351-078d3fa03f16", "135aa4d9-a8af-4ab8-845c-c1a0f33d1a3f", "user", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9a576a0a-8311-4f2a-83e3-c700853ed38f", "aae9b763-30ef-4e8e-883e-fa7427f080a5", "admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6ce15a2d-75cb-4bf9-b351-078d3fa03f16");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a576a0a-8311-4f2a-83e3-c700853ed38f");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Ads",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
