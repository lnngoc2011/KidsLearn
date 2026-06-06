using System;

namespace KidsLearn.Models;

// Theo dõi UNIT ĐANG HỌC của mỗi học sinh
// Phục vụ chức năng "Tiếp tục học" (Continue Learning)
// UNIQUE (UserId + UnitId) - mỗi user có 1 record cho mỗi Unit
public partial class UserActivity
{
    public int ActivityId { get; set; }

    public int UserId { get; set; }              // FK → Users

    public int UnitId { get; set; }              // FK → Unit

    public int LastVocabIndex { get; set; }      // Vị trí từ vựng học dở (VD: 5/15)

    public DateTime LastAccessedAt { get; set; } // Lần truy cập gần nhất

    public bool IsCompleted { get; set; }        // Đã hoàn thành Unit (Score >= 70)

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Unit Unit { get; set; } = null!;
}