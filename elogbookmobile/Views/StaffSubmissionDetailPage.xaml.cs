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
    public partial class StaffSubmissionDetailPage : ContentPage
    {
        public StaffSubmissionDetailPage()
        {
            InitializeComponent();
            BindingContext = ViewModel= new StaffSubmissionDetailViewModel();
        }

        public StaffSubmissionDetailViewModel ViewModel { get; set; }

        private async void btnAddComment_Clicked(object sender, EventArgs e)
        {
            try
            {
                APIStaffSubmission submission = ViewModel.SelectedSubmission;
                APISubmissionComment c = new APISubmissionComment
                {
                    CommentText = txtComment.Text.Trim(),
                    CreatedBy = App.ApplicationUser.WebUsername,
                    CreatedOn = DateTime.Now,
                    SubmissionId = submission.SubmissionId,
                    InstitutionId = submission.InstitutionId,
                    UserType = App.ApplicationUser.RoleName,
                    CreatedByFullName = string.Format("{0} {1} ({2})", App.ApplicationUser.FirstName, App.ApplicationUser.LastName, App.ApplicationUser.RoleName)

                };
                await DBService.AddComment(c);
                ViewModel.LoadComments(submission);
                txtComment.Text = null;
            }catch(Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }

        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            btnAddComment.IsEnabled = !string.IsNullOrWhiteSpace(txtComment.Text);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //set status
            ViewModel.OnAppearing();
            //set achievement progress

        }

        private async void btnUploadComments_Clicked(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }
    }
}