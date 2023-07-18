using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIStaffSubmission
    {
        public int AssignmentId { get; set; }
        public string Student { get; set; }
        public long SubmissionId { get; set; }
        public Guid SubmissionIdT { get; set; }
        public string Mentor { get; set; }
        public string Hospital { get; set; }
        public int Cases { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int InstitutionId { get; set; }
        public string Status { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public string GradedBy { get; set; }
        public string GradedByName { get; set; }
        public DateTime GradedOn { get; set; }
        public string StatusColour
        {
            get
            {
                if (Status == "Pending")
                {
                    return "Red";
                }
                else if (Status == "Approved")
                {
                    return "Green";
                }
                else
                {
                    return "Orange";
                }
            }
        }
        public double Achievement { get; set; }
        public string AchievementColour
        {
            get
            {
                if (Achievement >= 100)
                {
                    return "Green";
                }
                else if (Achievement >= 50)
                {
                    return "Orange";
                }
                else
                {
                    return "Red";
                }
            }
        }
        public string AchievementLabel
        {
            get;set;
        }
    }
}