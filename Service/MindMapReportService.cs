using Model.Pagging;
using Model;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class MindMapReportService : IMindMapReportService
    {
        private readonly MindMapDbContext _db;
        public MindMapReportService(MindMapDbContext db) => _db = db;

        public async Task<MindMapReport?> GetByIdAsync(int reportId)
        {
            return await _db.MindMapReports
                .AsNoTracking()
             
                .FirstOrDefaultAsync(r => r.ReportId == reportId);
        }

        public async Task<IReadOnlyCollection<MindMapReport>> GetAllByMindMapAsync(int mindMapId)
        {
            var list = await _db.MindMapReports
                .AsNoTracking()
                .Where(r => r.MindMapId == mindMapId)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            return list;
        }

        public async Task<PaginatedList<MindMapReport>> GetByMindMapAsync(int mindMapId, int pageNumber, int pageSize)
        {
            var q = _db.MindMapReports
                .AsNoTracking()
                .Where(r => r.MindMapId == mindMapId)
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.ReportId);

            return await PaginatedList<MindMapReport>.CreateAsync(q, pageNumber, pageSize);
        }

        public async Task<PaginatedList<MindMapReport>> GetByMindMapInRangeAsync(int mindMapId, DateTime fromUtc, DateTime toUtc, int pageNumber, int pageSize)
        {
            var q = _db.MindMapReports
                .AsNoTracking()
                .Where(r => r.MindMapId == mindMapId && r.ReportDate >= fromUtc && r.ReportDate <= toUtc)
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.ReportId);

            return await PaginatedList<MindMapReport>.CreateAsync(q, pageNumber, pageSize);
        }

        public async Task<MindMapReport> AddAsync(AddMindMapReportRequestDTO dto)
        {
            var mapExists = await _db.MindMaps.AsNoTracking().AnyAsync(m => m.MindMapId == dto.MindMapId);
            if (!mapExists) throw new KeyNotFoundException($"MindMap {dto.MindMapId} not found.");

            var report = new MindMapReport
            {
                MindMapId = dto.MindMapId,
                MembershipId = dto.MembershipId,
                ReportDate = dto.ReportDate, // đảm bảo đã là UTC
                ReportContent = dto.ReportContent
            };

            _db.MindMapReports.Add(report);
            await _db.SaveChangesAsync();
            return report;
        }

        public async Task<MindMapReport?> UpdateAsync(int reportId, UpdateMindMapReportRequestDTO dto)
        {
            var report = await _db.MindMapReports.FirstOrDefaultAsync(r => r.ReportId == reportId);
            if (report == null) return null;

            if (dto.MembershipId.HasValue) report.MembershipId = dto.MembershipId.Value;
            if (dto.ReportDate.HasValue) report.ReportDate = dto.ReportDate.Value; // đảm bảo UTC ở caller
            if (dto.ReportContent != null) report.ReportContent = dto.ReportContent;

            await _db.SaveChangesAsync();
            return report;
        }

        public async Task DeleteAsync(int reportId)
        {
            var report = await _db.MindMapReports.FirstOrDefaultAsync(r => r.ReportId == reportId);
            if (report == null) throw new KeyNotFoundException($"Report {reportId} not found.");

            _db.MindMapReports.Remove(report);
            await _db.SaveChangesAsync();
        }
    }
}
