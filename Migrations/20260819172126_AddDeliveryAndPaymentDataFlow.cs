using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAndPaymentDataFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPaymentData",
                table: "UserPaymentData");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Deliveries",
                newName: "Status");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserPaymentData",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "Deliveries",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "Deliveries",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPaymentData",
                table: "UserPaymentData",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserPaymentData_UserId_LastFour_Address",
                table: "UserPaymentData",
                columns: new[] { "UserId", "LastFour", "Address" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPaymentData",
                table: "UserPaymentData");

            migrationBuilder.DropIndex(
                name: "IX_UserPaymentData_UserId_LastFour_Address",
                table: "UserPaymentData");

            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Deliveries",
                newName: "status");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserPaymentData",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Deliveries",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Deliveries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPaymentData",
                table: "UserPaymentData",
                columns: new[] { "UserId", "LastFour" });
        }
    }
}
