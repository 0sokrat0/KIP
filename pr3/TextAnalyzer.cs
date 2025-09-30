using System;
using System.Collections.Generic;
using System.Text;

namespace pr3
{
  
    public class TextAnalyzer
    {
        private readonly char[] vowels = { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 
                                         'А', 'Е', 'Ё', 'И', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я' };
        
        private readonly char[] consonants = { 'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ',
                                             'Б', 'В', 'Г', 'Д', 'Ж', 'З', 'Й', 'К', 'Л', 'М', 'Н', 'П', 'Р', 'С', 'Т', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ' };
        
        private readonly HashSet<string> conjunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "и", "а", "но", "да", "или", "либо", "что", "чтобы", "как", "так", "же", "тоже", "также",
            "если", "хотя", "пока", "когда", "где", "куда", "откуда", "зачем", "почему", "как", "сколько",
            "потому", "поэтому", "оттого", "отчего", "зато", "однако", "впрочем", "притом", "причем"
        };

        public TextStatistics AnalyzeText(string text, List<char>? removedLetters = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Текст не может быть пустым");
            }

            var statistics = new TextStatistics
            {
                OriginalText = text,
                RemovedLetters = removedLetters ?? new List<char>()
            };

            string processedText = RemoveLetters(text, removedLetters);
            statistics.ProcessedText = processedText;

            var words = ExtractWords(processedText);
            statistics.WordCount = words.Count;

            var wordsWithoutConjunctions = FilterWords(words);
            statistics.WordCountWithoutConjunctions = wordsWithoutConjunctions.Count;

            FindShortestAndLongestWords(words, out string shortest, out string longest);
            statistics.ShortestWord = shortest;
            statistics.LongestWord = longest;

            statistics.SentenceCount = CountSentences(processedText);

            CountVowelsAndConsonants(processedText, out int vowelCount, out int consonantCount);
            statistics.VowelCount = vowelCount;
            statistics.ConsonantCount = consonantCount;

            statistics.LetterFrequency = CreateLetterFrequency(processedText);

            return statistics;
        }

      
        private string RemoveLetters(string text, List<char>? lettersToRemove)
        {
            if (lettersToRemove == null || lettersToRemove.Count == 0)
            {
                return text;
            }

            var result = new StringBuilder(text);
            foreach (char letter in lettersToRemove)
            {
                result.Replace(letter.ToString(), "");
                result.Replace(char.ToUpper(letter).ToString(), "");
                result.Replace(char.ToLower(letter).ToString(), "");
            }
            return result.ToString();
        }

        private List<string> ExtractWords(string text)
        {
            var words = new List<string>();
            var currentWord = new StringBuilder();

            foreach (char c in text)
            {
                if (char.IsLetter(c) || c == '-' || c == '\'')
                {
                    currentWord.Append(c);
                }
                else
                {
                    if (currentWord.Length > 0)
                    {
                        words.Add(currentWord.ToString());
                        currentWord.Clear();
                    }
                }
            }

            if (currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
            }

            return words;
        }

      
        private List<string> FilterWords(List<string> words)
        {
            var filteredWords = new List<string>();

            foreach (string word in words)
            {
                if (IsNumber(word))
                {
                    continue;
                }

                if (conjunctions.Contains(word))
                {
                    continue;
                }

                filteredWords.Add(word);
            }

            return filteredWords;
        }

                private bool IsNumber(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            foreach (char c in word)
            {
                if (!char.IsDigit(c) && c != '.' && c != ',' && c != '-')
                {
                    return false;
                }
            }
            return true;
        }

       
        private void FindShortestAndLongestWords(List<string> words, out string shortest, out string longest)
        {
            shortest = "";
            longest = "";

            if (words.Count == 0)
                return;

            shortest = words[0];
            longest = words[0];

            foreach (string word in words)
            {
                if (word.Length < shortest.Length)
                {
                    shortest = word;
                }
                if (word.Length > longest.Length)
                {
                    longest = word;
                }
            }
        }

        private int CountSentences(string text)
        {
            int count = 0;
            bool inSentence = false;

            foreach (char c in text)
            {
                if (char.IsLetter(c) || char.IsDigit(c))
                {
                    inSentence = true;
                }
                else if ((c == '.' || c == '!' || c == '?') && inSentence)
                {
                    count++;
                    inSentence = false;
                }
            }

            if (inSentence)
            {
                count++;
            }

            return count;
        }

      
        private void CountVowelsAndConsonants(string text, out int vowelCount, out int consonantCount)
        {
            vowelCount = 0;
            consonantCount = 0;

            foreach (char c in text)
            {
                if (Array.IndexOf(vowels, c) >= 0)
                {
                    vowelCount++;
                }
                else if (Array.IndexOf(consonants, c) >= 0)
                {
                    consonantCount++;
                }
            }
        }

      
        private Dictionary<char, int> CreateLetterFrequency(string text)
        {
            var frequency = new Dictionary<char, int>();

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char lowerC = char.ToLower(c);
                    if (frequency.ContainsKey(lowerC))
                    {
                        frequency[lowerC]++;
                    }
                    else
                    {
                        frequency[lowerC] = 1;
                    }
                }
            }

            return frequency;
        }

        
        public bool IsTextValid(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Length >= 100;
        }
    }
}
