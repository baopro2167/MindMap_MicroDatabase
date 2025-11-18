using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service;
using Service.Respone;

namespace MindMap_MicroProject.Controllers
{
    [Route("api/v1/mindmaps")]
    [ApiController]
    public class MindMapController : BaseApiController
    {
        private readonly IMindMapService _service;

        public MindMapController(IMindMapService service)
        {
            _service = service;
        }


        /// <summary>tim  MindMap theo mindmapid   </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var map = await _service.GetByIdAsync(id);
            if (map == null)
                return NotFoundResponse("MindMap not found");

            return Success(map, "Fetched successfully");
        }

        /// <summary>tao  MindMap   </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddMindMapRequestDTO dto)
        {
            var created = await _service.AddAsync(dto);
            return Created(created, "MindMap created successfully");

        }
        /// <summary>cap nhat  MindMap   </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMindMapRequestDTO dto)
        {
            if (id <= 0) return BadRequestResponse("id không hợp lệ");

            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFoundResponse($"MindMap with ID {id} not found");

            return Success(updated, "Updated successfully");
        }

        /// <summary>xoa  MindMap   </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Success<object>(null, "Deleted successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse($"MindMap with ID {id} not found");
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }

        /// <summary>Lấy danh sách  mindmap có phân trang </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequestResponse("pageNumber và pageSize phải > 0");

            var page = await _service.GetAll(pageNumber, pageSize);

            var pagination = new PaginationInfo
            {
                PageNumber = page.PageNumber, // hoặc PageIndex nếu PaginatedList của bạn dùng tên này
                PageSize = page.PageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages
            };

            // Trả về đúng envelope ApiResponse<T> của bạn + Pagination
            return Success(page.Items, "Fetched successfully", pagination);
            // Hoặc: return Ok(ApiResponse<MindMap>.FromPaginatedList(page));
        }


        /// <summary>Lấy  toan bo mindmap </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNoPaging()
        {
            var items = await _service.GetAllAsync();
            return Success(items, "Fetched successfully");
        }
    }
}
