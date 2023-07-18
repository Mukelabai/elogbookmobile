using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace elogbookmobile.ViewModels
{
   public class WebPlatformViewModel:BaseViewModel
    {
        public const string ViewName = "WebPlatformPage";
        public   WebPlatformViewModel()
        {
            Title = "Elogbook Web Platform";
            //LoadWeb();
        }

        public async void LoadWeb()
        {
            await Browser.OpenAsync("https://elogbook.coinfomas.net/");
            await Navigation.NavigateToAsync<AboutViewModel>();
        }
    }
}
