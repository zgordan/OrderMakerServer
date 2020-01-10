using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class Policy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_policy",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_policy", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_policy_forms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_policy = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false),
                    create = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    edit_all = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    edit_group = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    edit_own = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    view_all = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    view_group = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    view_own = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    delete_all = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    delete_group = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    delete_own = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    change_owner = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    reviewer = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_policy_forms", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_forms_form",
                        column: x => x.mtd_form,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_forms_policy",
                        column: x => x.mtd_policy,
                        principalTable: "mtd_policy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_policy_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_policy = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_group = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_policy_group", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_groups_group",
                        column: x => x.mtd_group,
                        principalTable: "mtd_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_groups_policy",
                        column: x => x.mtd_policy,
                        principalTable: "mtd_policy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_policy_parts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_policy = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_part = table.Column<string>(type: "varchar(36)", nullable: false),
                    create = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    edit = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    view = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_policy_parts", x => x.id);
                    table.ForeignKey(
                        name: "fk_policy_part_part",
                        column: x => x.mtd_form_part,
                        principalTable: "mtd_form_part",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_policy_part_policy",
                        column: x => x.mtd_policy,
                        principalTable: "mtd_policy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy_forms",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_policy_forms_form_idx",
                table: "mtd_policy_forms",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "fk_policy_forms_policy_idx",
                table: "mtd_policy_forms",
                column: "mtd_policy");

            migrationBuilder.CreateIndex(
                name: "UNIQUE_FORM",
                table: "mtd_policy_forms",
                columns: new[] { "mtd_policy", "mtd_form" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy_group",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_policy_groups_group_idx",
                table: "mtd_policy_group",
                column: "mtd_group");

            migrationBuilder.CreateIndex(
                name: "fk_policy_group_policy_idx",
                table: "mtd_policy_group",
                column: "mtd_policy");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy_parts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_policy_part_part_idx",
                table: "mtd_policy_parts",
                column: "mtd_form_part");

            migrationBuilder.CreateIndex(
                name: "fk_policy_part_policy_idx",
                table: "mtd_policy_parts",
                column: "mtd_policy");

            migrationBuilder.CreateIndex(
                name: "UNIQUE_PART",
                table: "mtd_policy_parts",
                columns: new[] { "mtd_policy", "mtd_form_part" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_policy_forms");

            migrationBuilder.DropTable(
                name: "mtd_policy_group");

            migrationBuilder.DropTable(
                name: "mtd_policy_parts");

            migrationBuilder.DropTable(
                name: "mtd_policy");
        }
    }
}
