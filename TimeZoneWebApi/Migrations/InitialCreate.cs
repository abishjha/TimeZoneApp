using System;
using Microsoft.EntityFrameworkCore.Migrations;
using TimeZoneWebApi.Models;

namespace TimeZoneWebApi.Migrations.SqlServerMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            AddDefaultAdmin(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    DifferenceToGMT = table.Column<double>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeZones_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                        );
                });
        }

        protected void AddDefaultAdmin(MigrationBuilder migrationBuilder)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512();
            byte[] passwordSalt = hmac.Key;
            byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("admin"));

            migrationBuilder.InsertData(
                table: "Users",
                new string[] {
                    "FirstName",
                    "LastName",
                    "Username",
                    "Role",
                    "PasswordHash",
                    "PasswordSalt"
                },
                new object[] {
                    "admin",
                    "",
                    "admin",
                    Roles.ROLE_ADMIN,
                    passwordHash,
                    passwordSalt
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
            migrationBuilder.DropTable(
                name: "TimeZones");
        }
    }
}
