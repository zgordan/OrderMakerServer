using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Mtd.OrderMaker.Server.Entity.Migrations
{
    public partial class OwnersActionLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_cpq_log_action",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(255)", nullable: false),
                    action_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    action_type = table.Column<int>(type: "int", nullable: false),
                    document_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_cpq_log_action", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_cpq_proposal_owner",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(255)", nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_cpq_proposal_owner", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_cpq_titles_owner",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_cpq_titles_owner", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_documentid",
                table: "mtd_cpq_log_action",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_cpq_log_action",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_userid",
                table: "mtd_cpq_log_action",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_cpq_proposal_owner",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_userid",
                table: "mtd_cpq_proposal_owner",
                column: "user_name");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_cpq_titles_owner",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_userid",
                table: "mtd_cpq_titles_owner",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_cpq_log_action");

            migrationBuilder.DropTable(
                name: "mtd_cpq_proposal_owner");

            migrationBuilder.DropTable(
                name: "mtd_cpq_titles_owner");
        }
    }
}
