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
   public class NewAssignmentQuestionsViewModel:BaseViewModel
    {
        public const string ViewName = "NewAssignmentQuestionsPage";


        

        public NewAssignmentQuestionsViewModel()
        {
            Title = "General Questions";
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
        
        public List<APIQuestion> Questions { get; set; }
        public async Task LoadItemId(APISubmission item)
        {
            try
            {
                SubmissionIdT = item.SubmissionIdT;
                SelectedSubmission = item;
                //load questions here and assign them to a property List<APIQuestion>
                Questions = await DBService.GetSubmissionQuestions(item.AssignmentId);
                //get assignment level questions
                Questions = Questions.Where(q => q.IsAssignmentLevel).ToList();
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }
      

        public override async Task InitializeAsync(object parameter)
        {
            
                await LoadItemId(parameter as APISubmission);
            
        }
    }
}
