using System;
using System.Collections.Generic;
using System.Text;

namespace elogbookmobile.Models.API
{
   public class APISubmissionStatus
    {
        public string StatusName { get; set; }
        public override string ToString()
        {
            return StatusName;
        }
    }
}
