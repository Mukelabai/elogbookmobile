using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIAssignment
    {
        [PrimaryKey]
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public int AcademicYear { get; set; }
        public DateTime DueDate { get; set; }
        public string Rotation { get; set; }
        public DateTime RotationStart { get; set; }
        public DateTime RotationEnd { get; set; }
        public int ElogbookId { get; set; }
        public string ElogbookName { get; set; }
    }
}