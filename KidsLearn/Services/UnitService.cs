using KidsLearn.Data;
using KidsLearn.DTOs.Quiz;
using KidsLearn.DTOs.Unit;
using KidsLearn.DTOs.Vocabulary;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class UnitService : IUnitService
{
    private readonly KidsLearnDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public UnitService(KidsLearnDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Lấy danh sách Unit, có thể filter theo Grade
    /// ✨ FIX: _context.Unit (sai) → _context.Units (đúng)
    /// ✨ FIX: u.Units.Count (sai) → u.Vocabularies.Count (đúng)
    /// </summary>
    public async Task<List<UnitDto>> GetAllUnitAsync(int? gradeId = null)
    {
        var query = _context.Units.AsQueryable();

        // Filter theo Grade nếu có
        if (gradeId.HasValue)
            query = query.Where(u => u.GradeId == gradeId.Value);

        return await query
            .OrderBy(u => u.GradeId)
            .ThenBy(u => u.OrderIndex ?? 0)
            .Select(u => new UnitDto
            {
                UnitId = u.UnitId,
                GradeId = u.GradeId,
                Title = u.Title,
                Description = u.Description,
                ImageUrl = u.ImageUrl,
                OrderIndex = u.OrderIndex ?? 0,
                VocabCount = u.Vocabularies.Count,  // ✨ FIX
                QuizCount = u.Quizzes.Count          // ✨ FIX
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy chi tiết 1 Unit kèm danh sách Vocabulary và Quiz
    /// ✨ FIX: Trước đây tham chiếu u.Units (vô lý) → giờ map Vocabularies + Quizzes
    /// </summary>
    public async Task<UnitDetailDto?> GetUnitDetailAsync(int unitId)
    {
        var unit = await _context.Units
            .Include(u => u.Vocabularies)
            .Include(u => u.Quizzes)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(u => u.UnitId == unitId);

        if (unit == null) return null;

        return new UnitDetailDto
        {
            UnitId = unit.UnitId,
            GradeId = unit.GradeId,
            Title = unit.Title,
            Description = unit.Description,
            ImageUrl = unit.ImageUrl,
            OrderIndex = unit.OrderIndex ?? 0,

            // Map từ vựng
            Vocabularies = unit.Vocabularies.Select(v => new VocabularyDto
            {
                VocabId = v.VocabId,
                Word = v.Word,
                Mean = v.Mean ?? "",
                Ipa = v.Ipa,
                ImageUrl = v.ImageUrl,
                Example = v.Example,
                TtsText = v.TtsText
            }).ToList(),

            // Map quiz kèm answer (không trả IsCorrect → chống cheat)
            Quizzes = unit.Quizzes.Select(q => new QuizDto
            {
                QuizId = q.QuizId,
                QuestionText = q.QuestionText ?? "",
                QuestionType = q.QuestionType ?? "text",
                ImageUrl = q.ImageUrl,
                TtsText = q.TtsText,
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    AnswerId = a.AnswerId,
                    AnswerText = a.AnswerText ?? "",
                    ImageUrl = a.ImageUrl
                }).ToList()
            }).ToList()
        };
    }

    /// <summary>
    /// Admin tạo Unit mới
    /// </summary>
    public async Task<UnitDto> CreateUnitAsync(CreateUpdateUnitDto dto)
    {
        // Kiểm tra Grade tồn tại
        var gradeExists = await _context.Grades.AnyAsync(g => g.GradeId == dto.GradeId);
        if (!gradeExists)
            throw new KeyNotFoundException($"Không tìm thấy Grade với ID = {dto.GradeId}");
        var newAvatarUrl = await _cloudinaryService.UploadImageAsync(dto.AvatarFile);
        var unit = new Unit
        {
            GradeId = dto.GradeId,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = newAvatarUrl,
            OrderIndex = dto.OrderIndex
        };

        _context.Units.Add(unit);
        await _context.SaveChangesAsync();

        return new UnitDto
        {
            UnitId = unit.UnitId,
            GradeId = unit.GradeId,
            Title = unit.Title,
            Description = unit.Description,
            ImageUrl = unit.ImageUrl,
            OrderIndex = unit.OrderIndex ?? 0,
            VocabCount = 0,
            QuizCount = 0
        };
    }

    /// <summary>
    /// Admin cập nhật Unit
    /// </summary>
    public async Task<UnitDto?> UpdateUnitAsync(int unitId, CreateUpdateUnitDto dto)
    {
        var unit = await _context.Units.FindAsync(unitId);
        if (unit == null) return null;
        var newAvatarUrl = await _cloudinaryService.UploadImageAsync(dto.AvatarFile);

        unit.GradeId = dto.GradeId;
        unit.Title = dto.Title;
        unit.Description = dto.Description;
        unit.ImageUrl = newAvatarUrl;
        unit.OrderIndex = dto.OrderIndex;

        await _context.SaveChangesAsync();

        var vocabCount = await _context.Vocabularies.CountAsync(v => v.UnitId == unitId);
        var quizCount = await _context.Quizzes.CountAsync(q => q.UnitId == unitId);

        return new UnitDto
        {
            UnitId = unit.UnitId,
            GradeId = unit.GradeId,
            Title = unit.Title,
            Description = unit.Description,
            ImageUrl = unit.ImageUrl,
            OrderIndex = unit.OrderIndex ?? 0,
            VocabCount = vocabCount,
            QuizCount = quizCount
        };
    }

    /// <summary>
    /// Admin xóa Unit (CASCADE sẽ xóa Vocabulary, Quiz, Answer luôn)
    /// </summary>
    public async Task<bool> DeleteUnitAsync(int unitId)
    {
        var unit = await _context.Units.FindAsync(unitId);
        if (unit == null) return false;

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<UnitDto>> GetUnitByGradeAsync(int gradeId)
    {
        return GetAllUnitAsync(gradeId);
    }

    public async Task<List<UnitDto>> SearchAsync(string keyword)
    {
        return await _context.Units
            .Where(u => u.Title.Contains(keyword))
            .Select(u => new UnitDto
            {
                UnitId = u.UnitId,
                GradeId = u.GradeId,
                Title = u.Title
            })
            .ToListAsync();
    }
}