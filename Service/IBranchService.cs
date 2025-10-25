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
    public interface IBranchService
    {
        Task<IReadOnlyCollection<Branch>> GetAllByMindMapAsync(int mindMapId);
        Task<Branch?> GetByIdAsync(int id);
        Task<PaginatedList<Branch>> GetByMindMapId(int mindMapId, int pageNumber, int pageSize);

        Task<Branch> AddAsync(AddBranchRequestDTO addDto);
        Task<Branch?> UpdateAsync(int id, UpdateBranchRequestDTO updateDto);
        Task DeleteAsync(int id);
    }
}

