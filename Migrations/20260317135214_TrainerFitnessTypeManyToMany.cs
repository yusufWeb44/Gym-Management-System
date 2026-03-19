using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class TrainerFitnessTypeManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_FitnessTypes_FitnessTypeId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_FitnessTypeId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "FitnessTypeId",
                table: "Trainers");

            migrationBuilder.CreateTable(
                name: "TrainerFitnessType",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    FitnessTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerFitnessType", x => new { x.TrainerId, x.FitnessTypeId });
                    table.ForeignKey(
                        name: "FK_TrainerFitnessType_FitnessTypes_FitnessTypeId",
                        column: x => x.FitnessTypeId,
                        principalTable: "FitnessTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainerFitnessType_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TrainerFitnessType",
                columns: new[] { "FitnessTypeId", "TrainerId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerFitnessType_FitnessTypeId",
                table: "TrainerFitnessType",
                column: "FitnessTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerFitnessType");

            migrationBuilder.AddColumn<int>(
                name: "FitnessTypeId",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 1,
                column: "FitnessTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 2,
                column: "FitnessTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 3,
                column: "FitnessTypeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 4,
                column: "FitnessTypeId",
                value: 4);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_FitnessTypeId",
                table: "Trainers",
                column: "FitnessTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_FitnessTypes_FitnessTypeId",
                table: "Trainers",
                column: "FitnessTypeId",
                principalTable: "FitnessTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
