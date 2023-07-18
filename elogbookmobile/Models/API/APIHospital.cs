using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace elogbookapi.Models.API
{
    public class APIHospital
    {
        [PrimaryKey]
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
    }
}