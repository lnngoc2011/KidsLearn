namespace KidsLearn.DTOs.Vocabulary;

// Thông tin từ vựng trả về cho học sinh
public class VocabularyDto
{
    public int VocabId { get; set; }
    public string Word { get; set; } = string.Empty;
    public string Mean { get; set; } = string.Empty;
    public string? Ipa { get; set; }         
    public string? ImageUrl { get; set; }    
    public string? Example { get; set; }     
    public string? TtsText { get; set; }     
}