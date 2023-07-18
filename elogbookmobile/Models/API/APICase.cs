using SQLite;
using System;

namespace elogbookapi.Models.API
{
    public class APICase
    {
        [PrimaryKey]
        
        public Guid CaseUID { get; set; }
        public long CaseId { get; set; }
        public string Patient { get; set; }
        public long SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public Guid SubmissionIdT { get; set; }
        public int StudentId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}