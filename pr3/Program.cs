using System;
using System.Collections.Generic;

namespace pr3
{
    class Program
    {
        private static TextAnalyzer analyzer = new TextAnalyzer();
        private static List<TextStatistics> statisticsHistory = new List<TextStatistics>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== АНАЛИЗАТОР ТЕКСТА ===");
            Console.WriteLine("Программа для анализа текста и извлечения статистики\n");

            while (true)
            {
                ShowMainMenu();
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AnalyzeNewText();
                        break;
                    case "2":
                        ShowStatisticsHistory();
                        break;
                    case "3":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

                static void ShowMainMenu()
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Анализировать новый текст");
            Console.WriteLine("2. Показать историю статистики");
            Console.WriteLine("3. Выход");
            Console.Write("Ваш выбор: ");
        }


        static void AnalyzeNewText()
        {
            Console.WriteLine("\n=== АНАЛИЗ НОВОГО ТЕКСТА ===");
            
            string? text = GetTextFromUser();
            if (text == null)
            {
                return; 
            }

            List<char> lettersToRemove = GetLettersToRemove();
            
            try
            {
                var statistics = analyzer.AnalyzeText(text, lettersToRemove);
                statisticsHistory.Add(statistics);
                
                statistics.PrintStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при анализе текста: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает текст от пользователя с проверкой минимальной длины
        /// </summary>
        static string? GetTextFromUser()
        {
            Console.WriteLine("Введите текст для анализа (минимум 100 символов):");
            Console.WriteLine("Для многострочного ввода завершите ввод пустой строкой:");
            
            var textBuilder = new System.Text.StringBuilder();
            string? line;
            
            do
            {
                line = Console.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    textBuilder.AppendLine(line);
                }
            } while (!string.IsNullOrEmpty(line));
            
            string text = textBuilder.ToString().Trim();
            
            if (!analyzer.IsTextValid(text))
            {
                Console.WriteLine("Ошибка: Текст должен содержать минимум 100 символов.");
                Console.WriteLine($"Введено символов: {text.Length}");
                Console.WriteLine("Попробовать снова? (y/n): ");
                
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    return GetTextFromUser();
                }
                return null;
            }
            
            return text;
        }

       
        static List<char> GetLettersToRemove()
        {
            var lettersToRemove = new List<char>();
            
            Console.WriteLine("\nХотите удалить определенные буквы из текста? (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                return lettersToRemove;
            }
            
            Console.WriteLine("Введите буквы для удаления (через пробел или без разделителей):");
            string? input = Console.ReadLine();
            
            if (!string.IsNullOrWhiteSpace(input))
            {
                foreach (char c in input)
                {
                    if (char.IsLetter(c) && !lettersToRemove.Contains(char.ToLower(c)))
                    {
                        lettersToRemove.Add(char.ToLower(c));
                    }
                }
            }
            
            if (lettersToRemove.Count > 0)
            {
                Console.WriteLine($"Будут удалены буквы: {string.Join(", ", lettersToRemove)}");
            }
            
            return lettersToRemove;
        }

      
        static void ShowStatisticsHistory()
        {
            Console.WriteLine("\n=== ИСТОРИЯ СТАТИСТИКИ ===");
            
            if (statisticsHistory.Count == 0)
            {
                Console.WriteLine("История пуста. Сначала проанализируйте текст.");
                return;
            }
            
            Console.WriteLine("Краткая статистика по всем проанализированным текстам:");
            for (int i = 0; i < statisticsHistory.Count; i++)
            {
                statisticsHistory[i].PrintBriefStatistics(i);
            }
            
            Console.WriteLine("\nХотите посмотреть подробную статистику для конкретного текста? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                ShowDetailedStatistics();
            }
        }

              static void ShowDetailedStatistics()
        {
            Console.Write($"Введите номер текста (1-{statisticsHistory.Count}): ");
            
            if (int.TryParse(Console.ReadLine(), out int index) && 
                index >= 1 && index <= statisticsHistory.Count)
            {
                Console.WriteLine($"\n=== ПОДРОБНАЯ СТАТИСТИКА ТЕКСТА №{index} ===");
                statisticsHistory[index - 1].PrintStatistics();
            }
            else
            {
                Console.WriteLine("Неверный номер текста.");
            }
        }
    }
}