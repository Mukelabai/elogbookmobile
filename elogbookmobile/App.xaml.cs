using elogbookapi.Models;
using elogbookmobile.Services;
using elogbookmobile.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace elogbookmobile
{
    public partial class App : Application
    {
        public App()
        {
            DevExpress.XamarinForms.Charts.Initializer.Init();
            DevExpress.XamarinForms.CollectionView.Initializer.Init();
            DevExpress.XamarinForms.Scheduler.Initializer.Init();
            DevExpress.XamarinForms.DataGrid.Initializer.Init();
            DevExpress.XamarinForms.Editors.Initializer.Init();
            DevExpress.XamarinForms.Navigation.Initializer.Init();
            DevExpress.XamarinForms.DataForm.Initializer.Init();
            DevExpress.XamarinForms.Popup.Initializer.Init();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<NavigationService>();
            

            InitializeComponent();

            var navigationService = DependencyService.Get<INavigationService>();
            // Use the NavigateToAsync<ViewModelName> method
            // to display the corresponding view.
            // Code lines below show how to navigate to a specific page.
            // Comment out all but one of these lines
            // to open the corresponding page.
            //navigationService.NavigateToAsync<LoginViewModel>();
            //check if user exists
            var result = Task.Run(async () => await DBService.GetApplicationUser()).Result;
           
            
            ApplicationUser = result;
            if (ApplicationUser == null)
            {
                navigationService.NavigateToAsync<LoginViewModel>();
            }
            else
            {
                MenuViewModel = new MainViewModel();
                navigationService.NavigateToAsync<MainViewModel>();
            }
        }
        public static User ApplicationUser { get; set; }
        public static MainViewModel MenuViewModel { get; set; }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
