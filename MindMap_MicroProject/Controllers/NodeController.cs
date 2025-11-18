using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Respone;
using Service;

namespace MindMap_MicroProject.Controllers
{
    [Route("api/v1/nodes")]
    [ApiController]
    public class NodeController : BaseApiController
    {
        private readonly INodeService _service;

        public NodeController(INodeService service)
        {
            _service = service;
        }


        /// <summary>tim  Node theo node id  </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var node = await _service.GetSlimByIdAsync(id);
            if (node == null)
                return NotFoundResponse($"Node {id} not found");
            return Success(node, "Fetched successfully");
        }

        /// <summary>getall  Node theo mindMapId id co phan trang  </summary>
        [HttpGet("mindmap/{mindMapId:int}")]
        public async Task<IActionResult> GetByMindMap([FromRoute] int mindMapId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            if (pageNumber <= 0 || pageSize <= 0) return BadRequestResponse("pageNumber và pageSize phải > 0");

            var page = await _service.GetByMindMapAsync(mindMapId, pageNumber, pageSize);

            var pagination = new PaginationInfo
            {
                PageNumber = page.PageNumber, // đổi sang PageIndex nếu PaginatedList bạn dùng tên khác
                PageSize = page.PageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages
            };

            return Success(page.Items, "Fetched successfully", pagination);
            // Hoặc: return Ok(ApiResponse<Node>.FromPaginatedList(page));
        }

        /// <summary> getall Node theo mindMapId id   </summary>
        [HttpGet("mindmap/{mindMapId:int}/all")]
        public async Task<IActionResult> GetAllByMindMap([FromRoute] int mindMapId)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            var nodes = await _service.GetAllByMindMapAsync(mindMapId);
            return Success(nodes, "Fetched successfully");
        }

        /// <summary>tao  Node   </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddNodeRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Validation Failed", ModelState);

            try
            {
                var created = await _service.AddAsync(dto);
                return Created(created, "Node created successfully");
            }
            catch (KeyNotFoundException ex)
            {
                // MindMap hoặc Parent không tồn tại
                return NotFoundResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Business rule: parent khác MindMap, tự làm parent chính nó, v.v.
                return BadRequestResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }

        /// <summary> cap nhat  Node   </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateNodeRequestDTO dto)
        {
            if (id <= 0) return BadRequestResponse("id không hợp lệ");
            if (!ModelState.IsValid)
                return BadRequestResponse("Validation Failed", ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFoundResponse($"Node {id} not found");

                return Success(updated, "Updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequestResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }

        /// <summary> xoa  Node   </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequestResponse("id không hợp lệ");

            try
            {
                await _service.DeleteAsync(id);
                return Success<object>(null, "Deleted successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse($"Node {id} not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequestResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }
    }
}
