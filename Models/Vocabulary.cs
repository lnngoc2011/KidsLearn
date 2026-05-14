using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Models;

[Table("Vocabulary")]
[Index("UnitId", Name = "IX_Vocabulary_UnitID")]
public partial class Vocabulary
{
    [Key]
    [Column("VocabID")]
    public int VocabId { get; set; }

    [Column("UnitID")]
    public int UnitId { get; set; }

    [StringLength(100)]
    public string Word { get; set; } = null!;

    [StringLength(200)]
    public string Mean { get; set; } = null!;

    [Column("IPA")]
    [StringLength(100)]
    public string? Ipa { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [StringLength(500)]
    public string? Example { get; set; }

    [Column("TTS_Text")]
    [StringLength(500)]
    public string? TtsText { get; set; }

    [ForeignKey("UnitId")]
    [InverseProperty("Vocabularies")]
    public virtual Unit Unit { get; set; } = null!;
}
