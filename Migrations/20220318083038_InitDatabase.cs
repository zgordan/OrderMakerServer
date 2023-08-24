using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Mtd.OrderMaker.Server.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mtd_category_form",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    parent = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_category_form", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_config_file",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(45)", nullable: false),
                    file_size = table.Column<string>(type: "varchar(45)", nullable: false),
                    file_type = table.Column<string>(type: "varchar(45)", nullable: false),
                    file_data = table.Column<byte[]>(type: "mediumblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_config_file", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_config_param",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(45)", nullable: true),
                    value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_config_param", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mtd_group",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_group", x => x.id);
                });

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
                name: "mtd_register",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    parent_limit = table.Column<sbyte>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_register", x => x.id);
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
                name: "mtd_sys_trigger",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_sys_trigger", x => x.id);
                });

            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('9C85B07F-9236-4314-A29E-87B20093CF82','No Trigger',0)");
            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('D3663BC7-FA05-4F64-8EBD-F25414E459B8','Datetime now',1)");
            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('33E8212E-059B-482D-8CBD-DFDB073E3B63','User group',2)");
            migrationBuilder.Sql("insert into mtd_sys_trigger (id,name,sequence) values('08FE6202-45D7-46C2-B343-B79FD4831F27','User name',3)");


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
                    mtd_category = table.Column<string>(type: "varchar(36)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    Parent = table.Column<string>(type: "varchar(36)", nullable: true),
                    visible_number = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    visible_date = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_category",
                        column: x => x.mtd_category,
                        principalTable: "mtd_category_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_form_parent",
                        column: x => x.Parent,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_approval",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", nullable: false),
                    description = table.Column<string>(type: "varchar(512)", nullable: false),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false),
                    img_start = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_start_type = table.Column<string>(type: "varchar(48)", nullable: true),
                    img_start_text = table.Column<string>(type: "varchar(255)", nullable: true),
                    img_iteraction = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_iteraction_type = table.Column<string>(type: "varchar(48)", nullable: true),
                    img_iteraction_text = table.Column<string>(type: "varchar(255)", nullable: true),
                    img_waiting = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_waiting_type = table.Column<string>(type: "varchar(48)", nullable: true),
                    img_waiting_text = table.Column<string>(type: "varchar(255)", nullable: true),
                    img_approved = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_approved_type = table.Column<string>(type: "varchar(48)", nullable: true),
                    img_approved_text = table.Column<string>(type: "varchar(255)", nullable: true),
                    img_rejected = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_rejected_type = table.Column<string>(type: "varchar(48)", nullable: true),
                    img_rejected_text = table.Column<string>(type: "varchar(255)", nullable: true),
                    img_required = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_required_type = table.Column<string>(type: "varchar(48)", nullable: true),
                    img_required_text = table.Column<string>(type: "varchar(255)", nullable: true)
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
                name: "mtd_event_subscribe",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    event_create = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    event_edit = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_event_subscribe", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_event_mtd_form",
                        column: x => x.mtd_form_id,
                        principalTable: "mtd_form",
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
                    page = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    waitlist = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    show_number = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    show_date = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'")
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
                name: "mtd_filter_script",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(256)", nullable: false),
                    script = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_script", x => x.id);
                    table.ForeignKey(
                        name: "fk_script_filter",
                        column: x => x.mtd_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_form_activity",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    image = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    image_type = table.Column<string>(type: "varchar(256)", nullable: true),
                    sequence = table.Column<int>(type: "int(11)", nullable: false),
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
                    title = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    child = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
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
                name: "mtd_form_related",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    parent_form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    child_form_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_form_related", x => x.id);
                    table.ForeignKey(
                        name: "fk_child_form",
                        column: x => x.child_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_parent_form",
                        column: x => x.parent_form_id,
                        principalTable: "mtd_form",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    reviewer = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    change_date = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    own_deny_group = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    export_to_excel = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    related_create = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    related_edit = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
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
                name: "mtd_store",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    active = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    mtd_form = table.Column<string>(type: "varchar(36)", nullable: false),
                    timecr = table.Column<DateTime>(type: "datetime", nullable: false),
                    Parent = table.Column<string>(type: "varchar(36)", nullable: true)
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
                    table.ForeignKey(
                        name: "fk_mtd_store_parent",
                        column: x => x.Parent,
                        principalTable: "mtd_store",
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
                name: "mtd_filter_date",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false),
                    date_start = table.Column<DateTime>(type: "datetime", nullable: false),
                    date_end = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_date", x => x.id);
                    table.ForeignKey(
                        name: "fk_date_filter",
                        column: x => x.id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "mtd_filter_related",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false),
                    form_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    docbasednumber = table.Column<int>(name: "doc-based-number", type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_related", x => x.id);
                    table.ForeignKey(
                        name: "fk_related_filter",
                        column: x => x.id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_filter_script_apply",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_filter_id = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_filter_script_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_filter_script_apply", x => x.id);
                    table.ForeignKey(
                        name: "fk_script_filter_apply1",
                        column: x => x.mtd_filter_id,
                        principalTable: "mtd_filter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_script_filter_apply2",
                        column: x => x.mtd_filter_script_id,
                        principalTable: "mtd_filter_script",
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
                    read_only = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    mtd_sys_type = table.Column<int>(type: "int(11)", nullable: false),
                    mtd_form_part = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_sys_trigger = table.Column<string>(type: "varchar(36)", nullable: false, defaultValue: ""),
                    default_data = table.Column<string>(type: "varchar(255)", nullable: false, defaultValue: "")
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
                        name: "fk_mtd_form_part_field_mtd_sys_trigger",
                        column: x => x.mtd_sys_trigger,
                        principalTable: "mtd_sys_trigger",
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

            migrationBuilder.CreateTable(
                name: "mtd_log_document",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_store = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(256)", nullable: false),
                    timech = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_log_document", x => x.id);
                    table.ForeignKey(
                        name: "fk_log_document_store",
                        column: x => x.mtd_store,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_activity",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_store_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_form_activity_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    app_comment = table.Column<string>(type: "varchar(512)", nullable: false),
                    timecr = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "mtd_store_owner",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_owner", x => x.id);
                    table.ForeignKey(
                        name: "fk_owner_store",
                        column: x => x.id,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_store_task",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_store_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(250)", nullable: false),
                    deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    iniciator = table.Column<string>(type: "varchar(36)", nullable: false),
                    init_note = table.Column<string>(type: "varchar(250)", nullable: false),
                    init_timecr = table.Column<DateTime>(type: "datetime", nullable: false),
                    executor = table.Column<string>(type: "varchar(36)", nullable: false),
                    exec_note = table.Column<string>(type: "varchar(250)", nullable: false),
                    exec_timecr = table.Column<DateTime>(type: "datetime", nullable: false),
                    complete = table.Column<int>(type: "int(11)", nullable: false),
                    private_task = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    last_evant_time = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_store_task", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_store_task",
                        column: x => x.mtd_store_id,
                        principalTable: "mtd_store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_approval_rejection",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    note = table.Column<string>(type: "varchar(512)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    color = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'green'"),
                    mtd_approval_stage_id = table.Column<int>(type: "int(11)", nullable: false),
                    img_data = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_type = table.Column<string>(type: "varchar(45)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_approval_rejection", x => x.id);
                    table.ForeignKey(
                        name: "fk_rejection_stage",
                        column: x => x.mtd_approval_stage_id,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_approval_resolution",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: false),
                    note = table.Column<string>(type: "varchar(512)", nullable: false),
                    sequence = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    color = table.Column<string>(type: "varchar(45)", nullable: false, defaultValueSql: "'green'"),
                    mtd_approval_stage_id = table.Column<int>(type: "int(11)", nullable: false),
                    img_data = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_type = table.Column<string>(type: "varchar(45)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_approval_resolution", x => x.id);
                    table.ForeignKey(
                        name: "fk_resolution_stage",
                        column: x => x.mtd_approval_stage_id,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mtd_log_approval",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mtd_store = table.Column<string>(type: "varchar(36)", nullable: false),
                    stage = table.Column<int>(type: "int(11)", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "'No Name'"),
                    user_recipient_id = table.Column<string>(type: "varchar(36)", nullable: true),
                    user_recipient_name = table.Column<string>(type: "varchar(255)", nullable: true),
                    result = table.Column<int>(type: "int(11)", nullable: false),
                    timecr = table.Column<DateTime>(type: "datetime", nullable: false),
                    img_data = table.Column<byte[]>(type: "mediumblob", nullable: true),
                    img_type = table.Column<string>(type: "varchar(50)", nullable: true),
                    color = table.Column<string>(type: "varchar(50)", nullable: true),
                    note = table.Column<string>(type: "varchar(512)", nullable: true),
                    app_comment = table.Column<string>(type: "varchar(512)", nullable: true),
                    is_sign = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_log_approval", x => x.id);
                    table.ForeignKey(
                        name: "fk_log_approval_stage",
                        column: x => x.stage,
                        principalTable: "mtd_approval_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_log_approval_store",
                        column: x => x.mtd_store,
                        principalTable: "mtd_store",
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
                    complete = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'0'"),
                    result = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    sign_chain = table.Column<string>(type: "longtext", nullable: true),
                    last_event_time = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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
                    value_extra = table.Column<string>(type: "varchar(256)", nullable: true),
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
                name: "mtd_register_field",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", nullable: false),
                    mtd_register_id = table.Column<string>(type: "varchar(36)", nullable: false),
                    income = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    expense = table.Column<sbyte>(type: "tinyint(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_register_field", x => x.id);
                    table.ForeignKey(
                        name: "fk_mtd_form_register",
                        column: x => x.mtd_register_id,
                        principalTable: "mtd_register",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mtd_form_register_field",
                        column: x => x.id,
                        principalTable: "mtd_form_part_field",
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
                    register = table.Column<byte[]>(type: "longblob", nullable: false),
                    file_name = table.Column<string>(type: "varchar(256)", nullable: false),
                    file_size = table.Column<long>(type: "bigint(20)", nullable: false),
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
                name: "fk_approvel_form_idx",
                table: "mtd_approval",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_rejection_stage_idx",
                table: "mtd_approval_rejection",
                column: "mtd_approval_stage_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval_rejection",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sequence",
                table: "mtd_approval_rejection",
                column: "sequence");

            migrationBuilder.CreateIndex(
                name: "fk_resolution_stage_idx",
                table: "mtd_approval_resolution",
                column: "mtd_approval_stage_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval_resolution",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sequence",
                table: "mtd_approval_resolution",
                column: "sequence");

            migrationBuilder.CreateIndex(
                name: "fk_stage_approval_idx",
                table: "mtd_approval_stage",
                column: "mtd_approval");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_approval_stage",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER",
                table: "mtd_approval_stage",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_group_themself_idx",
                table: "mtd_category_form",
                column: "parent");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_category_form",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_config_file",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_config_param",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_event_mtd_form_idx",
                table: "mtd_event_subscribe",
                column: "mtd_form_id");

            migrationBuilder.CreateIndex(
                name: "id_unique",
                table: "mtd_event_subscribe",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_id",
                table: "mtd_event_subscribe",
                column: "user_id");

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
                table: "mtd_filter_date",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_field",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "mtd_filter_field_mtd_form_field_idx",
                table: "mtd_filter_field",
                column: "mtd_form_part_field");

            migrationBuilder.CreateIndex(
                name: "mtd_filter_field_term_idx",
                table: "mtd_filter_field",
                column: "mtd_term");

            migrationBuilder.CreateIndex(
                name: "mtd_filter_idx",
                table: "mtd_filter_field",
                column: "mtd_filter");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_owner",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_related",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_idx",
                table: "mtd_filter_script",
                column: "mtd_form_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_script",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_apply1_idx",
                table: "mtd_filter_script_apply",
                column: "mtd_filter_id");

            migrationBuilder.CreateIndex(
                name: "fk_script_filter_apply2_idx",
                table: "mtd_filter_script_apply",
                column: "mtd_filter_script_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_filter_script_apply",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mtd_form_mtd_category",
                table: "mtd_form",
                column: "mtd_category");

            migrationBuilder.CreateIndex(
                name: "IX_mtd_form_Parent",
                table: "mtd_form",
                column: "Parent");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_activity_idx",
                table: "mtd_form_activity",
                column: "mtd_form_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_activity",
                column: "id",
                unique: true);

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
                name: "fk_list_form_idx",
                table: "mtd_form_list",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_list",
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
                table: "mtd_form_part",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_field_mtd_form_part1_idx",
                table: "mtd_form_part_field",
                column: "mtd_form_part");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_field_mtd_sys_trigger_idx",
                table: "mtd_form_part_field",
                column: "mtd_sys_trigger");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_part_field_mtd_sys_type1_idx",
                table: "mtd_form_part_field",
                column: "mtd_sys_type");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_part_field",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_part_header",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_child_form_idx",
                table: "mtd_form_related",
                column: "child_form_id");

            migrationBuilder.CreateIndex(
                name: "fk_parent_form_idx",
                table: "mtd_form_related",
                column: "parent_form_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_form_related",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_group",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_log_approval_stage_idx",
                table: "mtd_log_approval",
                column: "stage");

            migrationBuilder.CreateIndex(
                name: "fk_log_approval_store_idx",
                table: "mtd_log_approval",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_log_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_log_document_store_idx",
                table: "mtd_log_document",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_log_document",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_date",
                table: "mtd_log_document",
                column: "timech");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy",
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
                name: "id_UNIQUE",
                table: "mtd_policy_forms",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UNIQUE_FORM",
                table: "mtd_policy_forms",
                columns: new[] { "mtd_policy", "mtd_form" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_policy_parts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mtd_policy_parts_mtd_form_part",
                table: "mtd_policy_parts",
                column: "mtd_form_part");

            migrationBuilder.CreateIndex(
                name: "UNIQUE_PART",
                table: "mtd_policy_parts",
                columns: new[] { "mtd_policy", "mtd_form_part" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mtd_policy_scripts_mtd_policy_id",
                table: "mtd_policy_scripts",
                column: "mtd_policy_id");

            migrationBuilder.CreateIndex(
                name: "Unique_id",
                table: "mtd_policy_scripts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Unique_Policy_Script",
                table: "mtd_policy_scripts",
                columns: new[] { "mtd_filter_script_id", "mtd_policy_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_register",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_form_register_idx",
                table: "mtd_register_field",
                column: "mtd_register_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_register_field",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_mtd_form1_idx",
                table: "mtd_store",
                column: "mtd_form");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mtd_store_Parent",
                table: "mtd_store",
                column: "Parent");

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
                name: "fk_store_activity_idx",
                table: "mtd_store_activity",
                column: "mtd_form_activity_id");

            migrationBuilder.CreateIndex(
                name: "fk_store_activity_store_idx",
                table: "mtd_store_activity",
                column: "mtd_store_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_activity",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_store_approve_stage_idx",
                table: "mtd_store_approval",
                column: "md_approve_stage");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_approval",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APPROVED",
                table: "mtd_store_approval",
                column: "complete");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_link_mtd_store1_idx",
                table: "mtd_store_link",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_link",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_register",
                table: "mtd_store_link",
                column: "Register");

            migrationBuilder.CreateIndex(
                name: "ix_unique",
                table: "mtd_store_link",
                columns: new[] { "mtd_store", "id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_owner",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER",
                table: "mtd_store_owner",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_stack_mtd_form_part_field1_idx",
                table: "mtd_store_stack",
                column: "mtd_form_part_field");

            migrationBuilder.CreateIndex(
                name: "fk_mtd_store_stack_mtd_store_idx",
                table: "mtd_store_stack",
                column: "mtd_store");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_stack",
                column: "id",
                unique: true);

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
                name: "fk_mtd_store_task_idx",
                table: "mtd_store_task",
                column: "mtd_store_id");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_store_task",
                column: "id",
                unique: true);

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
                table: "mtd_sys_trigger",
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
                name: "mtd_approval_rejection");

            migrationBuilder.DropTable(
                name: "mtd_approval_resolution");

            migrationBuilder.DropTable(
                name: "mtd_config_file");

            migrationBuilder.DropTable(
                name: "mtd_config_param");

            migrationBuilder.DropTable(
                name: "mtd_event_subscribe");

            migrationBuilder.DropTable(
                name: "mtd_filter_column");

            migrationBuilder.DropTable(
                name: "mtd_filter_date");

            migrationBuilder.DropTable(
                name: "mtd_filter_field");

            migrationBuilder.DropTable(
                name: "mtd_filter_owner");

            migrationBuilder.DropTable(
                name: "mtd_filter_related");

            migrationBuilder.DropTable(
                name: "mtd_filter_script_apply");

            migrationBuilder.DropTable(
                name: "mtd_form_desk");

            migrationBuilder.DropTable(
                name: "mtd_form_header");

            migrationBuilder.DropTable(
                name: "mtd_form_list");

            migrationBuilder.DropTable(
                name: "mtd_form_part_header");

            migrationBuilder.DropTable(
                name: "mtd_form_related");

            migrationBuilder.DropTable(
                name: "mtd_group");

            migrationBuilder.DropTable(
                name: "mtd_log_approval");

            migrationBuilder.DropTable(
                name: "mtd_log_document");

            migrationBuilder.DropTable(
                name: "mtd_policy_forms");

            migrationBuilder.DropTable(
                name: "mtd_policy_parts");

            migrationBuilder.DropTable(
                name: "mtd_policy_scripts");

            migrationBuilder.DropTable(
                name: "mtd_register_field");

            migrationBuilder.DropTable(
                name: "mtd_store_activity");

            migrationBuilder.DropTable(
                name: "mtd_store_approval");

            migrationBuilder.DropTable(
                name: "mtd_store_link");

            migrationBuilder.DropTable(
                name: "mtd_store_owner");

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
                name: "mtd_store_task");

            migrationBuilder.DropTable(
                name: "mtd_sys_term");

            migrationBuilder.DropTable(
                name: "mtd_filter");

            migrationBuilder.DropTable(
                name: "mtd_filter_script");

            migrationBuilder.DropTable(
                name: "mtd_policy");

            migrationBuilder.DropTable(
                name: "mtd_register");

            migrationBuilder.DropTable(
                name: "mtd_form_activity");

            migrationBuilder.DropTable(
                name: "mtd_approval_stage");

            migrationBuilder.DropTable(
                name: "mtd_store_stack");

            migrationBuilder.DropTable(
                name: "mtd_approval");

            migrationBuilder.DropTable(
                name: "mtd_form_part_field");

            migrationBuilder.DropTable(
                name: "mtd_store");

            migrationBuilder.DropTable(
                name: "mtd_form_part");

            migrationBuilder.DropTable(
                name: "mtd_sys_trigger");

            migrationBuilder.DropTable(
                name: "mtd_sys_type");

            migrationBuilder.DropTable(
                name: "mtd_form");

            migrationBuilder.DropTable(
                name: "mtd_sys_style");

            migrationBuilder.DropTable(
                name: "mtd_category_form");
        }
    }
}
