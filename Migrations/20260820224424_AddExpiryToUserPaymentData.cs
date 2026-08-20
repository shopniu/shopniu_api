using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiryToUserPaymentData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpMonth",
                table: "UserPaymentData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpYear",
                table: "UserPaymentData",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpMonth",
                table: "UserPaymentData");

            migrationBuilder.DropColumn(
                name: "ExpYear",
                table: "UserPaymentData");
        }
    }
}
