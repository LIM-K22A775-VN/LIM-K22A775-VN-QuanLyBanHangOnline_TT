using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class updatereview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Order_IdDH",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_IdDH",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "IdDH",
                table: "Review");

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Zzj6GX2whISM4O8rQk81heNECB49oBEV1HvetQF4pxH2Fom/prDtS");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdDH",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
