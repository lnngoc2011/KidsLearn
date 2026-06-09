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


    // Endpoint cho phép tất cả người dùng xem danh sách câu hỏi thuộc một Unit cụ thể
    [HttpPost("review/submit")]
    public async Task<IActionResult> SubmitReview([FromBody] Dictionary<int, int> answers)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        try
        {
            var result = await _quizService.SubmitReviewAsync(userId, answers);
            return Ok(ApiResponse<object>.Ok(result, "Nộp bài Review thành công"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
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

    [HttpGet("review/{progressId:int}")]
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

    // ENDPOINT ÔN TẬP ĐỊNH KỲ & TỔNG HỢP (STUDENT) 

    [HttpGet("review/{gradeId:int}/{reviewNumber:int}")]
    public async Task<IActionResult> GetMidReview(int gradeId, int reviewNumber)
    {
        var data = await _quizService.GetMidReviewAsync(gradeId, reviewNumber);

        return Ok(ApiResponse<object>.Ok(data));
    }

    [HttpGet("final-review/{gradeId:int}")]
    public async Task<IActionResult> GetFinalReview(int gradeId)
    {
        var data = await _quizService.GetFinalReviewAsync(gradeId);

        return Ok(ApiResponse<object>.Ok(data));
    }
// ADMIN

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] CreateUpdateQuizDto dto)
    {
        var fileError = ValidateFiles(dto);
        if (fileError != null)
            return BadRequest(ApiResponse<string>.Fail(fileError));

        try
        {
            var created = await _quizService.CreateQuizAsync(dto);
            return Ok(ApiResponse<object>.Ok(created, "Đã tạo câu hỏi"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateUpdateQuizDto dto)
    {
        var fileError = ValidateFiles(dto);
        if (fileError != null)
            return BadRequest(ApiResponse<string>.Fail(fileError));

        try
        {
            var updated = await _quizService.UpdateQuizAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.Fail("Không tìm thấy câu hỏi"));

            return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _quizService.DeleteQuizAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy câu hỏi"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa câu hỏi"));
    }



    private string? ValidateFiles(CreateUpdateQuizDto dto)
    {
        // 1. Kiểm tra ảnh của câu hỏi nếu loại câu hỏi yêu cầu hình ảnh
        if (dto.QuestionType == "image" && (dto.ImageFile == null || dto.ImageFile.Length == 0))
        {
            return "Không có file ảnh của câu hỏi";
        }

        // 2. Kiểm tra độc lập ảnh của các đáp án (Tránh lỗi logic lồng nhau cũ)
        if (dto.Answers != null && dto.Answers.Any(a => a.AnswerType == "image" && (a.ImageFile == null || a.ImageFile.Length == 0)))
        {
            return "Không có file ảnh của câu trả lời";
        }

        return null;
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}