using System;
using System.Collections.Generic;

namespace pr3
{
        public class TextStatistics
    {
        public string OriginalText { get; set; } = "";
        public string ProcessedText { get; set; } = "";
        public int WordCount { get; set; }
        public int WordCountWithoutConjunctions { get; set; }
        public int SentenceCount { get; set; }
        public int VowelCount { get; set; }
        public int ConsonantCount { get; set; }
        public string ShortestWord { get; set; } = "";
        public string LongestWord { get; set; } = "";
        public Dictionary<char, int> LetterFrequency { get; set; } = new Dictionary<char, int>();
        public DateTime AnalysisTime { get; set; } = DateTime.Now;
        public List<char> RemovedLetters { get; set; } = new List<char>();

              public void PrintStatistics()
        {
            Console.WriteLine("\n=== СТАТИСТИКА АНАЛИЗА ТЕКСТА ===");
            Console.WriteLine($"Время анализа: {AnalysisTime:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine($"Длина исходного текста: {OriginalText.Length} символов");
            Console.WriteLine($"Длина обработанного текста: {ProcessedText.Length} символов");
            
            if (RemovedLetters.Count > 0)
            {
                Console.WriteLine($"Удаленные буквы: {string.Join(", ", RemovedLetters)}");
            }
            
            Console.WriteLine($"\nКоличество слов (всего): {WordCount}");
            Console.WriteLine($"Количество слов (без союзов и чисел): {WordCountWithoutConjunctions}");
            Console.WriteLine($"Количество предложений: {SentenceCount}");
            Console.WriteLine($"Количество гласных букв: {VowelCount}");
            Console.WriteLine($"Количество согласных букв: {ConsonantCount}");
            
            if (!string.IsNullOrEmpty(ShortestWord))
            {
                Console.WriteLine($"Самое короткое слово: \"{ShortestWord}\" (длина: {ShortestWord.Length})");
            }
            
            if (!string.IsNullOrEmpty(LongestWord))
            {
                Console.WriteLine($"Самое длинное слово: \"{LongestWord}\" (длина: {LongestWord.Length})");
            }
            
            Console.WriteLine("\nЧастота встречаемости букв:");
            var sortedLetters = new List<KeyValuePair<char, int>>(LetterFrequency);
            sortedLetters.Sort((x, y) => y.Value.CompareTo(x.Value));
            
            foreach (var letter in sortedLetters)
            {
                Console.WriteLine($"  {letter.Key}: {letter.Value} раз");
            }
        }

     
        public void PrintBriefStatistics(int index)
        {
            Console.WriteLine($"{index + 1}. {AnalysisTime:HH:mm:ss} - Слов: {WordCount}, Предложений: {SentenceCount}, " +
                            $"Гласных: {VowelCount}, Согласных: {ConsonantCount}");
        }
    }
}


