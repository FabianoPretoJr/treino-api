using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace projeto.Migrations
{
    public partial class AdicionandoEntidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "criminosos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    CPF = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_criminosos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "delegacias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Endereco = table.Column<string>(nullable: true),
                    Telefone = table.Column<string>(nullable: true),
                    Batalhao = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegacias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "delegados",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    CPF = table.Column<string>(nullable: true),
                    Funcional = table.Column<string>(nullable: true),
                    Turno = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "legistas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    CRM = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legistas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "policiais",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    Funcional = table.Column<string>(nullable: true),
                    Patente = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policiais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vitimas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    CPF = table.Column<string>(nullable: true),
                    Idade = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vitimas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "autopsias",
                columns: table => new
                {
                    LegistaID = table.Column<int>(nullable: false),
                    VitimaID = table.Column<int>(nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    Laudo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_autopsias", x => new { x.VitimaID, x.LegistaID });
                    table.ForeignKey(
                        name: "FK_autopsias_legistas_LegistaID",
                        column: x => x.LegistaID,
                        principalTable: "legistas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_autopsias_vitimas_VitimaID",
                        column: x => x.VitimaID,
                        principalTable: "vitimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "crimes",
                columns: table => new
                {
                    CriminosoID = table.Column<int>(nullable: false),
                    VitimaID = table.Column<int>(nullable: false),
                    PolicialID = table.Column<int>(nullable: false),
                    Descricao = table.Column<string>(nullable: true),
                    Data = table.Column<DateTime>(nullable: false),
                    DelegaciaId = table.Column<int>(nullable: true),
                    DelegadoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crimes", x => new { x.CriminosoID, x.VitimaID, x.PolicialID });
                    table.ForeignKey(
                        name: "FK_crimes_criminosos_CriminosoID",
                        column: x => x.CriminosoID,
                        principalTable: "criminosos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_crimes_delegacias_DelegaciaId",
                        column: x => x.DelegaciaId,
                        principalTable: "delegacias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_crimes_delegados_DelegadoId",
                        column: x => x.DelegadoId,
                        principalTable: "delegados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_crimes_policiais_PolicialID",
                        column: x => x.PolicialID,
                        principalTable: "policiais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_crimes_vitimas_VitimaID",
                        column: x => x.VitimaID,
                        principalTable: "vitimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_autopsias_LegistaID",
                table: "autopsias",
                column: "LegistaID");

            migrationBuilder.CreateIndex(
                name: "IX_crimes_DelegaciaId",
                table: "crimes",
                column: "DelegaciaId");

            migrationBuilder.CreateIndex(
                name: "IX_crimes_DelegadoId",
                table: "crimes",
                column: "DelegadoId");

            migrationBuilder.CreateIndex(
                name: "IX_crimes_PolicialID",
                table: "crimes",
                column: "PolicialID");

            migrationBuilder.CreateIndex(
                name: "IX_crimes_VitimaID",
                table: "crimes",
                column: "VitimaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "autopsias");

            migrationBuilder.DropTable(
                name: "crimes");

            migrationBuilder.DropTable(
                name: "legistas");

            migrationBuilder.DropTable(
                name: "criminosos");

            migrationBuilder.DropTable(
                name: "delegacias");

            migrationBuilder.DropTable(
                name: "delegados");

            migrationBuilder.DropTable(
                name: "policiais");

            migrationBuilder.DropTable(
                name: "vitimas");
        }
    }
}
