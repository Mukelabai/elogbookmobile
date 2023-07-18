using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIResponses
    {

        public int QuestionId { get; set; }
        public string ResponseText { get; set; }

        public long CaseId { get; set; }
        public long SubmissionId { get; set; }


    }
}