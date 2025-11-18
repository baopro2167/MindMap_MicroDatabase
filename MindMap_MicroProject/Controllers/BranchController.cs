using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Respone;
using Service;

namespace MindMap_MicroProject.Controllers
{
    [Route("api/v1/branchs")]
    [ApiController]
    public class BranchController : BaseApiController
    {
        private readonly IBranchService _service;

        public BranchController(IBranchService service)
        {
            _service = service;
        }

        /// <summary>Lấy  branch theo Branch id </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var br = await _service.GetByIdAsync(id);
            if (br == null) return NotFoundResponse($"Branch {id} not found");
            return Success(br, "Fetched successfully");
        }

        /// <summary>Lấy danh sách  branch theo mindMapId id , có phân trang </summary>
        [HttpGet("mindmap/{mindMapId:int}")]
        public async Task<IActionResult> GetByMindMap(int mindMapId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            if (pageNumber <= 0 || pageSize <= 0) return BadRequestResponse("pageNumber và pageSize phải > 0");

            var page = await _service.GetByMindMapId(mindMapId, pageNumber, pageSize);

            var pagination = new PaginationInfo
            {
                PageNumber = page.PageNumber,  // đổi sang PageIndex nếu PaginatedList của bạn dùng tên khác
                PageSize = page.PageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages
            };

            return Success(page.Items, "Fetched successfully", pagination);
            // Hoặc: return Ok(ApiResponse<Branch>.FromPaginatedList(page));
        }

        /// <summary>Lấy toàn bộ brand theo mindMapId id </summary>
        [HttpGet("mindmap/{mindMapId:int}/all")]
        public async Task<IActionResult> GetAllByMindMap(int mindMapId)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            var list = await _service.GetAllByMindMapAsync(mindMapId);
            return Success(list, "Fetched successfully");
        }

        /// <summary>tao brand   </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddBranchRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Validation Failed", ModelState);

            try
            {
                var created = await _service.AddAsync(dto);
                return Created(created, "Branch created successfully");
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

        /// <summary>cap nhat brand   </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchRequestDTO dto)
        {
            if (id <= 0) return BadRequestResponse("id không hợp lệ");
            if (!ModelState.IsValid)
                return BadRequestResponse("Validation Failed", ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFoundResponse($"Branch {id} not found");
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

        /// <summary>xoa brand   </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequestResponse("id không hợp lệ");

            try
            {
                await _service.DeleteAsync(id);
                return Success<object>(null, "Deleted successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse($"Branch {id} not found");
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }
    }
}
