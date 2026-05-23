namespace KidsLearn.DTOs.Game;
// Kết quả sau khi cộng XP
public class XPUpdateResultDto
{
    public int XPGained { get; set; }       // XP vừa nhận được
    public int TotalXP { get; set; }        // Tổng XP hiện tại
    public int Level { get; set; }          // Level hiện tại (sau khi update)
    public bool LeveledUp { get; set; }     // Có lên level không
    public string LevelName { get; set; } = string.Empty; // Beginner/Explorer/...
    public int XPToNextLevel { get; set; }  // XP cần để lên cấp tiếp theo
}
 