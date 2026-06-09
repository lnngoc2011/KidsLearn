using System;
using System.Collections.Generic;

namespace KidsLearn.Models;

// Lưu CHI TIẾT từng câu trả lời trong 1 lượt làm Quiz
// Phục vụ chức năng "Xem lại bài làm" (Review Mode)
public partial class QuizAttemptDetail
{
    public int DetailId { get; set; }

    public int ProgressId { get; set; }    

    public int QuizId { get; set; }         

    public int? SelectedAnswerId { get; set; }

    public bool IsCorrect { get; set; }       

    // Navigation properties
    public virtual LearningProgress Progress { get; set; } = null!;
    public virtual Quiz Quiz { get; set; } = null!;
    public virtual Answer? SelectedAnswer { get; set; }
}