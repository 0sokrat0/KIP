using System;
using System.Collections.Generic;

namespace pr5.Models
{
    public class Teacher : Person
    {
        private int teacherId;
        private string department;
        private List<Course> courses;

        public Teacher(string name, int age, string contactInfo, int teacherId, string department) 
            : base(name, age, contactInfo)
        {
            this.teacherId = teacherId;
            this.department = department;
            this.courses = new List<Course>();
        }

        public int TeacherId
        {
            get { return teacherId; }
            set { teacherId = value; }
        }

        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        public List<Course> Courses
        {
            get { return courses; }
        }

        public void AssignToCourse(Course course)
        {
            if (!courses.Contains(course))
            {
                courses.Add(course);
                course.AssignTeacher(this);
            }
        }

        public override string GetInfo()
        {
            return $"Преподаватель: {name}, ID: {teacherId}, Возраст: {age}, Кафедра: {department}, Контакты: {contactInfo}";
        }

        public string GetDetailedInfo()
        {
            var info = GetInfo();
            info += $"\nКурсы ({courses.Count}):";
            foreach (var course in courses)
            {
                info += $"\n  - {course.Name} (студентов: {course.Students.Count})";
            }
            return info;
        }
    }
}
