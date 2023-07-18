using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIElogbookCompetence
    {
        public long CompetenceId { get; set; }
        public int ElogbookId { get; set; }

        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionOption { get; set; }
        public int Expected { get; set; }
        public int AssignmentId { get; set; }
    }
}