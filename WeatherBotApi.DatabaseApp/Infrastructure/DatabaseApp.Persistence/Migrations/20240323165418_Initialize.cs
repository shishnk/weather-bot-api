using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    USERNAME = table.Column<string>(type: "text", nullable: false),
                    MOBILE_NUMBER = table.Column<string>(type: "text", nullable: false),
                    TIMESTAMP = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WEATHER_DESCRIPTIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LOCATION = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WEATHER_DESCRIPTIONS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WEATHER_SUBSCRIPTIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WEATHER_DESCRIPTION_ID = table.Column<int>(type: "integer", nullable: false),
                    USER_ID = table.Column<int>(type: "integer", nullable: false),
                    RESEND_INTERVAL = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WEATHER_SUBSCRIPTIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WEATHER_DESCRIPTION_ID",
                        column: x => x.WEATHER_DESCRIPTION_ID,
                        principalTable: "WEATHER_DESCRIPTIONS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WEATHER_SUBSCRIPTIONS_USER_ID",
                table: "WEATHER_SUBSCRIPTIONS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WEATHER_SUBSCRIPTIONS_WEATHER_DESCRIPTION_ID",
                table: "WEATHER_SUBSCRIPTIONS",
                column: "WEATHER_DESCRIPTION_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WEATHER_SUBSCRIPTIONS");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "WEATHER_DESCRIPTIONS");
        }
    }
}
