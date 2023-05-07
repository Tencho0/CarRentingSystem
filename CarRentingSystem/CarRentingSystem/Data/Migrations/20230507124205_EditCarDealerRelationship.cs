using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentingSystem.Data.Migrations
{
    public partial class EditCarDealerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Dealers_DealerId1",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_DealerId1",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DealerId1",
                table: "Cars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DealerId1",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_DealerId1",
                table: "Cars",
                column: "DealerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Dealers_DealerId1",
                table: "Cars",
                column: "DealerId1",
                principalTable: "Dealers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
