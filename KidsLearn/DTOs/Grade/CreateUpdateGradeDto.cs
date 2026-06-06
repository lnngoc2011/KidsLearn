using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Grade;

public class CreateUpdateGradeDto
{
    [Required(ErrorMessage = "Tên khối lớp không được để trống")]
    [StringLength(50)]
    public string GradeName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int OrderIndex { get; set; } = 1;
}