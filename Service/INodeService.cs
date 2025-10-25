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
    public interface INodeService
    {
        Task<IReadOnlyCollection<Node>> GetAllByMindMapAsync(int mindMapId);
        Task<NodeReadDTO?> GetSlimByIdAsync(int nodeId, CancellationToken ct = default);
        Task<PaginatedList<Node>> GetByMindMapAsync(int mindMapId, int pageNumber, int pageSize);

        Task<Node> AddAsync(AddNodeRequestDTO addDto);
        Task<Node?> UpdateAsync(int id, UpdateNodeRequestDTO updateDto);
        Task DeleteAsync(int id);

       
       
       
    }
}
