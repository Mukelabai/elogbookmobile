using elogbookapi.Models.API;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace elogbookmobile.Models.API
{
   public class QuestionFormElement
    {
        public View SectionControl { get; set; }
        public View QuestionControl { get; set; }
        public Label QuestionLabel { get; set; }
        public APIQuestion Question { get; set; }
    }
}
