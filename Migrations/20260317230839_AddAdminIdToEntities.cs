using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminIdToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "FitnessTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Role" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "seeded@gym.com", "Seeded Admin", true, "$2a$11$I1qhKIZ.3yL5udUg3kS.PusCFLP4el7tC5/NkVekvvKhfE0pl7w7a", "Admin" });

            migrationBuilder.UpdateData(
                table: "FitnessTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FitnessTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FitnessTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "FitnessTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 3,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 4,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 3,
                column: "AdminId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 4,
                column: "AdminId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_AdminId",
                table: "Trainers",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_AdminId",
                table: "Plans",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_AdminId",
                table: "Members",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_FitnessTypes_AdminId",
                table: "FitnessTypes",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_FitnessTypes_Admins_AdminId",
                table: "FitnessTypes",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Admins_AdminId",
                table: "Members",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Admins_AdminId",
                table: "Plans",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Admins_AdminId",
                table: "Trainers",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FitnessTypes_Admins_AdminId",
                table: "FitnessTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Admins_AdminId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Admins_AdminId",
                table: "Plans");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Admins_AdminId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_AdminId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Plans_AdminId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Members_AdminId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_FitnessTypes_AdminId",
                table: "FitnessTypes");

            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "FitnessTypes");
        }
    }
}
