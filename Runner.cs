using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Solution
{
    internal class Runner
    {
        private readonly string forename;
        private readonly string surname;
        private readonly int age;
        private readonly string gender;

        public Runner(string forename, string surname, int age, string gender)
        {
            this.forename = forename;
            this.surname = surname;
            this.age = age;
            this.gender = gender;
        }

        public string GetForename()
        {
            return forename;
        }

        public string GetSurname()
        {
            return surname;
        }

        public int GetAge()
        {
            return age;
        }

        public string GetGender()
        {
            return gender;
        }
    }
}
