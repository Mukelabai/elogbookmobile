using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class StaffSubmissionDetailViewModel : BaseViewModel
    {
        public const string ViewName = "StaffSubmissionDetailPage";

        public StaffSubmissionDetailViewModel()
        {
            Grades = new ObservableRangeCollection<APIGrade>();
            Statuses = new ObservableRangeCollection<APISubmissionStatus> { new APISubmissionStatus { StatusName = "Approved" }, new APISubmissionStatus { StatusName = "Revise" }, new APISubmissionStatus { StatusName = "Pending" } };
            Comments = new ObservableRangeCollection<APISubmissionComment>();
            DeleteCommentCommand = new Command<APISubmissionComment>(OnDeleteItem);
            SaveStatusCommand = new Command<APISubmissionStatus>(OnSaveStatus);
            SaveGradeCommand = new Command<APIGrade>(OnSaveGrade);
            AddItemCommand = new Command(OnAddItem);
            AssignmentQuestionsCommand = new Command(OnAddAssignmentQuestions);
            UploadCommentCommand = new Command(async () => await UploadSubmissionComments());
            CompetenciesCommand = new Command(OnAddCompetencies);
        }
        //save grade
        public Command SaveGradeCommand { get; }
        async void OnSaveGrade(APIGrade g)
        {
            try
            {
                if (SelectedGrade == null || SelectedSubmission.Grade == SelectedGrade.GradeName)
                {
                    return;
                }

                SelectedSubmission.Grade = SelectedGrade.GradeName;
                SelectedSubmission.GradeId = SelectedGrade.GradeId;
                SelectedSubmission.GradedBy = App.ApplicationUser.WebUsername;
                SelectedSubmission.GradedOn = DateTime.Now;
                await DBService.UpdateSubmissionGrade(SelectedSubmission);
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }

        //save status
        public Command SaveStatusCommand { get; set; }
        async void OnSaveStatus(APISubmissionStatus status)
        {
            try
            {
                if (SelectedStatus == null)
                {
                    return;
                }
                SelectedSubmission.Status = SelectedStatus.StatusName;
                await DBService.UpdateSubmissionStatus(SelectedSubmission.SubmissionId, SelectedStatus.StatusName);
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
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

        public ObservableRangeCollection<APISubmissionStatus> Statuses { get; set; }
        APISubmissionStatus selectedStatus;
        public APISubmissionStatus SelectedStatus { get => selectedStatus; set => SetProperty(ref selectedStatus, value); }

        public ObservableRangeCollection<APIGrade> Grades { get; set; }

        string student, updatedOn, hospital, mentor, cases, achievementLabel, achievementColour;
        APIGrade grade;
        public string Student { get => this.student; set => SetProperty(ref this.student, value); }

        public string UpdatedOn { get => this.updatedOn; set => SetProperty(ref this.updatedOn, value); }

        public string Hospital { get => this.hospital; set => SetProperty(ref this.hospital, value); }
        public string Mentor { get => this.mentor; set => SetProperty(ref this.mentor, value); }
        public string Cases { get => this.cases; set => SetProperty(ref this.cases, value); }

        public string AchievementLabel { get => this.achievementLabel; set => SetProperty(ref this.achievementLabel, value); }
        public string AchievementColour { get => this.achievementColour; set => SetProperty(ref this.achievementColour, value); }

        public APIGrade SelectedGrade { get => this.grade; set => SetProperty(ref this.grade, value); }

        public async void OnAppearing()
        {
            Grades.Clear();
            var result = Task.Run(async () => await DBService.GetGrades()).Result;
            Grades.AddRange(result);
        }
        async Task UpdateProperties(APIStaffSubmission item)
        {

            UpdatedOn = item.UpdatedOn.ToLongDateString();
            Student = item.Student;
            Hospital = item.Hospital;
            Mentor = item.Mentor;
            Cases = item.Cases.ToString();
            AchievementColour = item.AchievementColour;
            AchievementLabel = item.AchievementLabel;
            SelectedStatus = Statuses.Where(s => s.StatusName == item.Status).FirstOrDefault();
            SelectedGrade = Grades.Where(g => g.GradeId == item.GradeId).FirstOrDefault();

            LoadComments(item);
        }
        public async void LoadComments(APIStaffSubmission item)
        {
            Comments.Clear();
            Comments.AddRange(await DBService.GetSubmissionComments(item.SubmissionId));
        }
        public APIStaffSubmission SelectedSubmission { get; set; }
        public async Task LoadItemId(APIStaffSubmission submission)
        {
            try
            {

                SelectedSubmission = submission;
                Title = SelectedSubmission.Student;

                await UpdateProperties(SelectedSubmission);

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }

        public override async Task InitializeAsync(object parameter)
        {
            await LoadItemId(parameter as APIStaffSubmission);
        }

        //sends this to cass view model
        public Command AddItemCommand { get; }
        async void OnAddItem(object obj)
        {

            await Navigation.NavigateToAsync<StaffCasesViewModel>(SelectedSubmission);
        }

        public Command AssignmentQuestionsCommand { get; }
        async void OnAddAssignmentQuestions(object obj)
        {

            await Navigation.NavigateToAsync<StaffNewAssignmentQuestionsViewModel>(SelectedSubmission);
        }

        public Command UploadCommentCommand { get; }
        public async Task UploadSubmissionComments()
        {
            IsBusy = true;
            try
            {
                string student = string.Format("{0}{1}", Student, Student.ToLower().EndsWith("s") ? "'" : "'s");
                bool accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", string.Format($"Are you sure you want to upload your comments for {student} submission?"), "Yes", "No");
                if (!accept)
                {
                    return;
                }
                APISubmissionCommentData data = new APISubmissionCommentData();
                //get all submissions and all comments



                //long submissionId = 10;
                //create list of errorData
                List<APIError> errorData = new List<APIError>();


                List<APIStaffSubmission> dataSubmissions = new List<APIStaffSubmission>();
                List<APISubmissionComment> dataComments = new List<APISubmissionComment>();
                List<APIResponses> dataResponses = new List<APIResponses>();
                List<APIAssignmentQuestionResponse> dataGeneralResponses = new List<APIAssignmentQuestionResponse>();
                //get submissions
                dataSubmissions.Add(SelectedSubmission);


                //get comments for submissions
                dataComments.AddRange(await DBService.GetSubmissionComments(SelectedSubmission.SubmissionId));
                //get responses for submissions
                dataResponses.AddRange(await DBService.GetSubmissionResponses(SelectedSubmission.SubmissionId));
                //get general responses
                dataGeneralResponses.AddRange(await DBService.GetSubmissionGeneralResponses(SelectedSubmission.SubmissionId));
                foreach (APIAssignmentQuestionResponse r in dataGeneralResponses)
                {
                    dataResponses.Add(new APIResponses
                    {
                        CaseId = -1,
                        QuestionId = r.QuestionId,
                        ResponseText = r.ResponseText,
                        SubmissionId = r.SubmissionId
                    });
                }

                //now send data for this list
                data.Submissions = dataSubmissions;
                data.Comments = dataComments;
                data.Responses = dataResponses;
                data.WebUsername = App.ApplicationUser.WebUsername;
                //now post
                List<APIError> messageList = await APIUserService.PostStaffSubmissionData(data);
                errorData.AddRange(messageList);



                string[] infoMessages = (from m in errorData.Where(e => e.MessageType == "I")
                                         select m.ErrorMessage).ToArray();
                string[] errorMessages = (from m in errorData.Where(e => e.MessageType == "E")
                                          select m.ErrorMessage).ToArray();

                //await Utility.DisplayInfoMessage("Successfully uploaded submission comments:\n" + string.Join("\n", infoMessages));
                await Utility.DisplayInfoMessage("Successfully uploaded submission comments");

                if (errorMessages.Length > 0)
                {
                    await Utility.DisplayErrorMessage(string.Join("\n", errorMessages));
                }


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

        public Command CompetenciesCommand { get; }
        async void OnAddCompetencies(object obj)
        {
           
            await Navigation.NavigateToAsync<NewCompetenciesViewModel>(SelectedSubmission.SubmissionIdT);
        }
    }
}
