using System.Text.Json.Serialization;

namespace KidsLearn.DTOs.ExternalApi
{
    public class DictionaryEntry
    {
        [JsonPropertyName("word")]
        public string? Word { get; set; }

        [JsonPropertyName("phonetic")]
        public string? Phonetic { get; set; }   // ⚠️ Thường NULL với nhiều từ

        [JsonPropertyName("phonetics")]
        public List<Phonetic>? Phonetics { get; set; }

        [JsonPropertyName("meanings")]
        public List<Meaning>? Meanings { get; set; }
    }

    public class Phonetic
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }       // ⚠️ Có thể null (phonetics[0] của "hello")

        [JsonPropertyName("audio")]
        public string? Audio { get; set; }      // ⚠️ Có thể là "" hoặc null
    }

    public class Meaning
    {
        [JsonPropertyName("partOfSpeech")]
        public string? PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")]
        public List<Definition>? Definitions { get; set; }
    }

    public class Definition
    {
        [JsonPropertyName("definition")]
        public string? DefinitionText { get; set; }

        [JsonPropertyName("example")]
        public string? Example { get; set; }    // ⚠️ Thường NULL (không phải định nghĩa nào cũng có example)
    }
}
