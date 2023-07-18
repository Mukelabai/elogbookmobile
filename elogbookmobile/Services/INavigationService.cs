using elogbookmobile.ViewModels;
using System;
using System.Threading.Tasks;

namespace elogbookmobile.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(Type viewModelType);

        Task NavigateToAsync(Type viewModelType, object parameter);

        Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;

        Task GoBackAsync();
    }
}
