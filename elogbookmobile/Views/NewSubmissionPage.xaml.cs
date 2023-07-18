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
    public partial class NewSubmissionPage : ContentPage
    {
        public NewSubmissionPage()
        {
            InitializeComponent();
            BindingContext = ViewModel= new NewSubmissionViewModel();

        }

        public NewSubmissionViewModel ViewModel { get; }
        protected override void OnAppearing()
        {
            base.OnAppearing();
           
        }
    }
}