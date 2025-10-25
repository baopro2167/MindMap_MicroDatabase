using Microsoft.EntityFrameworkCore;
using Model;
using Model.Pagging;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BranchService : IBranchService
    {
        private readonly MindMapDbContext _db;

        public BranchService(MindMapDbContext db) => _db = db;
        public async Task<Branch?> GetByIdAsync(int branchId)
        {
            return await _db.Branches
       .AsNoTracking()
       .FirstOrDefaultAsync(b => b.BranchId == branchId);
        }

        public async Task<IReadOnlyCollection<Branch>> GetAllByMindMapAsync(int mindMapId)
        {
            var list = await _db.Branches
                .AsNoTracking()
                .Where(b => b.MindMapId == mindMapId)
                .OrderBy(b => b.SourceNodeId).ThenBy(b => b.TargetNodeId)
                .ToListAsync();
            return list;
        }

        public async Task<PaginatedList<Branch>> GetByMindMapId(int mindMapId, int pageNumber, int pageSize)
        {
            var q = _db.Branches
                .AsNoTracking()
                .Where(b => b.MindMapId == mindMapId)
                .OrderBy(b => b.SourceNodeId).ThenBy(b => b.TargetNodeId);

            return await PaginatedList<Branch>.CreateAsync(q, pageNumber, pageSize);
        }

        public async Task<Branch> AddAsync(AddBranchRequestDTO dto)
        {
            // MindMap tồn tại?
            var mapExists = await _db.MindMaps.AsNoTracking().AnyAsync(m => m.MindMapId == dto.MindMapId);
            if (!mapExists) throw new KeyNotFoundException($"MindMap {dto.MindMapId} not found.");

            // Node tồn tại & cùng MindMap?
            var source = await _db.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.NodeId == dto.SourceNodeId);
            if (source == null) throw new KeyNotFoundException($"Source node {dto.SourceNodeId} not found.");
            var target = await _db.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.NodeId == dto.TargetNodeId);
            if (target == null) throw new KeyNotFoundException($"Target node {dto.TargetNodeId} not found.");

            if (source.MindMapId != dto.MindMapId || target.MindMapId != dto.MindMapId)
                throw new InvalidOperationException("Source/Target node phải thuộc cùng MindMap.");

            if (dto.SourceNodeId == dto.TargetNodeId)
                throw new InvalidOperationException("SourceNodeId không được trùng TargetNodeId.");

            var branch = new Branch
            {
                MindMapId = dto.MindMapId,
                SourceNodeId = dto.SourceNodeId,
                TargetNodeId = dto.TargetNodeId,
                BranchType = dto.BranchType,
                Label = dto.Label,
                Style = dto.Style
            };

            _db.Branches.Add(branch);
            await _db.SaveChangesAsync();
            return branch;
        }

        public async Task<Branch?> UpdateAsync(int branchId, UpdateBranchRequestDTO dto)
        {
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.BranchId == branchId);
            if (branch == null) return null;

            if (dto.SourceNodeId.HasValue && dto.SourceNodeId != branch.SourceNodeId)
            {
                var src = await _db.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.NodeId == dto.SourceNodeId.Value);
                if (src == null) throw new KeyNotFoundException($"Source node {dto.SourceNodeId} not found.");
                if (src.MindMapId != branch.MindMapId) throw new InvalidOperationException("Source node phải cùng MindMap.");
                branch.SourceNodeId = dto.SourceNodeId.Value;
            }

            if (dto.TargetNodeId.HasValue && dto.TargetNodeId != branch.TargetNodeId)
            {
                var tgt = await _db.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.NodeId == dto.TargetNodeId.Value);
                if (tgt == null) throw new KeyNotFoundException($"Target node {dto.TargetNodeId} not found.");
                if (tgt.MindMapId != branch.MindMapId) throw new InvalidOperationException("Target node phải cùng MindMap.");
                branch.TargetNodeId = dto.TargetNodeId.Value;
            }

            if (branch.SourceNodeId == branch.TargetNodeId)
                throw new InvalidOperationException("SourceNodeId không được trùng TargetNodeId.");

            if (dto.BranchType.HasValue) branch.BranchType = dto.BranchType.Value;
            if (dto.Label != null) branch.Label = dto.Label;
            if (dto.Style != null) branch.Style = dto.Style;

            await _db.SaveChangesAsync();
            return branch;
        }

        public async Task DeleteAsync(int branchId)
        {
            var branch = await _db.Branches.FirstOrDefaultAsync(b => b.BranchId == branchId);
            if (branch == null) throw new KeyNotFoundException($"Branch {branchId} not found.");

            _db.Branches.Remove(branch);
            await _db.SaveChangesAsync();
        }
    }
}
