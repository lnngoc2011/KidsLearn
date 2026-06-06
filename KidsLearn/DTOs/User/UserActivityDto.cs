namespace KidsLearn.DTOs;

public class UpdateActivityDto
{
    public int UnitId { get; set; }
    public int LastVocabIndex { get; set; }
}

public class UserActivityDto
{
    public int ActivityId { get; set; }
    public int UnitId { get; set; }
    public string UnitTitle { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public string GradeName { get; set; } = string.Empty;
    public int LastVocabIndex { get; set; }
    public DateTime LastAccessedAt { get; set; }
    public bool IsCompleted { get; set; }
    public int TotalVocabs { get; set; }
}