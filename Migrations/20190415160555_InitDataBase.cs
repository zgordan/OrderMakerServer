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

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class InitDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_group_form",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    parent = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_group_form", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_sys_style",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_sys_style", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_sys_term",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(45)", nullable: false),
                    sign = table.Column<string>(type: "varchar(45)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_sys_term", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_sys_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_sys_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    mtd_group = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_grooup",
                        column: x => x.mtd_group,
                        principalTable: "mtd_group_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_filter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idUser = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false),
                    page_size = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'10'"),
                    searchText = table.Column<string>(type: "varchar(256)", nullable: false),
                    searchNumber = table.Column<string>(type: "varchar(45)", nullable: false),
                    page = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter", x => x.id);
                    table.ForeignKey(
                        name: "mtd_filter_mtd_form",
                        column: x => x.mtd_form,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_desk",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    image = table.Column<byte[]>(type: "mediumblob", nullable: false),
                    image_type = table.Column<string>(type: "varchar(256)", nullable: false),
                    image_size = table.Column<int>(type: "int(11)", nullable: false),
                    color_font = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'white'"),
                    color_back = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'gray'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_desk", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_form_des_mtd_from",
                        column: x => x.id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_header",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    image = table.Column<byte[]>(type: "mediumblob", nullable: false),
                    image_type = table.Column<string>(type: "varchar(256)", nullable: false),
                    image_size = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_header", x => x.id);
                    table.ForeignKey(
                        name: "fk_image_form",
                        column: x => x.id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_part",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    mtd_sys_style = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false),
                    title = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_part", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_form_part_mtd_form1",
                        column: x => x.mtd_form,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mtd_form_part_mtd_sys_style1",
                        column: x => x.mtd_sys_style,
                        principalTable: "mtd_sys_style",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false),
                    timecr = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_store_mtd_form1",
                        column: x => x.mtd_form,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_part_field",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    required = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    mtd_sys_type = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_form_part = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_part_field", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_form_part_field_mtd_form_part1",
                        column: x => x.mtd_form_part,
                        principalTable: "mtd_form_part",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mtd_form_part_field_mtd_sys_type1",
                        column: x => x.mtd_sys_type,
                        principalTable: "mtd_sys_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_part_header",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    image = table.Column<byte[]>(type: "mediumblob", nullable: false),
                    image_type = table.Column<string>(type: "varchar(256)", nullable: false),
                    image_size = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_part_header", x => x.id);
                    table.ForeignKey(
                        name: "fk_image_form_part",
                        column: x => x.id,
                        principalTable: "mtd_form_part",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_filter_column",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_filter = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_form_part_field = table.Column<string>(type: "varchar(36)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_column", x => x.id);
                    table.ForeignKey(
                        name: "mtd_filter_column_mtd_field",
                        column: x => x.mtd_filter,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "mtd_roster_field",
                        column: x => x.mtd_form_part_field,
                        principalTable: "mtd_form_part_field",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_filter_field",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_filter = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_form_part_field = table.Column<string>(type: "varchar(36)", nullable: false),
                    value = table.Column<string>(type: "varchar(256)", nullable: false),
                    mtd_term = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_field", x => x.id);
                    table.ForeignKey(
                        name: "mtd_filter_field_mtd_field",
                        column: x => x.mtd_filter,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "mtd_filter_field_mtd_form_field",
                        column: x => x.mtd_form_part_field,
                        principalTable: "mtd_form_part_field",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "mtd_filter_field_mtd_term",
                        column: x => x.mtd_term,
                        principalTable: "mtd_sys_term",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_list",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_list", x => x.id);
                    table.ForeignKey(
                        name: "fk_list_field",
                        column: x => x.id,
                        principalTable: "mtd_form_part_field",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_list_form",
                        column: x => x.mtd_form,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_stack",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_store = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_part_field = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_stack", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_store_stack_mtd_form_part_field1",
                        column: x => x.mtd_form_part_field,
                        principalTable: "mtd_form_part_field",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mtd_store_stack_mtd_store",
                        column: x => x.mtd_store,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_link",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false),
                    mtd_store = table.Column<string>(type: "varchar(36)", nullable: false),
                    Register = table.Column<string>(type: "varchar(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_link", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_store_link_mtd_store_stack",
                        column: x => x.id,
                        principalTable: "mtd_store_stack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mtd_store_link_mtd_store1",
                        column: x => x.mtd_store,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_stack_date",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false),
                    register = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_stack_date", x => x.id);
                    table.ForeignKey(
                        name: "fk_date_stack",
                        column: x => x.id,
                        principalTable: "mtd_store_stack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_stack_decimal",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false),
                    register = table.Column<decimal>(type: "decimal(20,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_stack_decimal", x => x.id);
                    table.ForeignKey(
                        name: "fk_decimal_stack",
                        column: x => x.id,
                        principalTable: "mtd_store_stack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_stack_file",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false),
                    register = table.Column<byte[]>(type: "mediumblob", nullable: false),
                    file_name = table.Column<string>(type: "varchar(256)", nullable: false),
                    file_size = table.Column<int>(type: "int(11)", nullable: false),
                    file_type = table.Column<string>(type: "varchar(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_stack_file", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_stack",
                        column: x => x.id,
                        principalTable: "mtd_store_stack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_stack_int",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false),
                    register = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_stack_int", x => x.id);
                    table.ForeignKey(
                        name: "fk_int_stack",
                        column: x => x.id,
                        principalTable: "mtd_store_stack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_stack_text",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint(20)", nullable: false),
                    register = table.Column<string>(type: "varchar(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_stack_text", x => x.id);
                    table.ForeignKey(
                        name: "fk_text_stack",
                        column: x => x.id,
                        principalTable: "mtd_store_stack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INDEX_USER",
                table: "mtd_filter",
                column: "idUser");

            migrationBuilder.CreateIndex(
                name: "mtd_filter_mtd_form_idx",
                table: "mtd_filter",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_column",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mtd_filter_column_idx",
                table: "mtd_filter_column",
                column: "mtd_filter");

            migrationBuilder.CreateIndex(
                name: "mtd_roster_field_idx",
                table: "mtd_filter_column",
                column: "mtd_form_part_field");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_field",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mtd_filter_idx",
                table: "mtd_filter_field",
                column: "mtd_filter");

            migrationBuilder.CreateIndex(
                name: "mtd_filter_field_mtd_form_field_idx",
                table: "mtd_filter_field",
                column: "mtd_form_part_field");

            migrationBuilder.CreateIndex(
                name: "mtd_filter_field_term_idx",
                table: "mtd_filter_field",
                column: "mtd_term");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_form_grooup_idx",
                table: "mtd_form",
                column: "mtd_group");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_desk",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_header",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_list",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_list_form_idx",
                table: "mtd_form_list",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_part",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_mtd_form1_idx",
                table: "mtd_form_part",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_mtd_sys_style1_idx",
                table: "mtd_form_part",
                column: "mtd_sys_style");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_part_field",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_field_mtd_form_part1_idx",
                table: "mtd_form_part_field",
                column: "mtd_form_part");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_field_mtd_sys_type1_idx",
                table: "mtd_form_part_field",
                column: "mtd_sys_type");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_part_header",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_group_form",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_group_themself_idx",
                table: "mtd_group_form",
                column: "parent");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_mtd_form1_idx",
                table: "mtd_store",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "IX_TIMECR",
                table: "mtd_store",
                column: "timecr");

            migrationBuilder.CreateIndex(
                name: "Seq_Unique",
                table: "mtd_store",
                columns: new[] { "mtd_form", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_link",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_link_mtd_store1_idx",
                table: "mtd_store_link",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "ix_unique",
                table: "mtd_store_link",
                columns: new[] { "mtd_store", "id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_stack",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_stack_mtd_form_part_field1_idx",
                table: "mtd_store_stack",
                column: "mtd_form_part_field");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_stack_mtd_store_idx",
                table: "mtd_store_stack",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "IX_UNIQUE",
                table: "mtd_store_stack",
                columns: new[] { "mtd_store", "mtd_form_part_field" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_stack_date",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DATESTACK",
                table: "mtd_store_stack_date",
                column: "register");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_stack_decimal",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DECIMALREGISTER",
                table: "mtd_store_stack_decimal",
                column: "register");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_stack_file",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_stack_int",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INTSTACK",
                table: "mtd_store_stack_int",
                column: "register");

            migrationBuilder.CreateIndex(
                name: "category_id_UNIQUE",
                table: "mtd_store_stack_text",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_REGISTER_TEXT",
                table: "mtd_store_stack_text",
                column: "register");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_sys_style",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_sys_term",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_sys_type",
                column: "id",
                unique: true);            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_filter_column");

            migrationBuilder.DropTable(
                name: "mtd_filter_field");

            migrationBuilder.DropTable(
                name: "mtd_form_desk");

            migrationBuilder.DropTable(
                name: "mtd_form_header");

            migrationBuilder.DropTable(
                name: "mtd_form_list");

            migrationBuilder.DropTable(
                name: "mtd_form_part_header");

            migrationBuilder.DropTable(
                name: "mtd_store_link");

            migrationBuilder.DropTable(
                name: "mtd_store_stack_date");

            migrationBuilder.DropTable(
                name: "mtd_store_stack_decimal");

            migrationBuilder.DropTable(
                name: "mtd_store_stack_file");

            migrationBuilder.DropTable(
                name: "mtd_store_stack_int");

            migrationBuilder.DropTable(
                name: "mtd_store_stack_text");

            migrationBuilder.DropTable(
                name: "mtd_filter");

            migrationBuilder.DropTable(
                name: "mtd_sys_term");

            migrationBuilder.DropTable(
                name: "mtd_store_stack");

            migrationBuilder.DropTable(
                name: "mtd_form_part_field");

            migrationBuilder.DropTable(
                name: "mtd_store");

            migrationBuilder.DropTable(
                name: "mtd_form_part");

            migrationBuilder.DropTable(
                name: "mtd_sys_type");

            migrationBuilder.DropTable(
                name: "mtd_form");

            migrationBuilder.DropTable(
                name: "mtd_sys_style");

            migrationBuilder.DropTable(
                name: "mtd_group_form");
        }
    }
}
