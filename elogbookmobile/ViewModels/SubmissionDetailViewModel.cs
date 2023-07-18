using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class SubmissionDetailViewModel : BaseViewModel
    {
        public const string ViewName = "SubmissionDetailPage";

        string createdBy, createdOn, updatedBy, updatedOn, rotation, hospital, mentor, cases, status, grade, isPublished,achievementLabel;

        double achievementProgress;

        public SubmissionDetailViewModel()
        {
            Title = "Submission Details";
            Comments = new ObservableRangeCollection<APISubmissionComment>();
            AddItemCommand = new Command(OnAddItem);
            AssignmentQuestionsCommand = new Command(OnAddAssignmentQuestions);
            CompetenciesCommand = new Command(OnAddCompetencies);
            LoadQuestionsCommand = new Command(async () => await LoadSubmissionData());
            PublishSubmissionCommand = new Command(async () => await Publish());
            SyncSubmissionCommand = new Command(async () => await SyncSubmission());
            DeleteCommentCommand = new Command<APISubmissionComment>(OnDeleteItem);

        }

        //delete comment
        public Command DeleteCommentCommand { get; }
        async void OnDeleteItem(APISubmissionComment item)
        {
            try
            {
                bool accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", "Do you want to remove this comment?", "Yes", "No");
                if (item.CreatedBy != App.ApplicationUser.WebUsername)
                {
                    await Utility.DisplayInfoMessage("Sorry, you cannot delete this comment because you did not make it");
                    return;
                }
                if (accept)
                {
                    await DBService.DeleteComment(item);
                    LoadComments(SelectedSubmission);
                }
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }


        //comments
        public ObservableRangeCollection<APISubmissionComment> Comments { get; set; }
        //sync command
        public Command SyncSubmissionCommand { get; set; }
        async Task SyncSubmission()
        {
            IsBusy = true;
            try
            {
                string published = SelectedSubmission.IsPublished ? "PUBLISHED" : "UNPUBLISHED";
                string willWont = SelectedSubmission.IsPublished ? "WILL" : "WON'T";
                var accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", string.Format("Note that this submission is {0}, therefore, your supervisor {1} see it.\nThis data will replace everything on the server regarding this submission, proceed?", published, willWont), "Yes", "No");
                if (!accept)
                {
                    return;
                }
                APISubmissionData data = new APISubmissionData();
                data.SubmissionIdT = SelectedSubmission.SubmissionIdT;
                data.Submission = SelectedSubmission;
                //get submission responses
                List<APIQuestionResponse> responses = await DBService.GetSubmissionResponses(data.SubmissionIdT);
                //if (responses == null || responses.Count <= 0)
                //{
                //    throw new Exception("This submission has no cases recorded yet, please record cases then upload its data.");
                //}
                data.Responses = responses;
                List<APICase> cases = await DBService.GetSubmissionCases(data.SubmissionIdT);
                data.Cases = cases;
                data.Comments = await DBService.GetSubmissionComments(data.SubmissionIdT);
                data.AssignmentResponses = await DBService.GetAssignmentResonses(data.SubmissionIdT);
                data.ElogbookAchievements = await DBService.GetSubmissionAchievements(data.SubmissionIdT);
                //set question text to null to reduce size
                foreach (ElogbookAchievement ea in data.ElogbookAchievements)
                {
                    ea.QuestionText = null;
                }

                await APIUserService.PostSubmissionData(data);
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        public Command PublishSubmissionCommand { get; }
        async Task Publish()
        {
            bool proceed = false;
            if (SelectedSubmission.IsPublished)
            {
                proceed = await Application.Current.MainPage.DisplayAlert("Confirm Action", "This submission is PUBLISHED, do you want to UN PUBLISH it", "Yes", "No");
                if (proceed)
                {
                    SelectedSubmission.IsPublished = false;
                    SelectedSubmission.UpdatedOn = DateTime.Now;
                    await DBService.AddSubmission(SelectedSubmission);
                    await UpdateProperties(SelectedSubmission);
                }
            }
            else
            {
                proceed = await Application.Current.MainPage.DisplayAlert("Confirm Action", "Do you want to publish this submission--this will make it visible to your supervisor when you send it to the server (tapping on SEND DATA), proceed?", "Yes", "No");
                if (proceed)
                {
                    SelectedSubmission.IsPublished = true;
                    SelectedSubmission.UpdatedOn = DateTime.Now;
                    await DBService.AddSubmission(SelectedSubmission);
                    await UpdateProperties(SelectedSubmission);
                }
            }

        }
        public APISubmission SelectedSubmission { get; set; }
        public Guid Id { get; set; }



        public string CreatedBy { get => this.createdBy; set => SetProperty(ref this.createdBy, value); }
        public string CreatedOn { get => this.createdOn; set => SetProperty(ref this.createdOn, value); }
        public string UpdatedBy { get => this.updatedBy; set => SetProperty(ref this.updatedBy, value); }
        public string UpdatedOn { get => this.updatedOn; set => SetProperty(ref this.updatedOn, value); }
        public string Rotation { get => this.rotation; set => SetProperty(ref this.rotation, value); }
        public string Hospital { get => this.hospital; set => SetProperty(ref this.hospital, value); }
        public string Mentor { get => this.mentor; set => SetProperty(ref this.mentor, value); }
        public string Cases { get => this.cases; set => SetProperty(ref this.cases, value); }
        public string Status { get => this.status; set => SetProperty(ref this.status, value); }
        public string Grade { get => this.grade; set => SetProperty(ref this.grade, value); }
        public string IsPublished { get => this.isPublished; set => SetProperty(ref this.isPublished, value); }
        public double AchievementProgress { get => achievementProgress; set => SetProperty(ref achievementProgress, value); }

        public string AchievementLabel { get => achievementLabel; set => SetProperty(ref this.achievementLabel, value); }

        public async Task UpdateProperties(APISubmission item)
        {

            Id = item.SubmissionIdT;
            CreatedBy = item.CreatedBy;
            CreatedOn = item.CreatedOn.ToLongDateString();
            UpdatedBy = item.UpdatedBy;
            UpdatedOn = item.UpdatedOn.ToLongDateString();
            Rotation = item.Rotation;
            Hospital = item.Hospital;
            Mentor = item.Mentor;
            Cases = item.Cases.ToString();
            Status = item.Status;
            Grade = item.Grade;
            IsPublished = item.IsPublished ? "Yes (visible to supervisors)" : "No (not visible to supervisors)";
            LoadComments(item);
            LoadAchievements(item);
            

        }
        public async void LoadComments(APISubmission item)
        {
            Comments.Clear();
            Comments.AddRange(await DBService.GetSubmissionComments(item.SubmissionIdT));
        }
        public async void LoadAchievements(APISubmission item)
        {
            //load achivement progress
            List<ElogbookAchievement> achievements = await DBService.GetSubmissionAchievements(item.SubmissionIdT);
            //get all competencies
            List<APIElogbookCompetence> competences = await DBService.GetComptencies(item.AssignmentId);
            competences = competences.Where(c => c.Expected > 0).ToList();
            double expected = 0;
            if (competences != null && competences.Count > 0)
            {
                expected = (from c in competences select c.Expected).Sum();
            }
            double achieved = 0;
            if (achievements != null && achievements.Count > 0)
            {
                achieved = (from c in achievements select c.Achieved).Sum();
            }
            AchievementProgress = expected == 0 ? 1 : achieved / expected;
            AchievementLabel = string.Format($"Overall competence achievement: {Math.Round(AchievementProgress*100)}% ({achieved}/{expected})");
            
        }
        public async Task LoadItemId(Guid itemId)
        {
            try
            {
                var item = await DBService.GetSubmission(itemId);
                SelectedSubmission = item;
                await UpdateProperties(SelectedSubmission);

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }

        public override async Task InitializeAsync(object parameter)
        {
            await LoadItemId(Guid.Parse(parameter.ToString()));
        }

        public Command LoadQuestionsCommand { get; }

        public async Task LoadSubmissionData()
        {
            string message = NoCasesFound ? "No cases found for this submission. Do you want to download from the server if any exist?" : "Proceed to remove local cases, and responses for this submission and get those from the server?";
            var accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", message, "Yes", "No");
            if (!accept)
            {
                return;
            }
            IsBusy = true;
            try
            {
                await Utility.LoadSubmissionDataForStudent(Id);



                await UpdateProperties(SelectedSubmission);
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
            finally
            {
                IsBusy = false;
            }
            await Application.Current.MainPage.DisplayAlert("Downloaded data", "Download Completed", "OK");

        }
        public Command AddItemCommand { get; }
        public bool NoCasesFound { get; set; }
        async void OnAddItem(object obj)
        {
            //List<APICase> submissionCases = await DBService.GetSubmissionCases(Id);
            //if (submissionCases.Count <= 0)
            //{
            //    //var accept = await Application.Current.MainPage.DisplayAlert("No Cases Found", "Do you want to download cases for this submission from the server, if any?", "Yes","No");
            //    //if (accept)
            //    //{
            //    //NoCasesFound = true;
            //    //await LoadSubmissionData();
            //    //automatcally download existing case data to avoid loses
            //    await Utility.LoadSubmissionDataForStudent(Id);
            //    await Navigation.NavigateToAsync<CasesViewModel>(Id);
            //    //}
            //}
            //else
            //{
            await Navigation.NavigateToAsync<CasesViewModel>(Id);
            //}


        }

        public Command AssignmentQuestionsCommand { get; }
        async void OnAddAssignmentQuestions(object obj)
        {
            //List<APIQuestion> addedQuestions = await DBService.GetSubmissionQuestions(Id);
            //if (addedQuestions.Count <= 0)
            //{
            //    await Application.Current.MainPage.DisplayAlert("No Questions Found", "Please tap on Download DATA to pull questions, cases, and responses from the server. Ensure that you have internet access.", "OK");
            //    return;
            //}
            await Navigation.NavigateToAsync<NewAssignmentQuestionsViewModel>(SelectedSubmission);
        }

        public Command CompetenciesCommand { get; }
        async void OnAddCompetencies(object obj)
        {
         
            await Navigation.NavigateToAsync<NewCompetenciesViewModel>(SelectedSubmission.SubmissionIdT);
        }
    }
}
