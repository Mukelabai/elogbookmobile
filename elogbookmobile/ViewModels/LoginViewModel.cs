using elogbookapi.Models;
using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        string userName;
        string password;

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked, ValidateLogin);
            PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }


        public string UserName
        {
            get => this.userName;
            set => SetProperty(ref this.userName, value);
        }

        public string Password
        {
            get => this.password;
            set => SetProperty(ref this.password, value);
        }

        public Command LoginCommand { get; }


        async void OnLoginClicked()
        {
            IsBusy = true;
            try
            {
                //use api to get user
                var user = await APIUserService.IsValidUser(UserName, Password);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Login failed", "Invalid credentials", "OK");
                }
                else
                {
                    //pul relevant data

                    await Application.Current.MainPage.DisplayAlert("Login successful", string.Format("Welcome {0} {1}, you're logged in as{2} {3}", user.FirstName, user.LastName, user.RoleName.ToLower().Contains("student") ? " a" : "", user.RoleName), "OK");
                    //save user to database
                    await DBService.DeleteUser();
                    await DBService.AddUser(user);
                    App.ApplicationUser = user;

                    //Get user data
                    bool isStudent = user.RoleName.ToLower().Contains("student");
                    var userData = await APIUserService.GetUserData(UserName, isStudent);

                    //clean DB before saving data
                    await DBService.CleanDatabase();
                    //save assignments
                    foreach (APIAssignment a in userData.Assignments)
                    {
                        await DBService.AddAssignment(a);
                    }
                    //save hospitals
                    foreach (APIHospital a in userData.Hospitals)
                    {
                        await DBService.AddHospital(a);
                    }
                    //save mentors
                    foreach (APIMentor a in userData.Mentors)
                    {
                        await DBService.AddMentor(a);
                    }
                    if (isStudent)
                    {
                        //save submissions
                        foreach (APISubmission s in userData.Submissions)
                        {
                            await DBService.AddSubmission(s);
                        }
                        //save questions
                        if (userData.Questions != null)
                        {
                            await DBService.AddQuestion(userData.Questions);
                        }

                        if (userData.Competences != null)
                        {
                            await DBService.AddComptencies(userData.Competences);
                        }

                        //download submission data for each submission
                        if (userData.Submissions != null)
                        {
                            foreach (APISubmission s in userData.Submissions)
                            {
                                await Utility.LoadSubmissionDataForStudent(s.SubmissionIdT);
                            }
                        }

                    }
                    else
                    {
                        //save submissions for current year
                        int maxYear = (from y in userData.Assignments select y.AcademicYear).Max();
                        List<APIAssignment> currentAssignments = userData.Assignments.Where(a => a.AcademicYear == maxYear).ToList();
                        foreach (APIAssignment a in currentAssignments)
                        {
                            await Utility.DownloadSubmissionsForStaff(a.AssignmentId);
                        }
                        //save grades
                        if (userData.Grades != null)
                        {
                            foreach (APIGrade g in userData.Grades)
                            {
                                await DBService.AddGrade(g);
                            }
                        }

                    }

                    //get saved user

                    /* var asses = await DBService.GetAssignments();
                     var submissions = await DBService.GetSubmissions();

                     if (isStudent)
                     {
                         await Application.Current.MainPage.DisplayAlert("Data we found relevant to you", string.Format("Assignments: {0}\nSubmissions: {1}", asses.Count, submissions.Count), "OK");
                     }
                     else
                     {
                         await Application.Current.MainPage.DisplayAlert("Data we found relevant to you", string.Format("Assignments: {0}\nSubmissions: {1}", asses.Count, submissions.Count), "OK");
                     }*/

                    setMenuItems(user);
                    await Navigation.NavigateToAsync<MainViewModel>();

                }
            }catch(Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
            finally
            {
                IsBusy = false;
            }
            
        }
        
        void setMenuItems(User u)
        {
            App.MenuViewModel.MenuItems.Clear();
            App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "About", ViewModelType = typeof(AboutViewModel), ImageName = "ic_info" });
            if (u != null && u.RoleName.ToLower().Contains("student"))
            {
                App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Submissions", ViewModelType = typeof(StudentSubmissionsViewModel), ImageName = "ic_popup" });
            }
            else
            {
                //staff
                App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Assignments", ViewModelType = typeof(AssignmentsViewModel), ImageName = "ic_popup" });
            }
            App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Elogbook Web Platform", ViewModelType = typeof(WebPlatformViewModel), ImageName = "ic_browse" });

            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Browse", ViewModelType = typeof(ItemsViewModel), ImageName = "ic_browse" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Scheduler", ViewModelType = typeof(SchedulerViewModel), ImageName = "ic_scheduler" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "DataGrid", ViewModelType = typeof(DataGridViewModel), ImageName = "ic_datagrid" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Charts", ViewModelType = typeof(ChartsViewModel), ImageName = "ic_charts" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Popup", ViewModelType = typeof(PopupViewModel), ImageName = "ic_popup" });
            App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Logout", ViewModelType = typeof(LoginViewModel), ImageName = "ic_logout" });
            App.MenuViewModel.SelectedMenuItem = App.MenuViewModel.MenuItems[0];
        }

        bool ValidateLogin()
        {
            return !String.IsNullOrWhiteSpace(UserName)
                && !String.IsNullOrWhiteSpace(Password);
        }
    }
}
