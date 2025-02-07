using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseAndMasterpiece.Data.Migrations
{
    /// <inheritdoc />
    public partial class artistxartwork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArtistId",
                table: "Artworks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_ArtistId",
                table: "Artworks",
                column: "ArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artworks_Artists_ArtistId",
                table: "Artworks",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "ArtistId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artworks_Artists_ArtistId",
                table: "Artworks");

            migrationBuilder.DropIndex(
                name: "IX_Artworks_ArtistId",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "ArtistId",
                table: "Artworks");
        }
    }
}
