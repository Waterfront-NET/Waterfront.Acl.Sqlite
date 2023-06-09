﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Waterfront.Acl.Sqlite.Models;

#nullable disable

namespace Waterfront.Acl.Sqlite.Migrations
{
    [DbContext(typeof(SqliteAclDbContext))]
    [Migration("20230515121829_0.1.0-alpha.2")]
    partial class _010alpha2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.16");

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicy", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Name")
                        .HasName("pk_wf_acl");

                    b.ToTable("wf_acl", (string)null);
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicyAccessRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("PolicyName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("policy_name");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_wf_acl_policy_access_rules");

                    b.HasIndex("PolicyName")
                        .HasDatabaseName("ix_wf_acl_policy_access_rules_policy_name");

                    b.ToTable("wf_acl_policy_access_rules", (string)null);
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicyAccessRuleAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("RuleId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("rule_id");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_wf_acl_policy_access_rule_actions");

                    b.HasIndex("RuleId")
                        .HasDatabaseName("ix_wf_acl_policy_access_rule_actions_rule_id");

                    b.ToTable("wf_acl_policy_access_rule_actions", (string)null);
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclUser", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("TEXT")
                        .HasColumnName("username");

                    b.Property<string>("IpAddress")
                        .HasColumnType("TEXT")
                        .HasColumnName("ip_address");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT")
                        .HasColumnName("password");

                    b.HasKey("Username")
                        .HasName("pk_wf_users");

                    b.ToTable("wf_users", (string)null);
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclUserPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_wf_acl_user_policies");

                    b.HasIndex("Username")
                        .HasDatabaseName("ix_wf_acl_user_policies_username");

                    b.ToTable("wf_acl_user_policies", (string)null);
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicyAccessRule", b =>
                {
                    b.HasOne("Waterfront.Acl.SQLite.Models.SqliteAclPolicy", "Policy")
                        .WithMany("Access")
                        .HasForeignKey("PolicyName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_wf_acl_policy_access_rules_wf_acl_policy_name");

                    b.Navigation("Policy");
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicyAccessRuleAction", b =>
                {
                    b.HasOne("Waterfront.Acl.SQLite.Models.SqliteAclPolicyAccessRule", "Rule")
                        .WithMany("Actions")
                        .HasForeignKey("RuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_wf_acl_policy_access_rule_actions_wf_acl_policy_access_rules_rule_id");

                    b.Navigation("Rule");
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclUserPolicy", b =>
                {
                    b.HasOne("Waterfront.Acl.SQLite.Models.SqliteAclUser", "User")
                        .WithMany("Acl")
                        .HasForeignKey("Username")
                        .HasConstraintName("fk_wf_acl_user_policies_wf_users_username");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicy", b =>
                {
                    b.Navigation("Access");
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclPolicyAccessRule", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("Waterfront.Acl.SQLite.Models.SqliteAclUser", b =>
                {
                    b.Navigation("Acl");
                });
#pragma warning restore 612, 618
        }
    }
}
