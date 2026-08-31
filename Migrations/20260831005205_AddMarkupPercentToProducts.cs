using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddMarkupPercentToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MarkupPercent",
                table: "Products",
                type: "numeric(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkupPercent",
                table: "Products");
        }
    }
}
