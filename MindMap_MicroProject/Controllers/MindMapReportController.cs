using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Respone;
using Service;

namespace MindMap_MicroProject.Controllers
{
    [Route("api/mindmapreports")]
    [ApiController]
    public class MindMapReportController : BaseApiController
    {
        private readonly IMindMapReportService _service;
        public MindMapReportController(IMindMapReportService service) => _service = service;

        /// <summary> tim report theo reportId   </summary>
        [HttpGet("{reportId:int}")]
        public async Task<IActionResult> GetById(int reportId)
        {
            var r = await _service.GetByIdAsync(reportId);
            if (r == null) return NotFoundResponse($"Report {reportId} not found");
            return Success(r, "Fetched successfully");
        }

        /// <summary> tim  toan bo report theo mindMapId co phan trang   </summary>
        [HttpGet("mindmap/{mindMapId:int}")]
        public async Task<IActionResult> GetByMindMap(int mindMapId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            if (pageNumber <= 0 || pageSize <= 0) return BadRequestResponse("pageNumber và pageSize phải > 0");

            var page = await _service.GetByMindMapAsync(mindMapId, pageNumber, pageSize);
            var pagination = new PaginationInfo
            {
                PageNumber = page.PageNumber,   // hoặc PageIndex nếu class của bạn khác tên
                PageSize = page.PageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages
            };
            return Success(page.Items, "Fetched successfully", pagination);
        }

        /// <summary> tim  toan bo report, theo ngay thang , theo mindMapId co phan trang   </summary>
        [HttpGet("mindmap/{mindMapId:int}/range")]
        public async Task<IActionResult> GetByMindMapInRange(int mindMapId, [FromQuery] DateTime from, [FromQuery] DateTime to,
                                                             [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            if (from == default || to == default || from > to) return BadRequestResponse("Khoảng thời gian không hợp lệ");
            if (pageNumber <= 0 || pageSize <= 0) return BadRequestResponse("pageNumber và pageSize phải > 0");

            var page = await _service.GetByMindMapInRangeAsync(mindMapId, from, to, pageNumber, pageSize);
            var pagination = new PaginationInfo
            {
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages
            };
            return Success(page.Items, "Fetched successfully", pagination);
        }

         /// <summary> tim  toan bo report theo mindMapId ko phan trang   </summary>
        [HttpGet("mindmap/{mindMapId:int}/all")]
        public async Task<IActionResult> GetAllByMindMap(int mindMapId)
        {
            if (mindMapId <= 0) return BadRequestResponse("mindMapId không hợp lệ");
            var list = await _service.GetAllByMindMapAsync(mindMapId);
            return Success(list, "Fetched successfully");
        }

        /// <summary> tao report    </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddMindMapReportRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequestResponse("Validation Failed", ModelState);

            try
            {
                var created = await _service.AddAsync(dto);
                return Created(created, "Report created successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }

        /// <summary> cap nhat report    </summary>
        [HttpPut("{reportId:int}")]
        public async Task<IActionResult> Update(int reportId, [FromBody] UpdateMindMapReportRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequestResponse("Validation Failed", ModelState);

            try
            {
                var updated = await _service.UpdateAsync(reportId, dto);
                if (updated == null) return NotFoundResponse($"Report {reportId} not found");
                return Success(updated, "Updated successfully");
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }

        /// <summary> xóa report    </summary>
        [HttpDelete("{reportId:int}")]
        public async Task<IActionResult> Delete(int reportId)
        {
            try
            {
                await _service.DeleteAsync(reportId);
                return Success<object>(null, "Deleted successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse($"Report {reportId} not found");
            }
            catch (Exception ex)
            {
                return ServerErrorResponse("Unexpected error", ex.Message);
            }
        }
    }
}
