using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Vocabulary;

// Dùng cho Admin khi thêm mới hoặc cập nhật từ vựng
public class CreateUpdateVocabDto
{
    [Required(ErrorMessage = "UnitId không được để trống")]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Từ vựng không được để trống")]
    [StringLength(100, ErrorMessage = "Từ vựng tối đa 100 ký tự")]
    public string Word { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nghĩa không được để trống")]
    [StringLength(255, ErrorMessage = "Nghĩa tối đa 255 ký tự")]
    public string Mean { get; set; } = string.Empty;

    public IFormFile? ImageFile { get; set; }

    [StringLength(255)]
    public string? Example { get; set; }

    // ✨ FIX: TtsText thay vì AudioUrl
    [StringLength(255)]
    public string? TtsText { get; set; }
}
public class CreateUpdateImageVocabDto
{
    [Required(ErrorMessage = "UnitId không được để trống")]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Từ vựng không được để trống")]
    [StringLength(100, ErrorMessage = "Từ vựng tối đa 100 ký tự")]
    public string Word { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nghĩa không được để trống")]
    [StringLength(255, ErrorMessage = "Nghĩa tối đa 255 ký tự")]
    public string Mean { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Ipa { get; set; }

    public IFormFile? ImageFile { get; set; }

    [StringLength(255)]
    public string? Example { get; set; }

    // ✨ FIX: TtsText thay vì AudioUrl
    [StringLength(255)]
    public string? TtsText { get; set; }
}