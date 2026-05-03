using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tech_challenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriadoCamposCodigoEAjusteOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Servico",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "PecaInsumo",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FimExecucao",
                table: "OrdemServico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InicioExecucao",
                table: "OrdemServico",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Servico");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "PecaInsumo");

            migrationBuilder.DropColumn(
                name: "FimExecucao",
                table: "OrdemServico");

            migrationBuilder.DropColumn(
                name: "InicioExecucao",
                table: "OrdemServico");
        }
    }
}
