using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bekam.Infrastructure.Persistence._Data.Migrations
{
    /// <inheritdoc />
    public partial class addpictureUrlattributeinproductcategoryentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Categories");
        }
    }
}
