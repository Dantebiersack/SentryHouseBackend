using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentryHouseBackend.Migrations
{
    /// <inheritdoc />
    public partial class IdClienteEnCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Cotizaciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_UsuarioId",
                table: "Cotizaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_AspNetUsers_UsuarioId",
                table: "Cotizaciones",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_AspNetUsers_UsuarioId",
                table: "Cotizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_UsuarioId",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Cotizaciones");
        }
    }
}
