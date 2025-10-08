using System;
using pr5.Services;

namespace pr5
{
    class Program
    {
        private static UniversityService universityService = new UniversityService();

        static void Main(string[] args)
        {
            Console.WriteLine("СИСТЕМА УПРАВЛЕНИЯ УНИВЕРСИТЕТОМ");
            
            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        StudentMenu();
                        break;
                    case "2":
                        TeacherMenu();
                        break;
                    case "3":
                        CourseMenu();
                        break;
                    case "4":
                        GradeMenu();
                        break;
                    case "5":
                        ShowAllData();
                        break;
                    case "0":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор! Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\nГЛАВНОЕ МЕНЮ");
            Console.WriteLine("1. Управление студентами");
            Console.WriteLine("2. Управление преподавателями");
            Console.WriteLine("3. Управление курсами");
            Console.WriteLine("4. Система оценок");
            Console.WriteLine("5. Показать все данные");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите опцию: ");
        }

        static void StudentMenu()
        {
            while (true)
            {
                Console.WriteLine("\nУПРАВЛЕНИЕ СТУДЕНТАМИ");
                Console.WriteLine("1. Добавить студента");
                Console.WriteLine("2. Показать всех студентов");
                Console.WriteLine("3. Показать детали студента");
                Console.WriteLine("4. Записать на курс");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        universityService.ShowAllStudents();
                        break;
                    case "3":
                        ShowStudentDetails();
                        break;
                    case "4":
                        EnrollStudentInCourse();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void TeacherMenu()
        {
            while (true)
            {
                Console.WriteLine("\nУПРАВЛЕНИЕ ПРЕПОДАВАТЕЛЯМИ");
                Console.WriteLine("1. Добавить преподавателя");
                Console.WriteLine("2. Показать всех преподавателей");
                Console.WriteLine("3. Показать детали преподавателя");
                Console.WriteLine("4. Назначить на курс");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddTeacher();
                        break;
                    case "2":
                        universityService.ShowAllTeachers();
                        break;
                    case "3":
                        ShowTeacherDetails();
                        break;
                    case "4":
                        AssignTeacherToCourse();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void CourseMenu()
        {
            while (true)
            {
                Console.WriteLine("\nУПРАВЛЕНИЕ КУРСАМИ");
                Console.WriteLine("1. Создать курс");
                Console.WriteLine("2. Показать все курсы");
                Console.WriteLine("3. Показать детали курса");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddCourse();
                        break;
                    case "2":
                        universityService.ShowAllCourses();
                        break;
                    case "3":
                        ShowCourseDetails();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void GradeMenu()
        {
            while (true)
            {
                Console.WriteLine("\nСИСТЕМА ОЦЕНОК");
                Console.WriteLine("1. Добавить оценку");
                Console.WriteLine("2. Показать средний балл студента");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите опцию: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddGrade();
                        break;
                    case "2":
                        ShowStudentAverageGrade();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        static void AddStudent()
        {
            Console.Write("Введите имя студента: ");
            var name = Console.ReadLine() ?? "";
            Console.Write("Введите возраст: ");
            if (int.TryParse(Console.ReadLine(), out int age))
            {
                Console.Write("Введите контактную информацию: ");
                var contactInfo = Console.ReadLine() ?? "";
                universityService.AddStudent(name, age, contactInfo);
            }
            else
            {
                Console.WriteLine("Неверный формат возраста!");
            }
        }

        static void AddTeacher()
        {
            Console.Write("Введите имя преподавателя: ");
            var name = Console.ReadLine() ?? "";
            Console.Write("Введите возраст: ");
            if (int.TryParse(Console.ReadLine(), out int age))
            {
                Console.Write("Введите контактную информацию: ");
                var contactInfo = Console.ReadLine() ?? "";
                Console.Write("Введите кафедру: ");
                var department = Console.ReadLine() ?? "";
                universityService.AddTeacher(name, age, contactInfo, department);
            }
            else
            {
                Console.WriteLine("Неверный формат возраста!");
            }
        }

        static void AddCourse()
        {
            Console.Write("Введите название курса: ");
            var name = Console.ReadLine() ?? "";
            Console.Write("Введите описание: ");
            var description = Console.ReadLine() ?? "";
            Console.Write("Введите максимальное количество студентов (по умолчанию 30): ");
            var maxStudentsInput = Console.ReadLine();
            int maxStudents = 30;
            if (!string.IsNullOrEmpty(maxStudentsInput) && int.TryParse(maxStudentsInput, out int parsed))
            {
                maxStudents = parsed;
            }
            universityService.AddCourse(name, description, maxStudents);
        }

        static void ShowStudentDetails()
        {
            universityService.ShowAllStudents();
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int studentId))
            {
                universityService.ShowStudentDetails(studentId);
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void ShowTeacherDetails()
        {
            universityService.ShowAllTeachers();
            Console.Write("Введите ID преподавателя: ");
            if (int.TryParse(Console.ReadLine(), out int teacherId))
            {
                universityService.ShowTeacherDetails(teacherId);
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void ShowCourseDetails()
        {
            universityService.ShowAllCourses();
            Console.Write("Введите ID курса: ");
            if (int.TryParse(Console.ReadLine(), out int courseId))
            {
                universityService.ShowCourseDetails(courseId);
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void EnrollStudentInCourse()
        {
            universityService.ShowAllStudents();
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int studentId))
            {
                universityService.ShowAllCourses();
                Console.Write("Введите ID курса: ");
                if (int.TryParse(Console.ReadLine(), out int courseId))
                {
                    universityService.EnrollStudentInCourse(studentId, courseId);
                }
                else
                {
                    Console.WriteLine("Неверный формат ID курса!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID студента!");
            }
        }

        static void AssignTeacherToCourse()
        {
            universityService.ShowAllTeachers();
            Console.Write("Введите ID преподавателя: ");
            if (int.TryParse(Console.ReadLine(), out int teacherId))
            {
                universityService.ShowAllCourses();
                Console.Write("Введите ID курса: ");
                if (int.TryParse(Console.ReadLine(), out int courseId))
                {
                    universityService.AssignTeacherToCourse(teacherId, courseId);
                }
                else
                {
                    Console.WriteLine("Неверный формат ID курса!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID преподавателя!");
            }
        }

        static void AddGrade()
        {
            universityService.ShowAllStudents();
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int studentId))
            {
                universityService.ShowAllCourses();
                Console.Write("Введите ID курса: ");
                if (int.TryParse(Console.ReadLine(), out int courseId))
                {
                    Console.Write("Введите оценку (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int grade) && grade >= 1 && grade <= 5)
                    {
                        universityService.AddGrade(studentId, courseId, grade);
                    }
                    else
                    {
                        Console.WriteLine("Неверный формат оценки! Оценка должна быть от 1 до 5.");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный формат ID курса!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID студента!");
            }
        }

        static void ShowStudentAverageGrade()
        {
            universityService.ShowAllStudents();
            Console.Write("Введите ID студента: ");
            if (int.TryParse(Console.ReadLine(), out int studentId))
            {
                universityService.ShowStudentDetails(studentId);
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void ShowAllData()
        {
            universityService.ShowAllStudents();
            universityService.ShowAllTeachers();
            universityService.ShowAllCourses();
        }
    }
}