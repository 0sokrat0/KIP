using System;

namespace pr5.Models
{
    public abstract class Person
    {
        protected string name;
        protected int age;
        protected string contactInfo;

        public Person(string name, int age, string contactInfo)
        {
            this.name = name;
            this.age = age;
            this.contactInfo = contactInfo;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public string ContactInfo
        {
            get { return contactInfo; }
            set { contactInfo = value; }
        }

        public abstract string GetInfo();
    }
}
