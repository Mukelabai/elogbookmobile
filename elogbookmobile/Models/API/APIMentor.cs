using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIMentor
    {
        [PrimaryKey]
        public int MentorId { get; set; }
        public string MentorName { get; set; }
    }
}