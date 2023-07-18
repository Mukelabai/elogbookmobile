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
    public class StaffNewCaseViewModel : BaseViewModel
    {
        public const string ViewName = "StaffNewCasePage";


        

        public StaffNewCaseViewModel()
        {
            
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
        public APICase SelectedCase { get; set; }
        string selectedPatient;
        public string SelectedPatient { get => selectedPatient; set => SetProperty(ref selectedPatient, value); }
        public List<APIQuestion> Questions { get; set; }
        
        public async Task LoadCaseItemId(APICase item)
        {
            try
            {
                SelectedPatient = item.Patient;
                SubmissionIdT = item.SubmissionIdT;
                SelectedCase = item;
                //load questions here and assign them to a property List<APIQuestion>
                Questions = await DBService.GetSubmissionQuestions(item.AssignmentId);
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
           
                await LoadCaseItemId(parameter as APICase);
            
        }

    }
}
