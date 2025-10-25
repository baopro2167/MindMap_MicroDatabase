using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class aaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MindMap",
                columns: table => new
                {
                    MindMapId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedByUser = table.Column<int>(type: "integer", nullable: false),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindMap", x => x.MindMapId);
                });

            migrationBuilder.CreateTable(
                name: "MindMapReport",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MindMapId = table.Column<int>(type: "integer", nullable: false),
                    MembershipId = table.Column<int>(type: "integer", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReportContent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MindMapReport", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_MindMapReport_MindMap_MindMapId",
                        column: x => x.MindMapId,
                        principalTable: "MindMap",
                        principalColumn: "MindMapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Node",
                columns: table => new
                {
                    NodeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MindMapId = table.Column<int>(type: "integer", nullable: false),
                    ParentNodeId = table.Column<int>(type: "integer", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    NodeType = table.Column<int>(type: "integer", nullable: false),
                    PositionX = table.Column<float>(type: "real", nullable: false),
                    PositionY = table.Column<float>(type: "real", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Shape = table.Column<string>(type: "text", nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Node", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_Node_MindMap_MindMapId",
                        column: x => x.MindMapId,
                        principalTable: "MindMap",
                        principalColumn: "MindMapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Node_Node_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "Node",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MindMapId = table.Column<int>(type: "integer", nullable: false),
                    SourceNodeId = table.Column<int>(type: "integer", nullable: false),
                    TargetNodeId = table.Column<int>(type: "integer", nullable: false),
                    BranchType = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true),
                    Style = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_Branch_MindMap_MindMapId",
                        column: x => x.MindMapId,
                        principalTable: "MindMap",
                        principalColumn: "MindMapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Branch_Node_SourceNodeId",
                        column: x => x.SourceNodeId,
                        principalTable: "Node",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Branch_Node_TargetNodeId",
                        column: x => x.TargetNodeId,
                        principalTable: "Node",
                        principalColumn: "NodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branch_MindMapId",
                table: "Branch",
                column: "MindMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_SourceNodeId",
                table: "Branch",
                column: "SourceNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_TargetNodeId",
                table: "Branch",
                column: "TargetNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_MindMapReport_MindMapId",
                table: "MindMapReport",
                column: "MindMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Node_MindMapId",
                table: "Node",
                column: "MindMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Node_ParentNodeId",
                table: "Node",
                column: "ParentNodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "MindMapReport");

            migrationBuilder.DropTable(
                name: "Node");

            migrationBuilder.DropTable(
                name: "MindMap");
        }
    }
}
