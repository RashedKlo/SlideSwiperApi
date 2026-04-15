using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Title1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title3Part1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title3Part2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title3Part3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Title4 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slides_Order",
                table: "Slides",
                column: "Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slides");
        }
    }
}
