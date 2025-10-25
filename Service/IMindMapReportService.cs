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
    public interface IMindMapReportService
    {
        Task<MindMapReport?> GetByIdAsync(int reportId);
        Task<IReadOnlyCollection<MindMapReport>> GetAllByMindMapAsync(int mindMapId);
        Task<PaginatedList<MindMapReport>> GetByMindMapAsync(int mindMapId, int pageNumber, int pageSize);

        // lọc theo ngày (tuỳ chọn, hữu ích khi xem báo cáo theo range)
        Task<PaginatedList<MindMapReport>> GetByMindMapInRangeAsync(int mindMapId, DateTime fromUtc, DateTime toUtc, int pageNumber, int pageSize);

        Task<MindMapReport> AddAsync(AddMindMapReportRequestDTO dto);
        Task<MindMapReport?> UpdateAsync(int reportId, UpdateMindMapReportRequestDTO dto);
        Task DeleteAsync(int reportId);
    }
}
