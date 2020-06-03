using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdatePolicyFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from mtd_filter_script where id>0");

            migrationBuilder.DropForeignKey(
                name: "fk_script_filter",
                table: "mtd_filter_script");

            migrationBuilder.DropIndex(
                name: "fk_policy_part_policy_idx",
                table: "mtd_policy_parts");

            migrationBuilder.DropIndex(
                name: "fk_script_filter_idx",
                table: "mtd_filter_script");

            migrationBuilder.DropColumn(
                name: "description",
                table: "mtd_filter_script");

            migrationBuilder.DropColumn(
                name: "mtd_filter",
                table: "mtd_filter_script");

            migrationBuilder.RenameIndex(
                name: "fk_policy_part_part_idx",
                table: "mtd_policy_parts",
                newName: "IX_mtd_policy_parts_mtd_form_part");

            migrationBuilder.AddColumn<string>(
                name: "mtd_form_id",
                table: "mtd_filter_script",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "mtd_filter_owner",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false),
                    owner_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_owner", x => x.id);
                    table.ForeignKey(
                        name: "fk_owner_filter",
                        column: x => x.id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_policy_scripts",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_policy_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_filter_script_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_policy_scripts", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_filter",
                        column: x => x.mtd_filter_script_id,
                        principalTable: "mtd_filter_script",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_script",
                        column: x => x.mtd_policy_id,
                        principalTable: "mtd_policy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_register",
                table: "mtd_store_link",
                column: "Register");

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_idx",
                table: "mtd_filter_script",
                column: "mtd_form_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_owner",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Unique_id",
                table: "mtd_policy_scripts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mtd_policy_scripts_mtd_policy_id",
                table: "mtd_policy_scripts",
                column: "mtd_policy_id");

            migrationBuilder.CreateIndex(
                name: "Unique_Policy_Script",
                table: "mtd_policy_scripts",
                columns: new[] { "mtd_filter_script_id", "mtd_policy_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_script_filter",
                table: "mtd_filter_script",
                column: "mtd_form_id",
                principalTable: "mtd_form",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_script_filter",
                table: "mtd_filter_script");

            migrationBuilder.DropTable(
                name: "mtd_filter_owner");

            migrationBuilder.DropTable(
                name: "mtd_policy_scripts");

            migrationBuilder.DropIndex(
                name: "ix_register",
                table: "mtd_store_link");

            migrationBuilder.DropIndex(
                name: "fk_script_filter_idx",
                table: "mtd_filter_script");

            migrationBuilder.DropColumn(
                name: "mtd_form_id",
                table: "mtd_filter_script");

            migrationBuilder.RenameIndex(
                name: "IX_mtd_policy_parts_mtd_form_part",
                table: "mtd_policy_parts",
                newName: "fk_policy_part_part_idx");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "mtd_filter_script",
                type: "varchar(512)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "mtd_filter",
                table: "mtd_filter_script",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "fk_policy_part_policy_idx",
                table: "mtd_policy_parts",
                column: "mtd_policy");

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_idx",
                table: "mtd_filter_script",
                column: "mtd_filter");

            migrationBuilder.AddForeignKey(
                name: "fk_script_filter",
                table: "mtd_filter_script",
                column: "mtd_filter",
                principalTable: "mtd_filter",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
