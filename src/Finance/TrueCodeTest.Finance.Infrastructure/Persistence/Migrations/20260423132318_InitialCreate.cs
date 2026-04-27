using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrueCodeTest.Finance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currency",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    char_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    nominal = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currency", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_favorite",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorite", x => new { x.user_id, x.currency_id });
                    table.ForeignKey(
                        name: "FK_user_favorite_currency_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currency",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_currency_char_code",
                table: "currency",
                column: "char_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_currency_id",
                table: "user_favorite",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_user_id",
                table: "user_favorite",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_favorite");

            migrationBuilder.DropTable(
                name: "currency");
        }
    }
}
