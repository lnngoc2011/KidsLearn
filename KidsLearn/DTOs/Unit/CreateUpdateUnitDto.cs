using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Unit;

// Dùng cho Admin khi thêm mới hoặc cập nhật Unit
// Dùng chung 1 DTO cho cả Create lẫn Update để tránh viết lặp
public class CreateUpdateUnitDto
{
    [Required(ErrorMessage = "GradeId không được để trống")]
    public int GradeId { get; set; }

    [Required(ErrorMessage = "Tiêu đề không được để trống")]
    [StringLength(100, ErrorMessage = "Tiêu đề tối đa 100 ký tự")]
    public string Title { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }
    public IFormFile? AvatarFile { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "OrderIndex phải lớn hơn 0")]
    public int OrderIndex { get; set; } = 1;
}