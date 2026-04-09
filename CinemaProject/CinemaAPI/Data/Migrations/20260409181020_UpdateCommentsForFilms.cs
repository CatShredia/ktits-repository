using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommentsForFilms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentsConversationId",
                table: "Films",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Films_CommentsConversationId",
                table: "Films",
                column: "CommentsConversationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Films_Conversations_CommentsConversationId",
                table: "Films",
                column: "CommentsConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Films_Conversations_CommentsConversationId",
                table: "Films");

            migrationBuilder.DropIndex(
                name: "IX_Films_CommentsConversationId",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "CommentsConversationId",
                table: "Films");
        }
    }
}
