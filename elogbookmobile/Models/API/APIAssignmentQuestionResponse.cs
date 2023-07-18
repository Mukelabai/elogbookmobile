using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    /// <summary>
    /// This class holds responses to assignment-level questions for a student. For instance, questions about topics covered at tutorials during a rotation
    /// </summary>
    public class APIAssignmentQuestionResponse
    {
        public int QuestionId { get; set; }
        public string ResponseText { get; set; }
        public Guid SubmissionIdT { get; set; }
        public long SubmissionId { get; set; }
    }
}