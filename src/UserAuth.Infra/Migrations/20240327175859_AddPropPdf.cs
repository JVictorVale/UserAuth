using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAuth.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPropPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pdf",
                table: "Usuarios",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pdf",
                table: "Usuarios");
        }
    }
}
