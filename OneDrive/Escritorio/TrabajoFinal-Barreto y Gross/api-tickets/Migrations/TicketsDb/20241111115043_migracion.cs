using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_biblioteca.Migrations.TicketsDb
{
    /// <inheritdoc />
    public partial class migracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TareaId",
                table: "Estados",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_TareaId",
                table: "Comentarios",
                column: "TareaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Tareas_TareaId",
                table: "Comentarios",
                column: "TareaId",
                principalTable: "Tareas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Tareas_TareaId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_TareaId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "TareaId",
                table: "Estados");
        }
    }
}
