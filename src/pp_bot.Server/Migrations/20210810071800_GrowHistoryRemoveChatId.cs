using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pp_bot.Server.Migrations
{
    public partial class GrowHistoryRemoveChatId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GrowHistories",
                table: "GrowHistories");

            migrationBuilder.DropColumn(
                name: "UserChatId",
                table: "GrowHistories");

            migrationBuilder.AlterColumn<long>(
                name: "GrowHistoryId",
                table: "GrowHistories",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrowHistories",
                table: "GrowHistories",
                column: "GrowHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GrowHistories",
                table: "GrowHistories");

            migrationBuilder.AlterColumn<long>(
                name: "GrowHistoryId",
                table: "GrowHistories",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "UserChatId",
                table: "GrowHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrowHistories",
                table: "GrowHistories",
                columns: new[] { "GrowHistoryId", "UserChatId" });
        }
    }
}
