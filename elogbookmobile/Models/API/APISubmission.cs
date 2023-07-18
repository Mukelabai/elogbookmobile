using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APISubmission
    {
        
        public long SubmissionId { get; set; }

        [PrimaryKey]
        public Guid SubmissionIdT { get; set; }
        public int StudentId { get; set; }
        public int MentorId { get; set; }
        public int HospitalId { get; set; }
        public int AssignmentId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Rotation { get; set; }
        public string Hospital { get; set; }
        public string Mentor { get; set; }
        public string SubmissionName { get; set; }
        public int Cases { get; set; }
        public string Status { get; set; }
        public string Grade { get; set; }
        public bool IsPublished { get; set; }
    }
}