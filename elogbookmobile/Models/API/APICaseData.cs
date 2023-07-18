using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APICaseData
    {
        public List<APICase> Cases { get; set; }
        public List<APIResponses> Responses { get; set; }
        public List<APIAssignmentQuestionResponse> AssignmentResponses { get; set; }
        public List<APISubmissionComment> Comments { get; set; }
        public List<ElogbookAchievement> Achievements { get; set; }
    }
}