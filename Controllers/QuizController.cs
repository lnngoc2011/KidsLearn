using KidsLearn.Common;
using KidsLearn.DTOs.Quiz;
using KidsLearn.Services;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/quizzes")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var quizzes = await _quizService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(quizzes));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitQuizRequestDto request)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        try
        {
            var result = await _quizService.SubmitQuizAsync(userId, request);
            return Ok(ApiResponse<object>.Ok(result, "Nộp bài thành công"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpGet("review/{progressId}")]
    public async Task<IActionResult> Review(int progressId)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var result = await _quizService.GetReviewAsync(userId, progressId);
        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy bài làm này"));

        return Ok(ApiResponse<object>.Ok(result));
    }

    // ===== ADMIN =====

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateImageQuizDto dto)
    {
        // Validation cơ bản: phải có đúng 1 đáp án đúng
        var correctCount = dto.Answers?.Count(a => a.IsCorrect) ?? 0;
        if (correctCount != 1)
            return BadRequest(ApiResponse<string>.Fail("Câu hỏi phải có đúng 1 đáp án đúng"));
        if (dto.ImageFile == null || dto.ImageFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        var quizDto =
            new CreateUpdateQuizDto
            {
                UnitId = dto.UnitId,
                QuestionText = dto.QuestionText,
                QuestionType = dto.QuestionType,
                TtsText = dto.TtsText,
                ImageFile = dto.ImageFile,
                Answers = dto.Answers
            };
        try
        {
            var created = await _quizService.CreateQuizAsync(quizDto);
            return Ok(ApiResponse<object>.Ok(created, "Đã tạo câu hỏi"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateImageQuizDto dto)
    {
        var correctCount = dto.Answers?.Count(a => a.IsCorrect) ?? 0;
        if(dto.ImageFile == null || dto.ImageFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        if (correctCount != 1)
            return BadRequest(ApiResponse<string>.Fail("Câu hỏi phải có đúng 1 đáp án đúng"));
        if (dto.ImageFile == null || dto.ImageFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        
        var quizDto =
            new CreateUpdateQuizDto
            {
                UnitId = dto.UnitId,
                QuestionText = dto.QuestionText,
                QuestionType = dto.QuestionType,
                TtsText = dto.TtsText,
                ImageFile = dto.ImageFile,
                Answers = dto.Answers
            };

        var updated = await _quizService.UpdateQuizAsync(id, quizDto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy câu hỏi"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _quizService.DeleteQuizAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy câu hỏi"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa câu hỏi"));
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}