using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnder.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistToSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Songs",
                newName: "YouTubeUrl");

            migrationBuilder.AddColumn<string>(
                name: "Artist",
                table: "Songs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Playlists",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artist",
                table: "Songs");

            migrationBuilder.RenameColumn(
                name: "YouTubeUrl",
                table: "Songs",
                newName: "FilePath");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Playlists",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
