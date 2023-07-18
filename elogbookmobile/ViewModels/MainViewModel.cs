using elogbookapi.Models;
using elogbookmobile.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        CustomDrawerMenuItem selectedMenuItem;

        public MainViewModel()
        {
            IsBusy = true;
            User u = App.ApplicationUser;
            MenuItems = new ObservableCollection<CustomDrawerMenuItem>();
            //get application user
            //SetUser();
            //common menus
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "About", ViewModelType = typeof(AboutViewModel), ImageName = "ic_info" });
            if (u != null && u.RoleName.ToLower().Contains("student"))
            {
                MenuItems.Add(new CustomDrawerMenuItem() { Name = "Submissions", ViewModelType = typeof(StudentSubmissionsViewModel), ImageName = "ic_popup" });
            }
            else
            {
                //staff
                MenuItems.Add(new CustomDrawerMenuItem() { Name = "Assignments", ViewModelType = typeof(AssignmentsViewModel), ImageName = "ic_popup" });
            }
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "Elogbook Web Platform", ViewModelType = typeof(WebPlatformViewModel), ImageName = "ic_browse" });





            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Browse", ViewModelType = typeof(ItemsViewModel), ImageName = "ic_browse" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Scheduler", ViewModelType = typeof(SchedulerViewModel), ImageName = "ic_scheduler" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "DataGrid", ViewModelType = typeof(DataGridViewModel), ImageName = "ic_datagrid" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Charts", ViewModelType = typeof(ChartsViewModel), ImageName = "ic_charts" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Popup", ViewModelType = typeof(PopupViewModel), ImageName = "ic_popup" });
            MenuItems.Add(new CustomDrawerMenuItem() { Name = "Logout", ViewModelType = typeof(LoginViewModel), ImageName = "ic_logout" });
            SelectedMenuItem = MenuItems[0];
            IsBusy = false;
        }

        public ObservableCollection<CustomDrawerMenuItem> MenuItems { get; }

        public CustomDrawerMenuItem SelectedMenuItem
        {
            get => this.selectedMenuItem;
            set
            {
                if (SelectedMenuItem == value || value == null)
                    return;
                SetProperty(ref this.selectedMenuItem, value);
                if (SelectedMenuItem != null && !IsBusy)
                    OnMenuItemSelected(SelectedMenuItem.ViewModelType);
            }
        }


        async void OnMenuItemSelected(Type pageType)
        {
            await Navigation.NavigateToAsync(SelectedMenuItem.ViewModelType);
        }
    }

    public class CustomDrawerMenuItem
    {
        public string Name { get; set; }
        public Type ViewModelType { get; set; }
        public string ImageName { get; set; }
        public bool NavigateToRoot { get; set; }
    }
}
