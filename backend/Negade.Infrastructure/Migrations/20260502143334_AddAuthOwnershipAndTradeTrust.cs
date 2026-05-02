using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Negade.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthOwnershipAndTradeTrust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BuyerUserId",
                table: "Rfqs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierUserId",
                table: "Quotes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "BusinessProfiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CounterpartyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TradeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeHistory_BusinessProfiles_BusinessProfileId",
                        column: x => x.BusinessProfileId,
                        principalTable: "BusinessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    RaterUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeRatings_AppUsers_RaterUserId",
                        column: x => x.RaterUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TradeRatings_BusinessProfiles_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "BusinessProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rfqs_BuyerUserId",
                table: "Rfqs",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_SupplierUserId",
                table: "Quotes",
                column: "SupplierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessProfiles_OwnerUserId",
                table: "BusinessProfiles",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_PhoneNumber",
                table: "AppUsers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_BusinessProfileId",
                table: "TradeHistory",
                column: "BusinessProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeRatings_RaterUserId",
                table: "TradeRatings",
                column: "RaterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeRatings_SupplierId",
                table: "TradeRatings",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessProfiles_AppUsers_OwnerUserId",
                table: "BusinessProfiles",
                column: "OwnerUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_AppUsers_SupplierUserId",
                table: "Quotes",
                column: "SupplierUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rfqs_AppUsers_BuyerUserId",
                table: "Rfqs",
                column: "BuyerUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessProfiles_AppUsers_OwnerUserId",
                table: "BusinessProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_AppUsers_SupplierUserId",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Rfqs_AppUsers_BuyerUserId",
                table: "Rfqs");

            migrationBuilder.DropTable(
                name: "TradeHistory");

            migrationBuilder.DropTable(
                name: "TradeRatings");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_Rfqs_BuyerUserId",
                table: "Rfqs");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_SupplierUserId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_BusinessProfiles_OwnerUserId",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "BuyerUserId",
                table: "Rfqs");

            migrationBuilder.DropColumn(
                name: "SupplierUserId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "BusinessProfiles");
        }
    }
}
