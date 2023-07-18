using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Services;
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
    public partial class SubmissionDetailPage : ContentPage
    {
        public SubmissionDetailPage()
        {
            InitializeComponent();
            BindingContext = ViewModel= new SubmissionDetailViewModel();
        }

        public SubmissionDetailViewModel ViewModel { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            pbAchievement.ProgressColor = pbAchievement.Progress >= 1 ? Color.Green : pbAchievement.ProgressColor;
        }

        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            btnAddComment.IsEnabled = !string.IsNullOrWhiteSpace(txtComment.Text);
        }
        private async void btnAddComment_Clicked(object sender, EventArgs e)
        {
            try
            {
                APISubmission submission = ViewModel.SelectedSubmission;
                APISubmissionComment c = new APISubmissionComment
                {
                    CommentText = txtComment.Text.Trim(),
                    CreatedBy = App.ApplicationUser.WebUsername,
                    CreatedOn = DateTime.Now,
                    SubmissionIdT = submission.SubmissionIdT,
                    SubmissionId = -1,
                    InstitutionId = -1,
                    UserType = App.ApplicationUser.RoleName,
                    CreatedByFullName = string.Format("{0} {1} ({2})", App.ApplicationUser.FirstName, App.ApplicationUser.LastName, App.ApplicationUser.RoleName)

                };
                await DBService.AddComment(c);
                ViewModel.LoadComments(submission);
                txtComment.Text = null;
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }
    }
}