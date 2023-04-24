using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryManagementService.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumberOfItems = table.Column<int>(type: "int", nullable: false),
                    InvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillingAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CourierId = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    CourierUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CourierFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryHostories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryHostories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryHostories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryHostories_OrderId",
                table: "DeliveryHostories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryHostories");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
