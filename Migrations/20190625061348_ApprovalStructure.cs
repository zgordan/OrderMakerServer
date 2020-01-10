/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class ApprovalStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_approval",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_approval", x => x.id);
                    table.ForeignKey(
                        name: "fk_approvel_form",
                        column: x => x.mtd_form,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_approval_stage",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    mtd_approval = table.Column<string>(type: "varchar(36)", nullable: false),
                    stage = table.Column<int>(type: "int(11)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    block_parts = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_approval_stage", x => x.id);
                    table.ForeignKey(
                        name: "fk_stage_approval",
                        column: x => x.mtd_approval,
                        principalTable: "mtd_approval",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_log_approval",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_approval_stage = table.Column<int>(type: "int(11)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    timech = table.Column<DateTime>(type: "datetime", nullable: false),
                    result = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_log_approval", x => x.id);
                    table.ForeignKey(
                        name: "fk_log_approval_stage",
                        column: x => x.mtd_approval_stage,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_approval",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    md_approve_stage = table.Column<int>(type: "int(11)", nullable: false),
                    parts_approved = table.Column<string>(type: "longtext", nullable: false),
                    approved = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_approval", x => x.id);
                    table.ForeignKey(
                        name: "fk_store_approve",
                        column: x => x.id,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_store_approve_stage",
                        column: x => x.md_approve_stage,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_approvel_form_idx",
                table: "mtd_approval",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval_stage",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_stage_approval_idx",
                table: "mtd_approval_stage",
                column: "mtd_approval");

            migrationBuilder.CreateIndex(
                name: "IX_USER",
                table: "mtd_approval_stage",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_log_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_log_approval_stage_idx",
                table: "mtd_log_approval",
                column: "mtd_approval_stage");

            migrationBuilder.CreateIndex(
                name: "IX_APPROVED",
                table: "mtd_store_approval",
                column: "approved");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_store_approve_stage_idx",
                table: "mtd_store_approval",
                column: "md_approve_stage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_log_approval");

            migrationBuilder.DropTable(
                name: "mtd_store_approval");

            migrationBuilder.DropTable(
                name: "mtd_approval_stage");

            migrationBuilder.DropTable(
                name: "mtd_approval");
        }
    }
}
