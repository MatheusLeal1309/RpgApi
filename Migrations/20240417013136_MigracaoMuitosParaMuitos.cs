using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RpgApi.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoMuitosParaMuitos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_HABILIDADES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "Varchar(200)", maxLength: 200, nullable: false),
                    Dano = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_HABILIDADES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_PERSONAGENS_HABILIDADES",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(type: "int", nullable: false),
                    HabilidadeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PERSONAGENS_HABILIDADES", x => new { x.PersonagemId, x.HabilidadeId });
                    table.ForeignKey(
                        name: "FK_TB_PERSONAGENS_HABILIDADES_TB_HABILIDADES_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "TB_HABILIDADES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_PERSONAGENS_HABILIDADES_TB_PERSONAGENS_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "TB_PERSONAGENS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TB_HABILIDADES",
                columns: new[] { "Id", "Dano", "Nome" },
                values: new object[,]
                {
                    { 1, 39, "Adormecer" },
                    { 2, 41, "COngelar" },
                    { 3, 37, "Adormecer" }
                });

            migrationBuilder.UpdateData(
                table: "TB_USUARIOS",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: new byte[] { 184, 184, 167, 12, 0, 225, 37, 173, 185, 185, 213, 201, 142, 156, 6, 62, 141, 138, 48, 206, 81, 119, 181, 65, 244, 12, 14, 186, 163, 95, 63, 4, 200, 29, 228, 9, 110, 238, 51, 62, 65, 5, 137, 99, 106, 63, 156, 6, 49, 17, 23, 234, 224, 60, 35, 142, 90, 101, 192, 110, 215, 88, 169, 53, 85, 151, 212, 80, 177, 36, 59, 112, 97, 208, 202, 225, 58, 187, 158, 30, 83, 199, 253, 251, 53, 94, 136, 218, 34, 76, 201, 172, 171, 142, 46, 255, 129, 139, 224, 22, 187, 236, 218, 28, 136, 126, 89, 89, 42, 99, 45, 253, 32, 195, 120, 23, 234, 205, 184, 142, 99, 132, 152, 72, 60, 77, 123, 40 });

            migrationBuilder.InsertData(
                table: "TB_PERSONAGENS_HABILIDADES",
                columns: new[] { "HabilidadeId", "PersonagemId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 3, 3 },
                    { 3, 4 },
                    { 1, 5 },
                    { 2, 6 },
                    { 3, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_PERSONAGENS_HABILIDADES_HabilidadeId",
                table: "TB_PERSONAGENS_HABILIDADES",
                column: "HabilidadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_PERSONAGENS_HABILIDADES");

            migrationBuilder.DropTable(
                name: "TB_HABILIDADES");

            migrationBuilder.UpdateData(
                table: "TB_USUARIOS",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: new byte[] { 208, 122, 225, 185, 62, 211, 247, 46, 161, 195, 14, 248, 57, 132, 111, 216, 251, 223, 222, 75, 187, 196, 22, 249, 133, 107, 142, 20, 177, 251, 151, 112, 190, 181, 118, 106, 120, 6, 12, 65, 178, 253, 149, 136, 207, 92, 44, 172, 14, 52, 163, 225, 172, 107, 248, 224, 97, 205, 45, 144, 97, 109, 192, 57, 202, 99, 123, 183, 88, 153, 116, 129, 43, 163, 129, 42, 175, 97, 132, 55, 50, 18, 233, 169, 49, 189, 53, 75, 18, 91, 53, 51, 177, 28, 14, 157, 111, 216, 183, 124, 25, 161, 58, 162, 108, 79, 238, 64, 197, 141, 24, 114, 124, 132, 67, 213, 7, 148, 175, 142, 53, 239, 211, 234, 19, 71, 70, 140 });
        }
    }
}
