using CloudinaryDotNet.Actions;
using KidsLearn.Data;
using KidsLearn.DTOs.Vocabulary;
using KidsLearn.DTOs.ExternalApi;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Text.Json;
using DictionaryEntry = KidsLearn.DTOs.ExternalApi.DictionaryEntry;

namespace KidsLearn.Services;

public class VocabularyService : IVocabularyService
{
    private readonly KidsLearnDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public VocabularyService(KidsLearnDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
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

        // Gọi Dictionary API
        var client = new HttpClient();
        var response = await client.GetAsync(
            $"https://api.dictionaryapi.dev/api/v2/entries/en/{dto.Word}"
        );
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Không tìm thấy từ \"{dto.Word}\" trong từ điển");

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<DictionaryEntry>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var entry = data?[0];

        // Lấy IPA: ưu tiên phonetics[].text có giá trị, fallback về entry.Phonetic
        var ipa = entry?.Phonetics
                        ?.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Text))
                        ?.Text
                  ?? entry?.Phonetic
                  ?? string.Empty;

        // Upload ảnh
        var newAvatarUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);

        var vocab = new Vocabulary
        {
            UnitId = dto.UnitId,
            Word = dto.Word,
            Mean = dto.Mean,
            Ipa = ipa,
            ImageUrl = newAvatarUrl,
            Example = dto.Example,
            TtsText = dto.TtsText
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
        // Gọi Dictionary API
        var client = new HttpClient();
        var response = await client.GetAsync(
            $"https://api.dictionaryapi.dev/api/v2/entries/en/{dto.Word}"
        );
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Không tìm thấy từ \"{dto.Word}\" trong từ điển");

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<DictionaryEntry>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var entry = data?[0];

        // Lấy IPA: ưu tiên phonetics[].text có giá trị, fallback về entry.Phonetic
        var ipa = entry?.Phonetics
                        ?.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Text))
                        ?.Text
                  ?? entry?.Phonetic
                  ?? string.Empty;
        string? newAvatarUrl = vocab.ImageUrl;

        if (dto.ImageFile != null)
        {
            if (!string.IsNullOrEmpty(vocab.ImageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(vocab.ImageUrl);
            }

            newAvatarUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
        }

        vocab.Word = dto.Word;
        vocab.Mean = dto.Mean;
        vocab.Ipa = ipa;
        vocab.ImageUrl = newAvatarUrl;
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
        if (!string.IsNullOrEmpty(vocab.ImageUrl)
    && vocab.ImageUrl.Contains("cloudinary"))
        {
            await _cloudinaryService.DeleteImageAsync(vocab.ImageUrl);
        }
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