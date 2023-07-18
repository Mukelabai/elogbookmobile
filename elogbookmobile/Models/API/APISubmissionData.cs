using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APISubmissionData
    {
        public Guid SubmissionIdT { get; set; }
        public APISubmission Submission { get; set; }
        public List<APICase> Cases { get; set; }
        public List<APIQuestion> Questions { get; set; }
        public List<APIQuestionResponse> Responses { get; set; }
        public List<APIAssignmentQuestionResponse> AssignmentResponses { get; set; }
        public List<APISubmissionComment> Comments { get; set; }
        public List<ElogbookAchievement> ElogbookAchievements { get; set; }
    }
}