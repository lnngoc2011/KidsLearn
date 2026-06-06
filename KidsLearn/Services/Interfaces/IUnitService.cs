using KidsLearn.DTOs.Unit;

namespace KidsLearn.Services.Interfaces;

public interface IUnitService
{
    // Cho học sinh: gradeId nullable - null = lấy tất cả
    Task<List<UnitDto>> GetAllUnitAsync(int? gradeId = null);
    Task<List<UnitDto>> GetUnitByGradeAsync(int gradeId);

    Task<UnitDetailDto?> GetUnitDetailAsync(int unitId);
    Task<List<UnitDto>> SearchAsync(string keyword);

    // Cho Admin
    Task<UnitDto> CreateUnitAsync(CreateUpdateUnitDto dto);
    Task<UnitDto?> UpdateUnitAsync(int unitId, CreateUpdateUnitDto dto);
    Task<bool> DeleteUnitAsync(int unitId);
}