using elogbookapi.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public const string ViewName = "AboutPage";
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(OnLoad);
        }

        public ICommand OpenWebCommand { get; }

        public async void OnLoad()
        {
            User u = App.ApplicationUser;
            if (u != null && !u.IsStudent)
            {
                await Navigation.NavigateToAsync<AssignmentsViewModel>();
            }
            else
            {
                await Navigation.NavigateToAsync<StudentSubmissionsViewModel>();
            }
        }
    }
}