using System;
using System.Collections.Generic;

namespace pr5.Models
{
    public class Course
    {
        private int courseId;
        private string name;
        private string description;
        private int maxStudents;
        private Teacher? teacher;
        private List<Student> students;

        public Course(int courseId, string name, string description, int maxStudents = 30)
        {
            this.courseId = courseId;
            this.name = name;
            this.description = description;
            this.maxStudents = maxStudents;
            this.students = new List<Student>();
        }

        public int CourseId
        {
            get { return courseId; }
            set { courseId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int MaxStudents
        {
            get { return maxStudents; }
            set { maxStudents = value; }
        }

        public Teacher? Teacher
        {
            get { return teacher; }
        }

        public List<Student> Students
        {
            get { return students; }
        }

        public bool CanAddStudent()
        {
            return students.Count < maxStudents;
        }

        public void AddStudent(Student student)
        {
            if (CanAddStudent() && !students.Contains(student))
            {
                students.Add(student);
            }
        }

        public void AssignTeacher(Teacher teacher)
        {
            this.teacher = teacher;
        }

        public string GetInfo()
        {
            return $"Курс: {name}, ID: {courseId}, Описание: {description}, Студентов: {students.Count}/{maxStudents}";
        }

        public string GetDetailedInfo()
        {
            var info = GetInfo();
            if (teacher != null)
            {
                info += $"\nПреподаватель: {teacher.Name} ({teacher.Department})";
            }
            else
            {
                info += "\nПреподаватель не назначен";
            }
            
            if (students.Count > 0)
            {
                info += "\nСтуденты:";
                foreach (var student in students)
                {
                    info += $"\n  - {student.Name} (ID: {student.StudentId})";
                }
            }
            else
            {
                info += "\nСтуденты не записаны";
            }
            
            return info;
        }
    }
}
