using System;

namespace KidsLearn.Models;

// Lưu các huy hiệu MÀ USER ĐÃ ĐẠT
// Quan hệ N-N giữa Users và Badge
// UNIQUE (UserId + BadgeId) - mỗi user chỉ nhận 1 lần mỗi huy hiệu
public partial class UserBadge
{
    public int UserBadgeId { get; set; }

    public int UserId { get; set; }       // FK → Users

    public int BadgeId { get; set; }      // FK → Badge

    public DateTime EarnedAt { get; set; } // Thời gian đạt được

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Badge Badge { get; set; } = null!;
}