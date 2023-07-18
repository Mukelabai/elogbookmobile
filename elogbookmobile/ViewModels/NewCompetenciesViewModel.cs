using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
   public class NewCompetenciesViewModel : BaseViewModel
    {
        public const string ViewName = "NewCompetenciesPage";




        public NewCompetenciesViewModel()
        {
            Title = "Competence Achievements";
        }




        public Command SaveCommand { get; }


        public Command CancelCommand { get; }




        async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }
        public List<QuestionFormElement> QuestionElements { get; set; }
        async void OnSave()
        {

            //APICase newItem = new APICase()
            //{
            //    CaseUID = Guid.NewGuid(),
            //    Text = Text,
            //    Description = Description
            //};

            //await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }


        public Guid SubmissionIdT { get; set; }
        public APISubmission SelectedSubmission { get; set; }

        public List<ElogbookAchievement> Questions { get; set; }
        public async Task LoadItemId(Guid item)
        {
            try
            {
                SubmissionIdT = item;
                //SelectedSubmission = item;
                //load questions here and assign them to a property List<APIQuestion>
                Questions = await DBService.GetSubmissionAchievements(item);
                
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }


        public override async Task InitializeAsync(object parameter)
        {
            Guid id = (Guid)parameter;
            await LoadItemId(id);

        }
    }
}