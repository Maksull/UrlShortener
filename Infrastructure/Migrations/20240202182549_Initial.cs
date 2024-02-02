using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Urls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalUrl = table.Column<string>(type: "text", nullable: false),
                    ShortenedUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urls", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Urls",
                columns: new[] { "Id", "CreatedAt", "OriginalUrl", "ShortenedUrl" },
                values: new object[,]
                {
                    { 1L, new DateTime(2024, 2, 2, 18, 25, 48, 934, DateTimeKind.Utc).AddTicks(9594), "https://www.youtube.com/", "PQ" },
                    { 2L, new DateTime(2024, 2, 2, 18, 25, 48, 934, DateTimeKind.Utc).AddTicks(9611), "https://learn.microsoft.com/", "Mn" },
                    { 3L, new DateTime(2024, 2, 2, 18, 25, 48, 934, DateTimeKind.Utc).AddTicks(9622), "https://www.linkedin.com/feed/", "wQ" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Urls_OriginalUrl",
                table: "Urls",
                column: "OriginalUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Urls_ShortenedUrl",
                table: "Urls",
                column: "ShortenedUrl",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Urls");
        }
    }
}
