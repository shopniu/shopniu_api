using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopniu_api.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderTransactionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderTransactionId",
                table: "Transactions",
                type: "text",
                nullable: true);

            // Backfill: las transacciones creadas con el bug tenían el ID de
            // Wompi en TransactionReference. Se mueve a ProviderTransactionId
            // y se restaura la referencia del comercio (shopniu_<idempotencyKey>)
            // para que el webhook vuelva a resolverlas.
            migrationBuilder.Sql("""
                UPDATE "Transactions"
                SET "ProviderTransactionId" = "TransactionReference",
                    "TransactionReference" = 'shopniu_' || "IdempotencyKey"
                WHERE "ProviderTransactionId" IS NULL
                  AND "TransactionReference" IS NOT NULL
                  AND "TransactionReference" NOT LIKE 'shopniu\_%' ESCAPE '\';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderTransactionId",
                table: "Transactions");
        }
    }
}
