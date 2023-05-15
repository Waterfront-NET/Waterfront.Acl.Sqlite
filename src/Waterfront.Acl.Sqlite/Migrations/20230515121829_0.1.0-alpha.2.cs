using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waterfront.Acl.Sqlite.Migrations
{
    public partial class _010alpha2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wf_acl",
                columns: table => new
                {
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wf_acl", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "wf_users",
                columns: table => new
                {
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    password = table.Column<string>(type: "TEXT", nullable: true),
                    ip_address = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wf_users", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "wf_acl_policy_access_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    policy_name = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wf_acl_policy_access_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_wf_acl_policy_access_rules_wf_acl_policy_name",
                        column: x => x.policy_name,
                        principalTable: "wf_acl",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wf_acl_user_policies",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wf_acl_user_policies", x => x.id);
                    table.ForeignKey(
                        name: "fk_wf_acl_user_policies_wf_users_username",
                        column: x => x.username,
                        principalTable: "wf_users",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "wf_acl_policy_access_rule_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    value = table.Column<string>(type: "TEXT", nullable: false),
                    rule_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wf_acl_policy_access_rule_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_wf_acl_policy_access_rule_actions_wf_acl_policy_access_rules_rule_id",
                        column: x => x.rule_id,
                        principalTable: "wf_acl_policy_access_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_wf_acl_policy_access_rule_actions_rule_id",
                table: "wf_acl_policy_access_rule_actions",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "ix_wf_acl_policy_access_rules_policy_name",
                table: "wf_acl_policy_access_rules",
                column: "policy_name");

            migrationBuilder.CreateIndex(
                name: "ix_wf_acl_user_policies_username",
                table: "wf_acl_user_policies",
                column: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wf_acl_policy_access_rule_actions");

            migrationBuilder.DropTable(
                name: "wf_acl_user_policies");

            migrationBuilder.DropTable(
                name: "wf_acl_policy_access_rules");

            migrationBuilder.DropTable(
                name: "wf_users");

            migrationBuilder.DropTable(
                name: "wf_acl");
        }
    }
}
