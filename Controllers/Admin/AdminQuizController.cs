using KidsLearn.Common;
using KidsLearn.DTOs.Quiz;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers.Admin;

[ApiController]
[Route("api/admin/quizzes")]
[Authorize(Policy = "AdminOnly")]
public class AdminQuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public AdminQuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var quizzes = await _quizService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(quizzes));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUpdateQuizDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        try
        {
            var result = await _quizService.CreateQuizAsync(dto);
            return Ok(ApiResponse<object>.Ok(result, "Tạo câu hỏi thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateQuizDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        try
        {
            var result = await _quizService.UpdateQuizAsync(id, dto);
            if (result == null)
                return NotFound(ApiResponse<string>.Fail("Không tìm thấy câu hỏi"));
            return Ok(ApiResponse<object>.Ok(result, "Cập nhật thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _quizService.DeleteQuizAsync(id);
        if (!ok)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy câu hỏi"));
        return Ok(ApiResponse<string>.Ok("OK", "Xóa thành công"));
    }
}