using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHangOnline.Migrations
{
    public partial class Import_ImPort_Detail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Import",
                columns: table => new
                {
                    IdImport = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdStaff = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import", x => x.IdImport);
                    table.ForeignKey(
                        name: "FK_Import_Staff_IdStaff",
                        column: x => x.IdStaff,
                        principalTable: "Staff",
                        principalColumn: "IdStaff",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$0OSlnLUTm58iRPX5TtKQPOz3Nci0Lo.hByP9/kFpbD89yIohVmtN2");

            migrationBuilder.CreateIndex(
                name: "IX_Import_IdStaff",
                table: "Import",
                column: "IdStaff");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetail_IdImport",
                table: "ImportDetail",
                column: "IdImport");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetail_IdSP",
                table: "ImportDetail",
                column: "IdSP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportDetail");

            migrationBuilder.DropTable(
                name: "Import");

            migrationBuilder.UpdateData(
                table: "Admin",
                keyColumn: "IdAdmin",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Q.tcSglTYh7zooehXXkj/u3O3bni0ylcfREq2hbODN.rvlEjd2PCK");
        }
    }
}
