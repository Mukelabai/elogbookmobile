using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    /// <summary>
    /// Contains list of staff submissions and list of comments
    /// </summary>
    public class APISubmissionCommentData
    {
        public string WebUsername { get; set; }
        public List<APIStaffSubmission> Submissions { get; set; }
        public List<APISubmissionComment> Comments { get; set; }
        public List<APIResponses> Responses { get; set; }
    }
}