using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class UpdateActivites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_form_activity",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    image = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    image_type = table.Column<string>(type: "varchar(256)", nullable: true),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_activity", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_form_activity",
                        column: x => x.mtd_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_activity",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_store_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_activity_id = table.Column<string>(type: "varchar(36)", nullable: true),
                    app_comment = table.Column<string>(type: "varchar(512)", nullable: false),
                    timecr = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_activity", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_activity_idx",
                        column: x => x.mtd_form_activity_id,
                        principalTable: "mtd_form_activity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_store_activity_store",
                        column: x => x.mtd_store_id,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_activity",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_activity_idx",
                table: "mtd_form_activity",
                column: "mtd_form_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_activity",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_store_activity_idx",
                table: "mtd_store_activity",
                column: "mtd_form_activity_id");

            migrationBuilder.CreateIndex(
                name: "fk_store_activity_store_idx",
                table: "mtd_store_activity",
                column: "mtd_store_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_store_activity");

            migrationBuilder.DropTable(
                name: "mtd_form_activity");
        }
    }
}
