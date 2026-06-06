using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Models;

[Index("Username", Name = "IX_Users_Username")]
[Index("Username", Name = "UQ_Users_Username", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    public int TotalXP { get; set; } = 0;

    public int Level { get; set; } = 1;

    [InverseProperty("User")]
    public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();

    [InverseProperty("User")]
    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    [InverseProperty("User")]
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

    [InverseProperty("User")]
    public virtual UserStreak? UserStreak { get; set; }
}
