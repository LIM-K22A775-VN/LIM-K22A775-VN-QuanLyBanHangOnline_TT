using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class add_otpAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountOtps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountOtps", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$KWISNE0AzBdt3QGOVZxp1.6HB1d5G5XV5RradhFdRtKf4BG4PwKAy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountOtps");

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$yBfLsaQKqBwqSYImI5Nju.YsouBOMYrCxFyQ.1nbfc4vzykH4Mppu");
        }
    }
}
