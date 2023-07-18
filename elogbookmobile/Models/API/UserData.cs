using elogbookapi.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class UserData
    {
        public string WebUsername { get; set; }
        public List<APIAssignment> Assignments { get; set; }
        public List<APIMentor> Mentors { get; set; }
        public List<APIHospital> Hospitals { get; set; }
        public List<APISubmission> Submissions { get; set; }
        public List<APICase> Cases { get; set; }
        public List<APIGrade> Grades { get; set; }
        public List<APIQuestion> Questions { get; set; }
        public List<APIElogbookCompetence> Competences { get; set; }
    }
}