using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIAssignmentData
    {
        public List<APIStaffSubmission> Submissions { get; set; }
        public List<APIQuestion> Questions { get; set; }
        public List<APIElogbookCompetence> Competences { get; set; }
    }
}