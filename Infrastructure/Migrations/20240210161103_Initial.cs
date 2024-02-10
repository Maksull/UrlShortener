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
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urls", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Urls",
                columns: new[] { "Id", "Code", "CreatedAt", "ExpireAt", "OriginalUrl", "ShortenedUrl" },
                values: new object[,]
                {
                    { 1L, "PQ", new DateTime(2024, 2, 10, 16, 11, 3, 515, DateTimeKind.Utc).AddTicks(6717), new DateTime(2024, 2, 10, 16, 16, 3, 515, DateTimeKind.Utc).AddTicks(6719), "https://www.youtube.com/", "https://localhost:7167/PQ" },
                    { 2L, "Mn", new DateTime(2024, 2, 10, 16, 11, 3, 515, DateTimeKind.Utc).AddTicks(6749), new DateTime(2024, 2, 10, 16, 16, 3, 515, DateTimeKind.Utc).AddTicks(6750), "https://learn.microsoft.com/", "https://localhost:7167/Mn" },
                    { 3L, "wQ", new DateTime(2024, 2, 10, 16, 11, 3, 515, DateTimeKind.Utc).AddTicks(6775), new DateTime(2024, 2, 10, 16, 16, 3, 515, DateTimeKind.Utc).AddTicks(6776), "https://www.linkedin.com/feed/", "https://localhost:7167/wQ" }
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
