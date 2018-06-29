using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieManager.Migrations
{
    public partial class NewEpisodeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episode_Medias_MediaID",
                table: "Episode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Episode",
                table: "Episode");

            migrationBuilder.RenameTable(
                name: "Episode",
                newName: "Episodes");

            migrationBuilder.RenameIndex(
                name: "IX_Episode_MediaID",
                table: "Episodes",
                newName: "IX_Episodes_MediaID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Episodes",
                table: "Episodes",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Medias_MediaID",
                table: "Episodes",
                column: "MediaID",
                principalTable: "Medias",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Medias_MediaID",
                table: "Episodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Episodes",
                table: "Episodes");

            migrationBuilder.RenameTable(
                name: "Episodes",
                newName: "Episode");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_MediaID",
                table: "Episode",
                newName: "IX_Episode_MediaID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Episode",
                table: "Episode",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Episode_Medias_MediaID",
                table: "Episode",
                column: "MediaID",
                principalTable: "Medias",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
