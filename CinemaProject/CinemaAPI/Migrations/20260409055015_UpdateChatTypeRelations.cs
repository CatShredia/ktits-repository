using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CinemaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChatTypeRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Conversations",
                newName: "ConversationTypeId");

            migrationBuilder.CreateTable(
                name: "ConversationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ConversationTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Direct" },
                    { 2, "Group" },
                    { 3, "Channel" },
                    { 4, "Comments" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ConversationTypeId",
                table: "Conversations",
                column: "ConversationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationTypes_Name",
                table: "ConversationTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ConversationTypes_ConversationTypeId",
                table: "Conversations",
                column: "ConversationTypeId",
                principalTable: "ConversationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConversationTypes_ConversationTypeId",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "ConversationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ConversationTypeId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ConversationTypeId",
                table: "Conversations",
                newName: "Type");
        }
    }
}
