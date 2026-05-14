using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Models;

[Table("Grade")]
[Index("OrderIndex", Name = "IX_Grade_OrderIndex")]
[Index("GradeName", Name = "UQ_Grade_GradeName", IsUnique = true)]
public partial class Grade
{
    [Key]
    [Column("GradeID")]
    public int GradeId { get; set; }

    [StringLength(50)]
    public string GradeName { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    public int OrderIndex { get; set; }

    [InverseProperty("Grade")]
    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
