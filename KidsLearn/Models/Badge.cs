using System;
using System.Collections.Generic;

namespace KidsLearn.Models;

// Danh sách HUY HIỆU trong hệ thống
// Admin định nghĩa sẵn, học sinh đạt được sẽ lưu vào UserBadge
public partial class Badge
{
    public int BadgeId { get; set; }

    public string Name { get; set; } = null!;        // VD: "First Step", "On Fire"

    public string? Description { get; set; }         // Mô tả điều kiện

    public string? IconUrl { get; set; }             // Đường dẫn icon

    public string? ConditionType { get; set; }       // VD: "STREAK_7", "LEARN_100_WORDS"

    // Navigation property
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}