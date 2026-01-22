using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class updateProduct_GIA_Mua : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceOriginal",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$QkHFI62z7Koevtmdq9uGt.UiXx6cxrTOVXe2nHz2NdNMylkmzKHW6");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceOriginal",
                table: "Product");

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Zzj6GX2whISM4O8rQk81heNECB49oBEV1HvetQF4pxH2Fom/prDtS");
        }
    }
}
