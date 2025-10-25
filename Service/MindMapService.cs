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
   public class MindMapService : IMindMapService
    {
        private readonly MindMapDbContext _db;

        public MindMapService(MindMapDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<MindMap>> GetAllAsync()
        {
            return await _db.MindMaps
                .AsNoTracking()
                .Include(m => m.Nodes)
                .Include(m => m.Branches)
                .ToListAsync();
        }

        public async Task<MindMap?> GetByIdAsync(int id)
        {
            return await _db.MindMaps
                .Include(m => m.Nodes)
                .Include(m => m.Branches)
                .Include(m => m.Reports)
                .FirstOrDefaultAsync(m => m.MindMapId == id);
        }

        public async Task<PaginatedList<MindMap>> GetAll(int pageNumber, int pageSize)
        {
            var q = _db.MindMaps.AsNoTracking().OrderByDescending(m => m.MindMapId);
            return await PaginatedList<MindMap>.CreateAsync(q, pageNumber, pageSize);
        }

        public async Task<PaginatedList<MindMap>> GetByUserId(int userId, int pageNumber, int pageSize)
        {
            var q = _db.MindMaps.AsNoTracking()
                                 .Where(m => m.CreatedByUser == userId)
                                 .OrderByDescending(m => m.MindMapId);
            return await PaginatedList<MindMap>.CreateAsync(q, pageNumber, pageSize);
        }

        public async Task<MindMap> AddAsync(AddMindMapRequestDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var map = new MindMap
            {
                Description = dto.Description,
                CreatedByUser = dto.CreatedBy,
                LessonId = dto.LessonId,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow
            };

            _db.MindMaps.Add(map);
            await _db.SaveChangesAsync();
            return map;
        }

        public async Task<MindMap?> UpdateAsync(int id, UpdateMindMapRequestDTO dto)
        {
            var map = await _db.MindMaps.FindAsync(id);
            if (map == null) return null;

            map.Description = dto.Description;
            map.IsPublished = dto.IsPublished;
            await _db.SaveChangesAsync();
            return map;
        }

        public async Task DeleteAsync(int id)
        {
            var map = await _db.MindMaps.FindAsync(id);
            if (map == null) throw new KeyNotFoundException($"MindMap {id} not found.");

            _db.MindMaps.Remove(map);
            await _db.SaveChangesAsync();
        }
    }
}
