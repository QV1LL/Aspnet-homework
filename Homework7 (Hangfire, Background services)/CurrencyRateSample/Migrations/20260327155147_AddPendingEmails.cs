using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyRateSample.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pending_emails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    to = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    subject = table.Column<string>(type: "TEXT", maxLength: 998, nullable: false),
                    body = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_sent = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retry_count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_emails", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pending_emails_created_at",
                table: "pending_emails",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_pending_emails_is_sent",
                table: "pending_emails",
                column: "is_sent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pending_emails");
        }
    }
}
