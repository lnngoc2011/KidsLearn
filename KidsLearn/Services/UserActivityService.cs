using Microsoft.EntityFrameworkCore;
using KidsLearn.Data;
using KidsLearn.DTOs;
using KidsLearn.Models;

namespace KidsLearn.Services;

public interface IUserActivityService
{
    Task UpdateActivityAsync(int userId, UpdateActivityDto dto);
    Task<UserActivityDto?> GetByUnitAsync(int userId, int unitId);
    Task<List<UserActivityDto>> GetInProgressAsync(int userId);
    Task<UserActivityDto?> GetLatestAsync(int userId);
}

public class UserActivityService : IUserActivityService
{
    private readonly KidsLearnDbContext _context;

    public UserActivityService(KidsLearnDbContext context)
    {
        _context = context;
    }

    public async Task UpdateActivityAsync(int userId, UpdateActivityDto dto)
    {
        var activity = await _context.UserActivities
            .FirstOrDefaultAsync(a => a.UserId == userId && a.UnitId == dto.UnitId);

        if (activity == null)
        {
            activity = new UserActivity
            {
                UserId = userId,
                UnitId = dto.UnitId,
                LastVocabIndex = dto.LastVocabIndex,
                LastAccessedAt = DateTime.Now,
                IsCompleted = false
            };
            _context.UserActivities.Add(activity);
        }
        else
        {
            activity.LastVocabIndex = dto.LastVocabIndex;
            activity.LastAccessedAt = DateTime.Now;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<UserActivityDto?> GetByUnitAsync(int userId, int unitId)
    {
        var activity = await _context.UserActivities
            .Include(a => a.Unit).ThenInclude(u => u.Grade)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.UnitId == unitId);

        if (activity == null) return null;

        var totalVocabs = await _context.Vocabularies.CountAsync(v => v.UnitId == unitId);

        return new UserActivityDto
        {
            ActivityId = activity.ActivityId,
            UnitId = activity.UnitId,
            UnitTitle = activity.Unit.Title,
            GradeId = activity.Unit.GradeId,
            GradeName = activity.Unit.Grade.GradeName,
            LastVocabIndex = activity.LastVocabIndex,
            LastAccessedAt = activity.LastAccessedAt,
            IsCompleted = activity.IsCompleted,
            TotalVocabs = totalVocabs
        };
    }

    public async Task<List<UserActivityDto>> GetInProgressAsync(int userId)
    {
        var activities = await _context.UserActivities
            .Include(a => a.Unit).ThenInclude(u => u.Grade)
            .Where(a => a.UserId == userId && !a.IsCompleted)
            .OrderByDescending(a => a.LastAccessedAt)
            .ToListAsync();

        var result = new List<UserActivityDto>();
        foreach (var a in activities)
        {
            var total = await _context.Vocabularies.CountAsync(v => v.UnitId == a.UnitId);
            result.Add(new UserActivityDto
            {
                ActivityId = a.ActivityId,
                UnitId = a.UnitId,
                UnitTitle = a.Unit.Title,
                GradeId = a.Unit.GradeId,
                GradeName = a.Unit.Grade.GradeName,
                LastVocabIndex = a.LastVocabIndex,
                LastAccessedAt = a.LastAccessedAt,
                IsCompleted = a.IsCompleted,
                TotalVocabs = total
            });
        }
        return result;
    }

    public async Task<UserActivityDto?> GetLatestAsync(int userId)
    {
        var activity = await _context.UserActivities
            .Include(a => a.Unit).ThenInclude(u => u.Grade)
            .Where(a => a.UserId == userId && !a.IsCompleted)
            .OrderByDescending(a => a.LastAccessedAt)
            .FirstOrDefaultAsync();

        if (activity == null) return null;

        var totalVocabs = await _context.Vocabularies.CountAsync(v => v.UnitId == activity.UnitId);

        return new UserActivityDto
        {
            ActivityId = activity.ActivityId,
            UnitId = activity.UnitId,
            UnitTitle = activity.Unit.Title,
            GradeId = activity.Unit.GradeId,
            GradeName = activity.Unit.Grade.GradeName,
            LastVocabIndex = activity.LastVocabIndex,
            LastAccessedAt = activity.LastAccessedAt,
            IsCompleted = activity.IsCompleted,
            TotalVocabs = totalVocabs
        };
    }
}