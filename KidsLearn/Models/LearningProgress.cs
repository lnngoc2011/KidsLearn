using System;
using System.Collections.Generic;

namespace KidsLearn.Models;

public partial class LearningProgress
{
    public int ProgressId { get; set; }

    public int UserId { get; set; }

    public int UnitId { get; set; }

    // Đổi double → decimal vì DB lưu decimal(5,2)
    // Nếu để double sẽ sai khi map: SQL Server không tự cast double <-> decimal
    public decimal Score { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual Unit Unit { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    // Chi tiết các câu trả lời của lượt làm bài này (Review Mode)
    public virtual ICollection<QuizAttemptDetail> QuizAttemptDetails { get; set; }
        = new List<QuizAttemptDetail>();
}