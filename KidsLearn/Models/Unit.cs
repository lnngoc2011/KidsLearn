using System;
using System.Collections.Generic;

namespace KidsLearn.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public int GradeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? OrderIndex { get; set; }

    // Navigation properties
    public virtual Grade Grade { get; set; } = null!;

    // ✨ FIX: Đổi từ "Vocabulary"/"Quiz" sang "Vocabularies"/"Quizzes" (số nhiều - chuẩn EF)
    public virtual ICollection<Vocabulary> Vocabularies { get; set; }
        = new List<Vocabulary>();

    public virtual ICollection<Quiz> Quizzes { get; set; }
        = new List<Quiz>();

    public virtual ICollection<LearningProgress> LearningProgresses { get; set; }
        = new List<LearningProgress>();

    // ✨ MỚI: Quan hệ với UserActivity (Continue Learning)
    public virtual ICollection<UserActivity> UserActivities { get; set; }
        = new List<UserActivity>();
}