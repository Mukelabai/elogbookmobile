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
    public partial class StudentSubmissionsPage : ContentPage
    {
        public StudentSubmissionsPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new StudentSubmissionsViewModel();
        }

        StudentSubmissionsViewModel ViewModel { get; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.OnAppearing();
        }
    }
}