using System;
using System.Collections.Generic;

namespace KidsLearn.Models;

public partial class Quiz
{
    public int QuizId { get; set; }

    public int UnitId { get; set; }

    public string? QuestionText { get; set; }

    public string? QuestionType { get; set; }   

    public string? ImageUrl { get; set; }

    public string? TtsText { get; set; }     

    // Navigation properties
    //Đổi "Answer" → "Answers" cho chuẩn convention
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Unit Unit { get; set; } = null!;

    //Quan hệ với QuizAttemptDetail (Review Mode)
    public virtual ICollection<QuizAttemptDetail> QuizAttemptDetails { get; set; }
        = new List<QuizAttemptDetail>();
}