using System;

namespace KidsLearn.Models;

// Lưu CHUỖI NGÀY HỌC LIÊN TIẾP của mỗi học sinh
// Mỗi user chỉ có 1 record duy nhất (UNIQUE UserId)
public partial class UserStreak
{
    public int StreakId { get; set; }

    public int UserId { get; set; }          

    public int CurrentStreak { get; set; }       // Chuỗi ngày hiện tại

    public int LongestStreak { get; set; }       // Kỷ lục cao nhất

    public DateTime? LastStudyDate { get; set; } // Ngày học gần nhất

    // Navigation property
    public virtual User User { get; set; } = null!;
}