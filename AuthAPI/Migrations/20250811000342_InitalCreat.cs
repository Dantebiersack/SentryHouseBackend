using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentryHouseBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitalCreat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CotizacionServicios_Servicios_ServicioId",
                table: "CotizacionServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_MateriasPrimas_Proveedores_ProveedorId",
                table: "MateriasPrimas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_AspNetUsers_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Cotizaciones_CotizacionId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_CotizacionId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas");

            migrationBuilder.AlterColumn<int>(
                name: "CotizacionId",
                table: "Ventas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Iva",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioBase",
                table: "Servicios",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiciosMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    CantidadRequerida = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosMateriales_MateriasPrimas_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "MateriasPrimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosMateriales_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VentasDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IvaPorcentaje = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Iva = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLinea = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentasDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentasDetalles_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentasDetalles_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_CotizacionId",
                table: "Ventas",
                column: "CotizacionId",
                unique: true,
                filter: "[CotizacionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Estado",
                table: "Ventas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsuarioId_FechaVenta",
                table: "Ventas",
                columns: new[] { "UsuarioId", "FechaVenta" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosMateriales_MateriaPrimaId",
                table: "ServiciosMateriales",
                column: "MateriaPrimaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosMateriales_ServicioId",
                table: "ServiciosMateriales",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_VentasDetalles_ServicioId",
                table: "VentasDetalles",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_VentasDetalles_VentaId",
                table: "VentasDetalles",
                column: "VentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CotizacionServicios_Servicios_ServicioId",
                table: "CotizacionServicios",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MateriasPrimas_Proveedores_ProveedorId",
                table: "MateriasPrimas",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_AspNetUsers_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Cotizaciones_CotizacionId",
                table: "Ventas",
                column: "CotizacionId",
                principalTable: "Cotizaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CotizacionServicios_Servicios_ServicioId",
                table: "CotizacionServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_MateriasPrimas_Proveedores_ProveedorId",
                table: "MateriasPrimas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_AspNetUsers_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Cotizaciones_CotizacionId",
                table: "Ventas");

            migrationBuilder.DropTable(
                name: "ServiciosMateriales");

            migrationBuilder.DropTable(
                name: "VentasDetalles");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_CotizacionId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_Estado",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_UsuarioId_FechaVenta",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Iva",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "PrecioBase",
                table: "Servicios");

            migrationBuilder.AlterColumn<int>(
                name: "CotizacionId",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_CotizacionId",
                table: "Ventas",
                column: "CotizacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_CotizacionServicios_Servicios_ServicioId",
                table: "CotizacionServicios",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MateriasPrimas_Proveedores_ProveedorId",
                table: "MateriasPrimas",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_AspNetUsers_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Cotizaciones_CotizacionId",
                table: "Ventas",
                column: "CotizacionId",
                principalTable: "Cotizaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
