using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class update_product_Cart_Cart_Detail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceOriginal",
                table: "Product");

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    IdCart = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.IdCart);
                    table.ForeignKey(
                        name: "FK_Cart_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartDetail",
                columns: table => new
                {
                    IdCartDetail = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCart = table.Column<int>(type: "int", nullable: false),
                    IdSP = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetail", x => x.IdCartDetail);
                    table.ForeignKey(
                        name: "FK_CartDetail_Cart_IdCart",
                        column: x => x.IdCart,
                        principalTable: "Cart",
                        principalColumn: "IdCart",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartDetail_Product_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Product",
                        principalColumn: "IdSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Q.tcSglTYh7zooehXXkj/u3O3bni0ylcfREq2hbODN.rvlEjd2PCK");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_IdUser",
                table: "Cart",
                column: "IdUser",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_IdCart",
                table: "CartDetail",
                column: "IdCart");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_IdSP",
                table: "CartDetail",
                column: "IdSP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDetail");

            migrationBuilder.DropTable(
                name: "Cart");

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
    }
}
