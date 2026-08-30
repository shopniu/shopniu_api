using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSourcingCertifiedOriginalAndSupplierFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CertifiedOriginal",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadTimeDays",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sourcing",
                table: "Products",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "LocalStock");

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertifiedOriginal",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LeadTimeDays",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Sourcing",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "Products");
        }
    }
}
