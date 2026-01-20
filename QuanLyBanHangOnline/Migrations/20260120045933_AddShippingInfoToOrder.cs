using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class AddShippingInfoToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Staff",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "OrderNotes",
                table: "Order",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Order",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhone",
                table: "Order",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "IdAdmin", "Email", "Password", "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { 1, "admin99@gmail.com", "$2a$11$X0EdV9k5SP9G86eMqNHqlOlB26i4gnQtJwQ0bEczjyrBwXCXiL1sC", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "OrderNotes",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ReceiverPhone",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "IdAdmin", "Email", "Password", "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { 10, "admin99@gmail.com", "$2a$11$GTnD.LCRR9LXqAtS.Pjf/.PaH37aWzzFOZqJrNzH6kFavP3bFuUlm", null, null });
        }
    }
}
