using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryFieldsToUserPaymentData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserPaymentData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "UserPaymentData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "UserPaymentData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "UserPaymentData",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "UserPaymentData");

            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "UserPaymentData");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "UserPaymentData");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "UserPaymentData");
        }
    }
}
