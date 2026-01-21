using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class InitReviewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_IdUser",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "StartTB",
                table: "ProductDetail");

            migrationBuilder.AddColumn<int>(
                name: "IdDH",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$8EE5.NbpGqJ2jivTr8UpMeMmMpNFV/lZcOnYimazWtDvny8Yxa97.");

            migrationBuilder.CreateIndex(
                name: "IX_Review_IdDH",
                table: "Review",
                column: "IdDH");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Order_IdDH",
                table: "Review",
                column: "IdDH",
                principalTable: "Order",
                principalColumn: "IdDH",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_IdUser",
                table: "Review",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Order_IdDH",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_IdUser",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_IdDH",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "IdDH",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Product");

            migrationBuilder.AddColumn<decimal>(
                name: "StartTB",
                table: "ProductDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$KWISNE0AzBdt3QGOVZxp1.6HB1d5G5XV5RradhFdRtKf4BG4PwKAy");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_IdUser",
                table: "Review",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
