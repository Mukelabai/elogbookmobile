using DevExpress.XamarinForms.Navigation;
using elogbookmobile.ViewModels;
using elogbookmobile.Views;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.Services
{
    public class NavigationService : INavigationService
    {

        public async Task NavigateToAsync(Type viewModelType)
        {
            await InternalNavigateToAsync(viewModelType, null, false);
        }
        public async Task NavigateToAsync(Type viewModelType, object parameter)
        {
            await InternalNavigateToAsync(viewModelType, parameter, true);
        }
        public async Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
        {
            await InternalNavigateToAsync(typeof(TViewModel), null, false);
        }

        public async Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel
        {
            await InternalNavigateToAsync(typeof(TViewModel), parameter, true);
        }
        public async Task GoBackAsync()
        {
            var navigation = GetActiveNavigation();
            if (navigation.ModalStack.Count != 0)
                await navigation.PopModalAsync();
            else
                await navigation.PopAsync();
        }

        async Task InternalNavigateToAsync(Type viewModelType, object parameter, bool isDetail)
        {
            Page page = CreatePage(viewModelType, parameter);

            if (viewModelType == typeof(LoginViewModel) || viewModelType == typeof(MainViewModel))
                Application.Current.MainPage = page;
            else if (!isDetail && Application.Current.MainPage is DrawerPage drawerPage)
                drawerPage.MainContent = new NavigationPage(page);
            else
            {
                var navigation = GetActiveNavigation();
                if (navigation == null)
                    Application.Current.MainPage = page;
                else
                    await navigation.PushAsync(page);
            }
            await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
        }

        Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("ViewModels", "Views").Replace("ViewModel", "Page");
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(
                        CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }

        INavigation GetActiveNavigation()
        {
            var navigationPage = Application.Current.MainPage as DrawerPage;
            var navigator = navigationPage?.MainContent as NavigationPage;
            return navigator?.Navigation;
        }
    }
}
