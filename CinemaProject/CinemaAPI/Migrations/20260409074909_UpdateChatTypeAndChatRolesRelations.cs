using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CinemaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChatTypeAndChatRolesRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Conversations",
                newName: "ConversationTypeId");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "ConversationParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConversationRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationRoles", x => x.Id);
                });

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
                table: "ConversationRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "Admin" },
                    { 3, "Moderator" },
                    { 4, "Member" }
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
                name: "IX_ConversationParticipants_RoleId",
                table: "ConversationParticipants",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationRoles_Name",
                table: "ConversationRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationTypes_Name",
                table: "ConversationTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_ConversationRoles_RoleId",
                table: "ConversationParticipants",
                column: "RoleId",
                principalTable: "ConversationRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_ConversationParticipants_ConversationRoles_RoleId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConversationTypes_ConversationTypeId",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "ConversationRoles");

            migrationBuilder.DropTable(
                name: "ConversationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ConversationTypeId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_RoleId",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ConversationParticipants");

            migrationBuilder.RenameColumn(
                name: "ConversationTypeId",
                table: "Conversations",
                newName: "Type");
        }
    }
}
