using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class StaffNewCompetenciesViewModel : BaseViewModel
    {
        public const string ViewName = "NewCompetenciesPage";




        public StaffNewCompetenciesViewModel()
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


       
        public APIAssignment SelectedAssignment { get; set; }

        public List<ElogbookAchievement> Questions { get; set; }
        public async Task LoadItemId(APIAssignment item)
        {
            try
            {
                
                //SelectedSubmission = item;
                //load questions here and assign them to a property List<APIQuestion>
                Questions = await DBService.GetSummarisedAchievementsForAssignment(item.AssignmentId);

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }


        public override async Task InitializeAsync(object parameter)
        {
            
            await LoadItemId(parameter as APIAssignment);

        }
    }
}