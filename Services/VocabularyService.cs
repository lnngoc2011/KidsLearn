using KidsLearn.Data;
using KidsLearn.DTOs.Vocabulary;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class VocabularyService : IVocabularyService
{
    private readonly KidsLearnDbContext _context;

    public VocabularyService(KidsLearnDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách từ vựng của 1 Unit
    /// ✨ FIX: rename param lessonId → unitId, thêm TtsText
    /// </summary>
    public async Task<List<VocabularyDto>> GetByUnitAsync(int unitId)
    {
        return await _context.Vocabularies
            .Where(v => v.UnitId == unitId)
            .Select(v => new VocabularyDto
            {
                VocabId = v.VocabId,
                Word = v.Word,
                Mean = v.Mean ?? "",
                Ipa = v.Ipa,
                ImageUrl = v.ImageUrl,
                Example = v.Example,
                TtsText = v.TtsText    // ✨ FIX: thêm TtsText (đã thiếu)
            })
            .ToListAsync();
    }

    /// <summary>
    /// Admin tạo từ vựng mới
    /// </summary>
    public async Task<VocabularyDto> CreateAsync(CreateUpdateVocabDto dto)
    {
        // Kiểm tra Unit tồn tại
        var unitExists = await _context.Units.AnyAsync(u => u.UnitId == dto.UnitId);
        if (!unitExists)
            throw new KeyNotFoundException($"Không tìm thấy Unit với ID = {dto.UnitId}");

        var vocab = new Vocabulary
        {
            UnitId = dto.UnitId,
            Word = dto.Word,
            Mean = dto.Mean,
            Ipa = dto.Ipa,
            ImageUrl = dto.ImageUrl,
            Example = dto.Example,
            TtsText = dto.TtsText    // ✨ FIX
        };

        _context.Vocabularies.Add(vocab);
        await _context.SaveChangesAsync();

        return MapToDto(vocab);
    }

    /// <summary>
    /// Admin cập nhật từ vựng
    /// </summary>
    public async Task<VocabularyDto?> UpdateAsync(int vocabId, CreateUpdateVocabDto dto)
    {
        var vocab = await _context.Vocabularies.FindAsync(vocabId);
        if (vocab == null) return null;

        vocab.Word = dto.Word;
        vocab.Mean = dto.Mean;
        vocab.Ipa = dto.Ipa;
        vocab.ImageUrl = dto.ImageUrl;
        vocab.Example = dto.Example;
        vocab.TtsText = dto.TtsText;    // ✨ FIX

        await _context.SaveChangesAsync();
        return MapToDto(vocab);
    }

    /// <summary>
    /// Admin xóa từ vựng
    /// </summary>
    public async Task<bool> DeleteAsync(int vocabId)
    {
        var vocab = await _context.Vocabularies.FindAsync(vocabId);
        if (vocab == null) return false;

        _context.Vocabularies.Remove(vocab);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper - tránh viết lặp code map
    private static VocabularyDto MapToDto(Vocabulary v) => new()
    {
        VocabId = v.VocabId,
        Word = v.Word,
        Mean = v.Mean ?? "",
        Ipa = v.Ipa,
        ImageUrl = v.ImageUrl,
        Example = v.Example,
        TtsText = v.TtsText
    };
}