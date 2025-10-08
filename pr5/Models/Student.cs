using System;
using System.Collections.Generic;
using System.Linq;

namespace pr5.Models
{
    public class Student : Person
    {
        private int studentId;
        private List<Course> courses;
        private Dictionary<Course, List<int>> grades;

        public Student(string name, int age, string contactInfo, int studentId) 
            : base(name, age, contactInfo)
        {
            this.studentId = studentId;
            this.courses = new List<Course>();
            this.grades = new Dictionary<Course, List<int>>();
        }

        public int StudentId
        {
            get { return studentId; }
            set { studentId = value; }
        }

        public List<Course> Courses
        {
            get { return courses; }
        }

        public void EnrollInCourse(Course course)
        {
            if (!courses.Contains(course))
            {
                courses.Add(course);
                grades[course] = new List<int>();
                course.AddStudent(this);
            }
        }

        public void AddGrade(Course course, int grade)
        {
            if (courses.Contains(course) && grades.ContainsKey(course))
            {
                grades[course].Add(grade);
            }
        }

        public double GetAverageGrade()
        {
            var allGrades = grades.Values.SelectMany(g => g).ToList();
            return allGrades.Count > 0 ? allGrades.Average() : 0.0;
        }

        public double GetAverageGradeForCourse(Course course)
        {
            if (grades.ContainsKey(course) && grades[course].Count > 0)
            {
                return grades[course].Average();
            }
            return 0.0;
        }

        public override string GetInfo()
        {
            return $"Студент: {name}, ID: {studentId}, Возраст: {age}, Контакты: {contactInfo}";
        }

        public string GetDetailedInfo()
        {
            var info = GetInfo();
            info += $"\nКурсы ({courses.Count}):";
            foreach (var course in courses)
            {
                var avgGrade = GetAverageGradeForCourse(course);
                info += $"\n  - {course.Name} (средний балл: {avgGrade:F2})";
            }
            info += $"\nОбщий средний балл: {GetAverageGrade():F2}";
            return info;
        }
    }
}
