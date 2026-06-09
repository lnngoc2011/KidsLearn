using KidsLearn.DTOs.Vocabulary;

namespace KidsLearn.Services.Interfaces;

public interface IVocabularyService
{
    Task<List<VocabularyDto>> GetByUnitAsync(int unitId);

    // Admin
    Task<VocabularyDto> CreateAsync(CreateUpdateVocabDto dto);
    Task<VocabularyDto?> UpdateAsync(int vocabId, CreateUpdateVocabDto dto);
    Task<bool> DeleteAsync(int vocabId);
}