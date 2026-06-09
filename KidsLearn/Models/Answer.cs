using System;
using System.Collections.Generic;

namespace KidsLearn.Models;

public partial class Answer
{
    public int AnswerId { get; set; }

    public int QuizId { get; set; }

    public string? AnswerText { get; set; }
    public string? AnswerType { get; set; }// "text" | "image" | "audio"

    public string? ImageUrl { get; set; }

    public bool IsCorrect { get; set; }

    // Navigation properties
    public virtual Quiz Quiz { get; set; } = null!;

    // Quan hệ với QuizAttemptDetail (đáp án này có bao nhiêu học sinh đã chọn)
    public virtual ICollection<QuizAttemptDetail> QuizAttemptDetails { get; set; }
        = new List<QuizAttemptDetail>();
}