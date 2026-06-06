using KidsLearn.Data;
using KidsLearn.DTOs.Grade;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class GradeService : IGradeService
{
    private readonly KidsLearnDbContext _context;

    public GradeService(KidsLearnDbContext context)
    {
        _context = context;
    }

    public async Task<List<GradeDto>> GetAllAsync()
    {
        return await _context.Grades
            .OrderBy(g => g.OrderIndex)
            .Select(g => new GradeDto
            {
                GradeId = g.GradeId,
                GradeName = g.GradeName,
                OrderIndex = g.OrderIndex,
                UnitCount = g.Units.Count
            })
            .ToListAsync();
    }

    public async Task<GradeDto?> GetByIdAsync(int gradeId)
    {
        var grade = await _context.Grades
            .Include(g => g.Units)
            .FirstOrDefaultAsync(g => g.GradeId == gradeId);

        if (grade == null) return null;

        return new GradeDto
        {
            GradeId = grade.GradeId,
            GradeName = grade.GradeName,
            OrderIndex = grade.OrderIndex,
            UnitCount = grade.Units.Count
        };
    }

    public async Task<GradeDto> CreateAsync(CreateUpdateGradeDto dto)
    {
        var grade = new Grade
        {
            GradeName = dto.GradeName,
            OrderIndex = dto.OrderIndex
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        return new GradeDto
        {
            GradeId = grade.GradeId,
            GradeName = grade.GradeName,
            OrderIndex = grade.OrderIndex,
            UnitCount = 0
        };
    }

    public async Task<GradeDto?> UpdateAsync(int gradeId, CreateUpdateGradeDto dto)
    {
        var grade = await _context.Grades.FindAsync(gradeId);
        if (grade == null) return null;

        grade.GradeName = dto.GradeName;
        grade.OrderIndex = dto.OrderIndex;
        await _context.SaveChangesAsync();

        var unitCount = await _context.Units.CountAsync(u => u.GradeId == gradeId);
        return new GradeDto
        {
            GradeId = grade.GradeId,
            GradeName = grade.GradeName,
            OrderIndex = grade.OrderIndex,
            UnitCount = unitCount
        };
    }

    public async Task<bool> DeleteAsync(int gradeId)
    {
        var grade = await _context.Grades.FindAsync(gradeId);
        if (grade == null) return false;

        // Kiểm tra có Unit con không (an toàn hơn CASCADE)
        var hasUnits = await _context.Units.AnyAsync(u => u.GradeId == gradeId);
        if (hasUnits)
            throw new InvalidOperationException(
                "Không thể xóa Grade vì còn Unit con. Hãy xóa các Unit trước.");

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();
        return true;
    }
}