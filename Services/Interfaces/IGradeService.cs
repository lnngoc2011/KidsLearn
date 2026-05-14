using KidsLearn.DTOs.Grade;

namespace KidsLearn.Services.Interfaces;

public interface IGradeService
{
    Task<List<GradeDto>> GetAllAsync();
    Task<GradeDto?> GetByIdAsync(int gradeId);
    Task<GradeDto> CreateAsync(CreateUpdateGradeDto dto);
    Task<GradeDto?> UpdateAsync(int gradeId, CreateUpdateGradeDto dto);
    Task<bool> DeleteAsync(int gradeId);
}