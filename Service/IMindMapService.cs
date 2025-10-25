using Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.DTO;
using Model.Pagging;

namespace Service
{
    public interface IMindMapService
    {
        Task<IEnumerable<MindMap>> GetAllAsync();
        Task<MindMap?> GetByIdAsync(int id);
        Task<PaginatedList<MindMap>> GetAll(int pageNumber, int pageSize);
        Task<PaginatedList<MindMap>> GetByUserId(int userId, int pageNumber, int pageSize);

        Task<MindMap> AddAsync(AddMindMapRequestDTO addDto);
        Task<MindMap?> UpdateAsync(int id, UpdateMindMapRequestDTO updateDto);
        Task DeleteAsync(int id);
    }
}
