using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveChatProject.Migrations
{
    /// <inheritdoc />
    public partial class relatiionship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ChatCommunications_AgentId",
                table: "ChatCommunications",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatCommunications_UserId",
                table: "ChatCommunications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatCommunications_Agents_AgentId",
                table: "ChatCommunications",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "AgentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatCommunications_Users_UserId",
                table: "ChatCommunications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatCommunications_Agents_AgentId",
                table: "ChatCommunications");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatCommunications_Users_UserId",
                table: "ChatCommunications");

            migrationBuilder.DropIndex(
                name: "IX_ChatCommunications_AgentId",
                table: "ChatCommunications");

            migrationBuilder.DropIndex(
                name: "IX_ChatCommunications_UserId",
                table: "ChatCommunications");
        }
    }
}
