using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models
{
    public class User
    {

        [PrimaryKey]
        public string WebUsername { get; set ; }
        public string LastName { get; set; }
        public string FirstName { get ; set ; }
        public string UserNumber { get ; set; }
        public string Sex { get ; set ; }
        public string RoleName { get ; set ; }
        public int UserId { get; set; }

        public bool IsStudent { get
            {
                return !string.IsNullOrWhiteSpace(RoleName) && RoleName.ToLower().Contains("student");
            } }
    }
}