using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peliculas.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Peliculas",
                newName: "Sinopsis");

            migrationBuilder.AddColumn<int>(
                name: "DuracionMinutos",
                table: "Peliculas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Poster",
                table: "Peliculas",
                type: "varbinary(max)",
                maxLength: 52428800,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Generos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Directores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidad",
                table: "Directores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Biografia",
                table: "Actores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Actores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionMinutos",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "Poster",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Generos");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Directores");

            migrationBuilder.DropColumn(
                name: "Nacionalidad",
                table: "Directores");

            migrationBuilder.DropColumn(
                name: "Biografia",
                table: "Actores");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Actores");

            migrationBuilder.RenameColumn(
                name: "Sinopsis",
                table: "Peliculas",
                newName: "Descripcion");
        }
    }
}
