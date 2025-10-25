using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MindMapDbContext : DbContext
    {
        public MindMapDbContext(DbContextOptions<MindMapDbContext> options)
            : base(options)
        {
        }

        public DbSet<MindMap> MindMaps { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<MindMapReport> MindMapReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MindMap - Node
            modelBuilder.Entity<Node>()
                .HasOne(n => n.MindMap)
                .WithMany(m => m.Nodes)
                .HasForeignKey(n => n.MindMapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Node self-ref
            modelBuilder.Entity<Node>()
                .HasOne(n => n.ParentNode)
                .WithMany(n => n.ChildNodes)
                .HasForeignKey(n => n.ParentNodeId)
                    .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // (type hint cho toạ độ — tuỳ chọn)
            modelBuilder.Entity<Node>().Property(n => n.PositionX).HasColumnType("real");
            modelBuilder.Entity<Node>().Property(n => n.PositionY).HasColumnType("real");
            modelBuilder.Entity<Node>().Property(n => n.Content).HasMaxLength(2000);
            modelBuilder.Entity<Node>().Property(n => n.Color).HasMaxLength(50);
            modelBuilder.Entity<Node>().Property(n => n.Shape).HasMaxLength(50);

            // Branch - MindMap
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.MindMap)
                .WithMany(m => m.Branches)
                .HasForeignKey(b => b.MindMapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Branch - Source/Target
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.SourceNode)
                .WithMany(n => n.OutgoingBranches)
                .HasForeignKey(b => b.SourceNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Branch>()
                .HasOne(b => b.TargetNode)
                .WithMany(n => n.IncomingBranches)
                .HasForeignKey(b => b.TargetNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // CHECK constraint cho Postgres (không dùng ngoặc vuông)
            modelBuilder.Entity<Branch>()
     .ToTable(t => t.HasCheckConstraint(
         "CK_Branch_Source_Not_Target",
         "\"SourceNodeId\" <> \"TargetNodeId\""   // 👈 QUOTE tên cột
     ));

            // Unique (directed edge)
            modelBuilder.Entity<Branch>()
                .HasIndex(x => new { x.MindMapId, x.SourceNodeId, x.TargetNodeId })
                .IsUnique();

            modelBuilder.Entity<Branch>().Property(b => b.Label).HasMaxLength(500);
            modelBuilder.Entity<Branch>().Property(b => b.Style).HasMaxLength(100);

            // MindMap
            modelBuilder.Entity<MindMap>().Property(m => m.Description).HasMaxLength(2000);
            modelBuilder.Entity<MindMap>().Property(m => m.CreatedAt)
                                         .HasDefaultValueSql("now() AT TIME ZONE 'UTC'");

            // MindMap - Report
            modelBuilder.Entity<MindMapReport>()
                .HasOne(r => r.MindMap)
                .WithMany(m => m.Reports)
                .HasForeignKey(r => r.MindMapId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
