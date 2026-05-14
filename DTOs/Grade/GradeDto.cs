namespace KidsLearn.DTOs.Grade;

public class GradeDto
{
    public int GradeId { get; set; }
    public string GradeName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int UnitCount { get; set; }  // Số Unit trong Grade này
}