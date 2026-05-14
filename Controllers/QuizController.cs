using System.Security.Claims;
using KidsLearn.Common;
using KidsLearn.DTOs.Quiz;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    /// <summary>
    /// Lấy quiz theo Unit (không bao gồm IsCorrect)
    /// GET /api/quizzes?unitId=5
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var quizzes = await _quizService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(quizzes));
    }

    /// <summary>
    /// Học sinh nộp bài quiz
    /// POST /api/quizzes/submit
    /// </summary>
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

    /// <summary>
    /// ✨ MỚI: Xem lại bài đã làm (Review Mode)
    /// GET /api/quizzes/review/{progressId}
    /// </summary>
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

    // Helper: Lấy UserId từ JWT Claims
    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}