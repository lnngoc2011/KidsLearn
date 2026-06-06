namespace KidsLearn.DTOs.Vocabulary;

// Thông tin từ vựng trả về cho học sinh
// ✨ FIX: Đổi từ AudioUrl → TtsText (Web Speech API tự đọc, không dùng file mp3)
public class VocabularyDto
{
    public int VocabId { get; set; }
    public string Word { get; set; } = string.Empty;
    public string Mean { get; set; } = string.Empty;
    public string? Ipa { get; set; }         // Ký hiệu phiên âm /dɒɡ/
    public string? ImageUrl { get; set; }    // URL hình ảnh minh họa
    public string? Example { get; set; }     // Câu ví dụ
    public string? TtsText { get; set; }     // Văn bản cho Text-to-Speech (Web Speech API)
}