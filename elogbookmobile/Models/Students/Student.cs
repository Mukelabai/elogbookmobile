using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.Students
{
    public class Student
    {
        private string webUsername, lastName, firstName, computerNumber, sex;

        public string WebUsername { get => webUsername; set => webUsername = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string ComputerNumber { get => computerNumber; set => computerNumber = value; }
        public string Sex { get => sex; set => sex = value; }
    }
}