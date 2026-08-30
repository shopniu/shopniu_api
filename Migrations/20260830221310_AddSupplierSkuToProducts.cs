using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierSkuToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "SupplierSku",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId_SupplierSku",
                table: "Products",
                columns: new[] { "SupplierId", "SupplierSku" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierId_SupplierSku",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierSku",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");
        }
    }
}
