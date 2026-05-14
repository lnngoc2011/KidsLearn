using KidsLearn.DTOs.User;

namespace KidsLearn.Services.Interfaces;

public interface IUserService
{
    // ========== STUDENT (self) ==========
    Task<UserProfileDto?> GetMyProfileAsync(int userId);
    Task<UserDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);

    // ========== ADMIN ==========
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto?> ChangeRoleAsync(int userId, ChangeRoleDto dto);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> ResetPasswordAsync(int userId, string newPassword);
}