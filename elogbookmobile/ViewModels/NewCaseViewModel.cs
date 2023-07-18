using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
   public class NewCaseViewModel:BaseViewModel
    {
        public const string ViewName = "NewCasePage";


        string text;
        string description;

        public NewCaseViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }


      
        
        public Command SaveCommand { get; }

        
        public Command CancelCommand { get; }


        bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(this.text)
                && !String.IsNullOrWhiteSpace(this.description);
        }

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


        public APISubmission Submission { get; set; }
        public APICase SelectedCase { get; set; }
        string selectedPatient;
        public string SelectedPatient { get=>selectedPatient; set=>SetProperty(ref selectedPatient,value); }
        public List<APIQuestion> Questions { get; set; }
        public async Task LoadItemId(APISubmission item)
        {
            try
            {
                Submission = item;
                //load questions here and assign them to a property List<APIQuestion>
                Questions = await DBService.GetSubmissionQuestions(item.AssignmentId);
                Questions = Questions.Where(q => !q.IsAssignmentLevel).ToList();
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }
        public async Task LoadCaseItemId(APICase item)
        {
            try
            {
                SelectedPatient = item.Patient;
               
                SelectedCase = item;
                Submission = await DBService.GetSubmission(item.SubmissionIdT);
                //load questions here and assign them to a property List<APIQuestion>
                Questions = await DBService.GetSubmissionQuestions(Submission.AssignmentId);
                //get non-assignment-level questions only
                Questions = Questions.Where(q => !q.IsAssignmentLevel).ToList();

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }

        public override async Task InitializeAsync(object parameter)
        {
            if (parameter is APISubmission)
            {
                await LoadItemId(parameter as APISubmission);
            }
            else
            {
                await LoadCaseItemId(parameter as APICase);
            }
        }

    }
}
