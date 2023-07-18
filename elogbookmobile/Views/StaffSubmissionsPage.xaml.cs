using elogbookmobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace elogbookmobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StaffSubmissionsPage : ContentPage
    {
        public StaffSubmissionsPage()
        {
            InitializeComponent();
            BindingContext = new StaffSubmissionsViewModel();
        }
    }
}