using System;
using System.Collections.Generic;
using System.Linq;
using pr5.Models;

namespace pr5.Services
{
    public class UniversityService
    {
        private List<Student> students;
        private List<Teacher> teachers;
        private List<Course> courses;
        private int nextStudentId;
        private int nextTeacherId;
        private int nextCourseId;

        public UniversityService()
        {
            students = new List<Student>();
            teachers = new List<Teacher>();
            courses = new List<Course>();
            nextStudentId = 1;
            nextTeacherId = 1;
            nextCourseId = 1;
        }

        public void AddStudent(string name, int age, string contactInfo)
        {
            var student = new Student(name, age, contactInfo, nextStudentId++);
            students.Add(student);
            Console.WriteLine($"Студент {name} добавлен с ID: {student.StudentId}");
        }

        public void AddTeacher(string name, int age, string contactInfo, string department)
        {
            var teacher = new Teacher(name, age, contactInfo, nextTeacherId++, department);
            teachers.Add(teacher);
            Console.WriteLine($"Преподаватель {name} добавлен с ID: {teacher.TeacherId}");
        }

        public void AddCourse(string name, string description, int maxStudents = 30)
        {
            var course = new Course(nextCourseId++, name, description, maxStudents);
            courses.Add(course);
            Console.WriteLine($"Курс {name} создан с ID: {course.CourseId}");
        }

        public void EnrollStudentInCourse(int studentId, int courseId)
        {
            var student = students.FirstOrDefault(s => s.StudentId == studentId);
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);

            if (student == null)
            {
                Console.WriteLine("Студент не найден!");
                return;
            }

            if (course == null)
            {
                Console.WriteLine("Курс не найден!");
                return;
            }

            if (!course.CanAddStudent())
            {
                Console.WriteLine("Курс переполнен! Максимальное количество студентов: " + course.MaxStudents);
                return;
            }

            student.EnrollInCourse(course);
            Console.WriteLine($"Студент {student.Name} записан на курс {course.Name}");
        }

        public void AssignTeacherToCourse(int teacherId, int courseId)
        {
            var teacher = teachers.FirstOrDefault(t => t.TeacherId == teacherId);
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);

            if (teacher == null)
            {
                Console.WriteLine("Преподаватель не найден!");
                return;
            }

            if (course == null)
            {
                Console.WriteLine("Курс не найден!");
                return;
            }

            teacher.AssignToCourse(course);
            Console.WriteLine($"Преподаватель {teacher.Name} назначен на курс {course.Name}");
        }

        public void AddGrade(int studentId, int courseId, int grade)
        {
            var student = students.FirstOrDefault(s => s.StudentId == studentId);
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);

            if (student == null)
            {
                Console.WriteLine("Студент не найден!");
                return;
            }

            if (course == null)
            {
                Console.WriteLine("Курс не найден!");
                return;
            }

            if (!student.Courses.Contains(course))
            {
                Console.WriteLine("Студент не записан на этот курс!");
                return;
            }

            student.AddGrade(course, grade);
            Console.WriteLine($"Оценка {grade} добавлена студенту {student.Name} по курсу {course.Name}");
        }

        public void ShowAllStudents()
        {
            Console.WriteLine("\nВСЕ СТУДЕНТЫ");
            if (students.Count == 0)
            {
                Console.WriteLine("Студенты не найдены.");
                return;
            }

            foreach (var student in students)
            {
                Console.WriteLine(student.GetInfo());
            }
        }

        public void ShowAllTeachers()
        {
            Console.WriteLine("\n ВСЕ ПРЕПОДАВАТЕЛИ");
            if (teachers.Count == 0)
            {
                Console.WriteLine("Преподаватели не найдены.");
                return;
            }

            foreach (var teacher in teachers)
            {
                Console.WriteLine(teacher.GetInfo());
            }
        }

        public void ShowAllCourses()
        {
            Console.WriteLine("\nВСЕ КУРСЫ");
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсы не найдены.");
                return;
            }

            foreach (var course in courses)
            {
                Console.WriteLine(course.GetInfo());
            }
        }

        public void ShowStudentDetails(int studentId)
        {
            var student = students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                Console.WriteLine("Студент не найден!");
                return;
            }

            Console.WriteLine("\nДЕТАЛЬНАЯ ИНФОРМАЦИЯ О СТУДЕНТЕ");
            Console.WriteLine(student.GetDetailedInfo());
        }

        public void ShowTeacherDetails(int teacherId)
        {
            var teacher = teachers.FirstOrDefault(t => t.TeacherId == teacherId);
            if (teacher == null)
            {
                Console.WriteLine("Преподаватель не найден!");
                return;
            }

            Console.WriteLine("\nДЕТАЛЬНАЯ ИНФОРМАЦИЯ О ПРЕПОДАВАТЕЛЕ");
            Console.WriteLine(teacher.GetDetailedInfo());
        }

        public void ShowCourseDetails(int courseId)
        {
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                Console.WriteLine("Курс не найден!");
                return;
            }

            Console.WriteLine("\nДЕТАЛЬНАЯ ИНФОРМАЦИЯ О КУРСЕ");
            Console.WriteLine(course.GetDetailedInfo());
        }

        public List<Student> GetStudents()
        {
            return students;
        }

        public List<Teacher> GetTeachers()
        {
            return teachers;
        }

        public List<Course> GetCourses()
        {
            return courses;
        }
    }
}
