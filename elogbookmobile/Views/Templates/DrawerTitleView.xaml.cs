using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace elogbookmobile.Views.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawerTitleView : ContentView
    {
        public DrawerTitleView()
        {
            InitializeComponent();
        }

        void OnMenuClicked(object sender, EventArgs args)
        {
            MessagingCenter.Instance.Send<View>(this, "OPEN_DRAWER");
        }
    }
}