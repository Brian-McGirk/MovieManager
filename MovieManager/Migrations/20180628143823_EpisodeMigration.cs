using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieManager.Migrations
{
    public partial class EpisodeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Users_UserID",
                table: "Media");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Media",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "AirDate",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "EpisodeName",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "EpisodeNumber",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Seen",
                table: "Media");

            migrationBuilder.RenameTable(
                name: "Media",
                newName: "Medias");

            migrationBuilder.RenameIndex(
                name: "IX_Media_UserID",
                table: "Medias",
                newName: "IX_Medias_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medias",
                table: "Medias",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Episode",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AirDate = table.Column<string>(nullable: true),
                    EpisodeName = table.Column<string>(nullable: true),
                    EpisodeNumber = table.Column<int>(nullable: false),
                    MediaID = table.Column<int>(nullable: false),
                    Season = table.Column<int>(nullable: false),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episode", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Episode_Medias_MediaID",
                        column: x => x.MediaID,
                        principalTable: "Medias",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episode_MediaID",
                table: "Episode",
                column: "MediaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Users_UserID",
                table: "Medias",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Users_UserID",
                table: "Medias");

            migrationBuilder.DropTable(
                name: "Episode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medias",
                table: "Medias");

            migrationBuilder.RenameTable(
                name: "Medias",
                newName: "Media");

            migrationBuilder.RenameIndex(
                name: "IX_Medias_UserID",
                table: "Media",
                newName: "IX_Media_UserID");

            migrationBuilder.AddColumn<string>(
                name: "AirDate",
                table: "Media",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EpisodeName",
                table: "Media",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EpisodeNumber",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "Media",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Media",
                table: "Media",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Users_UserID",
                table: "Media",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
