using Model.Pagging;
using Model;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.Respone;
using static NpgsqlTypes.NpgsqlTsQuery;

namespace Service
{
   public class NodeService :INodeService
    {
        private readonly MindMapDbContext _db;

        public NodeService(MindMapDbContext db)
        {
            _db = db;
        }

        public async Task<NodeReadDTO?> GetSlimByIdAsync(int nodeId, CancellationToken ct = default)
        {
            return await _db.Nodes
            .AsNoTracking()
                .Where(n => n.NodeId == nodeId)
                .Select(n => new NodeReadDTO
                {
                    NodeId = n.NodeId,
                    MindMapId = n.MindMapId,
                    ParentNodeId = n.ParentNodeId,
                    Content = n.Content,
                    NodeType = n.NodeType,
                    PositionX = n.PositionX,
                    PositionY = n.PositionY,
                    Color = n.Color,
                    Shape = n.Shape,
                    OrderIndex = n.OrderIndex
                })
                .FirstOrDefaultAsync(ct);
        }
        

        public async Task<IReadOnlyCollection<Node>> GetAllByMindMapAsync(int mindMapId)
        {
            // Lấy tất cả node của 1 MindMap (không phân trang)
            var list = await _db.Nodes
                .AsNoTracking()
                .Where(n => n.MindMapId == mindMapId)
                .OrderBy(n => n.ParentNodeId)
                .ThenBy(n => n.OrderIndex)
                .ToListAsync();

            return list;
        }

        public async Task<PaginatedList<Node>> GetByMindMapAsync(int mindMapId, int pageNumber, int pageSize)
        {
            // Phân trang node theo MindMap
            var query = _db.Nodes
                .AsNoTracking()
                .Where(n => n.MindMapId == mindMapId)
                .OrderBy(n => n.ParentNodeId)
                .ThenBy(n => n.OrderIndex);

            return await PaginatedList<Node>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Node> AddAsync(AddNodeRequestDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Chuẩn hoá: 0 => null
            if (dto.ParentNodeId.HasValue && dto.ParentNodeId.Value <= 0)
                dto.ParentNodeId = null;

            // mindmap phải tồn tại
            var mapExists = await _db.MindMaps.AsNoTracking()
                .AnyAsync(m => m.MindMapId == dto.MindMapId);
            if (!mapExists) throw new KeyNotFoundException($"MindMap {dto.MindMapId} not found.");

            // Chỉ check parent khi > 0
            if (dto.ParentNodeId is > 0)
            {
                var parent = await _db.Nodes.AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NodeId == dto.ParentNodeId.Value);
                if (parent == null)
                    throw new KeyNotFoundException($"Parent node {dto.ParentNodeId} not found.");
                if (parent.MindMapId != dto.MindMapId)
                    throw new InvalidOperationException("Parent node must belong to the same MindMap.");
            }
            var node = new Node
            {
                MindMapId = dto.MindMapId,
                ParentNodeId = dto.ParentNodeId,
                Content = dto.Content,
                NodeType = dto.NodeType,
                PositionX = dto.PositionX,
                PositionY = dto.PositionY,
                Color = dto.Color,
                Shape = dto.Shape,
                OrderIndex = dto.OrderIndex
            };

            _db.Nodes.Add(node);
            await _db.SaveChangesAsync();
            return node;
        }

        public async Task<Node?> UpdateAsync(int nodeId, UpdateNodeRequestDTO dto)
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(n => n.NodeId == nodeId);
            if (node == null) return null;

            // Nếu đổi ParentNodeId, kiểm tra hợp lệ và cùng MindMap
            if (dto.ParentNodeId.HasValue && dto.ParentNodeId.Value <= 0)
                dto.ParentNodeId = null;

            // Xử lý đổi ParentNodeId (kể cả gỡ parent về null)
            // So sánh nullable an toàn
            var isChangingParent =
                !(dto.ParentNodeId.HasValue && node.ParentNodeId.HasValue
                  ? dto.ParentNodeId.Value == node.ParentNodeId.Value
                  : dto.ParentNodeId == node.ParentNodeId);

            if (isChangingParent)
            {
                // Self-parent?
                if (dto.ParentNodeId.HasValue && dto.ParentNodeId.Value == node.NodeId)
                    throw new InvalidOperationException("A node cannot be parent of itself.");

                if (dto.ParentNodeId is null)
                {
                    // Gỡ parent => thành root
                    node.ParentNodeId = null;
                }
                else
                {
                    // Validate parent tồn tại & cùng MindMap
                    var newParent = await _db.Nodes
                        .AsNoTracking()
                        .Select(n => new { n.NodeId, n.MindMapId, n.ParentNodeId })
                        .FirstOrDefaultAsync(n => n.NodeId == dto.ParentNodeId.Value);

                    if (newParent == null)
                        throw new KeyNotFoundException($"Parent node {dto.ParentNodeId} not found.");

                    if (newParent.MindMapId != node.MindMapId)
                        throw new InvalidOperationException("Parent node must belong to the same MindMap.");

                    // Chống tạo vòng lặp: duyệt ancestor của newParent
                    int? cursorId = newParent.ParentNodeId;
                    while (cursorId.HasValue)
                    {
                        if (cursorId.Value == node.NodeId)
                            throw new InvalidOperationException("Changing parent would create a cycle.");
                        var cursor = await _db.Nodes
                            .AsNoTracking()
                            .Select(n => new { n.NodeId, n.ParentNodeId })
                            .FirstOrDefaultAsync(n => n.NodeId == cursorId.Value);
                        if (cursor == null) break; // cấu trúc không nhất quán nhưng tránh loop vô hạn
                        cursorId = cursor.ParentNodeId;
                    }

                    node.ParentNodeId = dto.ParentNodeId.Value;
                }
            }

            if (dto.Content != null) node.Content = dto.Content;
            if (dto.NodeType.HasValue) node.NodeType = dto.NodeType.Value;
            if (dto.PositionX.HasValue) node.PositionX = dto.PositionX.Value;
            if (dto.PositionY.HasValue) node.PositionY = dto.PositionY.Value;
            if (dto.Color != null) node.Color = dto.Color;
            if (dto.Shape != null) node.Shape = dto.Shape;
            if (dto.OrderIndex.HasValue) node.OrderIndex = dto.OrderIndex.Value;

            await _db.SaveChangesAsync();
            return node;
        }

        public async Task DeleteAsync(int nodeId)
        {
            var node = await _db.Nodes
                .Include(n => n.ChildNodes)
                .FirstOrDefaultAsync(n => n.NodeId == nodeId);

            if (node == null)
                throw new KeyNotFoundException($"Node {nodeId} not found.");

            // (Tuỳ chọn) quyết định chính sách xoá:
            // A) Cấm xoá nếu có con:
            // if (node.ChildNodes.Any())
            //     throw new InvalidOperationException("Cannot delete a node that has child nodes.");

            // B) Xoá cascade: nếu bạn đã config cascade delete ở Fluent API thì chỉ cần Remove(node)
            // Nếu chưa, có thể xoá con thủ công (cẩn thận branch liên quan).
            // Ở đây minh hoạ đơn giản: Remove node, rely on FK cascade.
            _db.Nodes.Remove(node);
            await _db.SaveChangesAsync();
        }
    }
}
