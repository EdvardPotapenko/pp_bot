using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace pp_bot.Server.Migrations
{
    public partial class RemoveIdFieldForChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotUserChat_Chats_UserChatsId",
                table: "BotUserChat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotUserChat",
                table: "BotUserChat");

            migrationBuilder.DropIndex(
                name: "IX_BotUserChat_UserChatsId",
                table: "BotUserChat");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserChatsId",
                table: "BotUserChat");

            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "Chats",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "UserChatsChatId",
                table: "BotUserChat",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotUserChat",
                table: "BotUserChat",
                columns: new[] { "ChatUsersId", "UserChatsChatId" });

            migrationBuilder.CreateIndex(
                name: "IX_BotUserChat_UserChatsChatId",
                table: "BotUserChat",
                column: "UserChatsChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotUserChat_Chats_UserChatsChatId",
                table: "BotUserChat",
                column: "UserChatsChatId",
                principalTable: "Chats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotUserChat_Chats_UserChatsChatId",
                table: "BotUserChat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotUserChat",
                table: "BotUserChat");

            migrationBuilder.DropIndex(
                name: "IX_BotUserChat_UserChatsChatId",
                table: "BotUserChat");

            migrationBuilder.DropColumn(
                name: "UserChatsChatId",
                table: "BotUserChat");

            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "Chats",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Chats",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "UserChatsId",
                table: "BotUserChat",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotUserChat",
                table: "BotUserChat",
                columns: new[] { "ChatUsersId", "UserChatsId" });

            migrationBuilder.CreateIndex(
                name: "IX_BotUserChat_UserChatsId",
                table: "BotUserChat",
                column: "UserChatsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotUserChat_Chats_UserChatsId",
                table: "BotUserChat",
                column: "UserChatsId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
