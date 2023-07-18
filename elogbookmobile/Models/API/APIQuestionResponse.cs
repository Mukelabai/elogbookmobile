using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIQuestionResponse
    {
        public Guid CaseUID { get; set; }
        public int QuestionId { get; set; }
        public string ResponseText { get; set; }
        public Guid SubmissionIdT { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public string UpdatedBy { get; set; }
        //public DateTime UpdatedOn { get; set; }
    }
}