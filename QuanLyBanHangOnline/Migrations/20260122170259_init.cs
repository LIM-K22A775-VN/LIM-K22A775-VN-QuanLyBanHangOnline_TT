using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    IdAccount = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.IdAccount);
                    table.UniqueConstraint("AK_Accounts_Email", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    IdSP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.IdSP);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountOtps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountOtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountOtps_Accounts_Email",
                        column: x => x.Email,
                        principalTable: "Accounts",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    IdAccount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.IdAccount);
                    table.ForeignKey(
                        name: "FK_Admins_Accounts_IdAccount",
                        column: x => x.IdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                });

            migrationBuilder.CreateTable(
                name: "Import",
                columns: table => new
                {
                    IdImport = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdAccount = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import", x => x.IdImport);
                    table.ForeignKey(
                        name: "FK_Import_Accounts_IdAccount",
                        column: x => x.IdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdAccount = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdAccount);
                    table.ForeignKey(
                        name: "FK_Users_Accounts_IdAccount",
                        column: x => x.IdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                });

            migrationBuilder.CreateTable(
                name: "ProductDetail",
                columns: table => new
                {
                    IdSP = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetail", x => x.IdSP);
                    table.ForeignKey(
                        name: "FK_ProductDetail_Product_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Product",
                        principalColumn: "IdSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    IdAccount = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.IdAccount);
                    table.ForeignKey(
                        name: "FK_Staffs_Accounts_IdAccount",
                        column: x => x.IdAccount,
                        principalTable: "Accounts",
                        principalColumn: "IdAccount");
                    table.ForeignKey(
                        name: "FK_Staffs_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImportDetail",
                columns: table => new
                {
                    IdImportDetail = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImport = table.Column<int>(type: "int", nullable: false),
                    IdSP = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImportPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportDetail", x => x.IdImportDetail);
                    table.ForeignKey(
                        name: "FK_ImportDetail_Import_IdImport",
                        column: x => x.IdImport,
                        principalTable: "Import",
                        principalColumn: "IdImport",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportDetail_Product_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Product",
                        principalColumn: "IdSP",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_Cart_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    IdDH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.IdDH);
                    table.ForeignKey(
                        name: "FK_Order_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    IdReview = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdSP = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.IdReview);
                    table.ForeignKey(
                        name: "FK_Review_Product_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Product",
                        principalColumn: "IdSP",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdAccount",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    IdOrderDetail = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDH = table.Column<int>(type: "int", nullable: false),
                    IdSP = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.IdOrderDetail);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_IdDH",
                        column: x => x.IdDH,
                        principalTable: "Order",
                        principalColumn: "IdDH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Product",
                        principalColumn: "IdSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "IdAccount", "Email", "Password", "RefreshToken", "RefreshTokenExpiryTime", "RoleType" },
                values: new object[,]
                {
                    { 1, "admin@gmail.com", "$2a$11$BbiFgNGQCSSOxZH6x1m8uOm.TrKLWR8NAuGHH6JGBsUIQwwSO.gtK", null, null, "Admin" },
                    { 2, "staff@gmail.com", "$2a$11$8HPqtd5cLUgcyBSXkDkL2OAyiy1nzpCyvY/Jd6M.L9FW4WOOp4Q5q", null, null, "Staff" },
                    { 3, "user@gmail.com", "$2a$11$7OdFyxZMhViCzGGSRW3ob.l/QkS2rqtO5R6fEPmnDWJOGuxYjNYrC", null, null, "User" },
                    { 4, "nxtql99@gmail.com", "$2a$11$.kJyjusLlOVTnwgr/8tjMepHgAJzHAef2DviSM5dkf7Qli4J8evcu", null, null, "User" },
                    { 5, "ndt@gmail.com", "$2a$11$Oj93lz3afb4lApk4kZ66cuOD9caTy/.JQDMBQBQ76b1B04/OdBykO", null, null, "User" }
                });

            migrationBuilder.InsertData(
                table: "Admins",
                column: "IdAccount",
                value: 1);

            migrationBuilder.InsertData(
                table: "Staffs",
                columns: new[] { "IdAccount", "Address", "FullName", "Phone", "RoleId", "Salary" },
                values: new object[] { 2, null, null, null, null, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "IdAccount", "Address", "FullName", "Phone" },
                values: new object[,]
                {
                    { 3, null, null, null },
                    { 4, null, null, null },
                    { 5, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountOtps_Email",
                table: "AccountOtps",
                column: "Email");

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

            migrationBuilder.CreateIndex(
                name: "IX_Import_IdAccount",
                table: "Import",
                column: "IdAccount");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetail_IdImport",
                table: "ImportDetail",
                column: "IdImport");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetail_IdSP",
                table: "ImportDetail",
                column: "IdSP");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IdUser",
                table: "Order",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_IdDH",
                table: "OrderDetail",
                column: "IdDH");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_IdSP",
                table: "OrderDetail",
                column: "IdSP");

            migrationBuilder.CreateIndex(
                name: "IX_Review_IdSP",
                table: "Review",
                column: "IdSP");

            migrationBuilder.CreateIndex(
                name: "IX_Review_IdUser",
                table: "Review",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_RoleId",
                table: "Staffs",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountOtps");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "CartDetail");

            migrationBuilder.DropTable(
                name: "ImportDetail");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ProductDetail");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Import");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
