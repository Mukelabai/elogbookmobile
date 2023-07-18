using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APISubmissionComment
    {
        public long CommentId { get; set; }
        public string CommentText { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long SubmissionId { get; set; }
        public Guid SubmissionIdT { get; set; }
        public int InstitutionId { get; set; }
        public string CreatedByFullName { get; set; }
        public string UserType { get; set; }
    }
}