using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class StaffSubmissionsViewModel : BaseViewModel
    {
        APIStaffSubmission _selectedItem;


        public StaffSubmissionsViewModel()
        {
            Title = "Submissions";
            StudentSubmissions = new ObservableCollection<APIStaffSubmission>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<APIStaffSubmission>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            EditSubmissionCommand = new Command<APIStaffSubmission>(OnEditItem);
            DownloadSubmissionsCommand = new Command(async () => await DownloadSubmissions());
            UploadSubmissionsCommand = new Command(async () => await UploadSubmissions());
            CompetenciesCommand = new Command(OnAddCompetencies);
        }


        public ObservableCollection<APIStaffSubmission> StudentSubmissions { get; }


        public Command LoadItemsCommand { get; }

        public Command AddItemCommand { get; }
        public Command EditSubmissionCommand { get; }

        public Command<APIStaffSubmission> ItemTapped { get; }

        public APIStaffSubmission SelectedItem
        {
            get => this._selectedItem;
            set
            {
                SetProperty(ref this._selectedItem, value);
                OnItemSelected(value);
            }
        }



        public async void OnAppearing()
        {
            SelectedItem = null;
            await ExecuteLoadItemsCommand();
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (SelectedAssignment == null)
                {
                    return;
                }

                var items = await DBService.GetStaffSubmissions(SelectedAssignment.AssignmentId); //await DataStore.GetItemsAsync(true);
                //if items don't exist, ask user to download.
                if (items == null || items.Count <= 0)
                {
                    bool accept = await Application.Current.MainPage.DisplayAlert("Submissions not found", "This assignment does not have submissions on your device, do you want to download them?", "Yes", "No");
                    if (accept)
                    {
                        NoPromptForDownload = true;
                        await DownloadSubmissions();
                    }
                }
                else
                {
                    LoadSubmissionData(items);
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

        async void OnAddItem(object obj)
        {
            await Navigation.NavigateToAsync<NewSubmissionViewModel>(null);
        }
        async void OnEditItem(APIStaffSubmission item)
        {
            await Navigation.NavigateToAsync<NewSubmissionViewModel>(item);
        }

        async void OnItemSelected(APIStaffSubmission item)
        {
            if (item == null)
                return;
            await Navigation.NavigateToAsync<StaffSubmissionDetailViewModel>(item);
        }

        public APIAssignment SelectedAssignment { get; set; }
        public async Task LoadItemId(APIAssignment assignment)
        {
            try
            {

                SelectedAssignment = assignment;
                await ExecuteLoadItemsCommand();

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

        public bool NoPromptForDownload { get; set; }
        public Command DownloadSubmissionsCommand { get; }

        public async Task DownloadSubmissions()
        {
            IsBusy = true;
            try
            {
                if (NoPromptForDownload == false)
                {
                    var accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", "This action downloads all student submissions and replaces whatever you have on your device; if you graded or commented on some submissions on your device, you must upload them before downloading. Do you want to proceed to download?", "Yes", "No");
                    if (!accept)
                    {
                        return;
                    }
                }
                await Utility.DownloadSubmissionsForStaff(SelectedAssignment.AssignmentId);
                var items = await DBService.GetStaffSubmissions(SelectedAssignment.AssignmentId);

                LoadSubmissionData(items);
                await Application.Current.MainPage.DisplayAlert("Saved Data", string.Format("Downloaded data for {0} submissions", StudentSubmissions.Count), "OK");
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

        private async void LoadSubmissionData(List<APIStaffSubmission> items)
        {
            //get all achivements for this assignment
            List<ElogbookAchievement> achievements = await DBService.GetAchievementsForAssignment(SelectedAssignment.AssignmentId);
            List<APIElogbookCompetence> competences = await DBService.GetComptencies(SelectedAssignment.AssignmentId);
            //now load
            StudentSubmissions.Clear();
            foreach (var item in items)
            {
                //get achiveements for submission
                if (competences != null && competences.Count > 0)
                {
                    List<ElogbookAchievement> submissionAchivements = achievements.Where(a => a.SubmissionId == item.SubmissionId).ToList();
                    double expected = (from c in competences select c.Expected).Sum();
                    double achieved = submissionAchivements == null ? 0 : (from s in submissionAchivements select s.Achieved).Sum();
                    double perc = expected == 0 ? 100 : Math.Round((achieved / expected) * 100);
                    item.Achievement = perc;
                    item.AchievementLabel = string.Format($"{perc}% ({achieved}/{expected})");
                }
                StudentSubmissions.Add(item);
            }
        }

        public Command UploadSubmissionsCommand { get; }
        public async Task UploadSubmissions()
        {
            IsBusy = true;
            try
            {
                bool accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", "Are you sure you want to upload your comments for this assignment?", "Yes", "No");
                if (!accept)
                {
                    return;
                }
                APISubmissionCommentData data = new APISubmissionCommentData();
                //get all submissions and all comments
                List<APIStaffSubmission> submissions = await DBService.GetStaffSubmissions(SelectedAssignment.AssignmentId);
                //List<APISubmissionComment> comments = await DBService.GetAssignmentComments(SelectedAssignment.AssignmentId);
                //List<APIResponses> responses = await DBService.GetAssignmentResponses(SelectedAssignment.AssignmentId);
                //List<APIAssignmentQuestionResponse> generalResponses = await DBService.GetAssignmentGeneralResponses(SelectedAssignment.AssignmentId);
                ////now add general responses to list of responses
                //foreach (APIAssignmentQuestionResponse r in generalResponses)
                //{
                //    responses.Add(new APIResponses { CaseId = -1, QuestionId = r.QuestionId, ResponseText = r.ResponseText, SubmissionId = r.SubmissionId });
                //}
                //send submissions in batches

                long[] submissionIds = (from s in submissions
                                        select s.SubmissionId).Distinct().ToArray();
                var lists = Utility.SplitList(submissionIds.ToList(), 3);

                //foreach(APIStaffSubmission submission in submissions)
                //{
                //    try
                //    {
                //        List<APIStaffSubmission> dataSubmissions = new List<APIStaffSubmission>();
                //        List<APISubmissionComment> dataComments = new List<APISubmissionComment>();
                //        List<APIResponses> dataResponses = new List<APIResponses>();
                //        List<APIAssignmentQuestionResponse> dataGeneralResponses = new List<APIAssignmentQuestionResponse>();
                //        dataSubmissions.Add(submission);
                //        //get comments for submissions
                //        dataComments.AddRange(await DBService.GetSubmissionComments(submission.SubmissionId));
                //        //get responses for submissions
                //        dataResponses.AddRange(await DBService.GetSubmissionResponses(submission.SubmissionId));
                //        //get general responses
                //        dataGeneralResponses.AddRange(await DBService.GetSubmissionGeneralResponses(submission.SubmissionId));
                //        foreach (APIAssignmentQuestionResponse r in dataGeneralResponses)
                //        {
                //            dataResponses.Add(new APIResponses { CaseId = -1, QuestionId = r.QuestionId, ResponseText = r.ResponseText, SubmissionId = r.SubmissionId });
                //        }
                //        data.Submissions = dataSubmissions;
                //        data.Comments = dataComments;
                //        data.Responses = dataResponses;
                //        data.WebUsername = App.ApplicationUser.WebUsername;
                //        //now post
                //        List<APIError> messageList = await APIUserService.PostStaffSubmissionData(data);
                //        errorData.AddRange(messageList);
                //    }
                //    catch(Exception ex)
                //    {

                //    }

                //}

                //long submissionId = 10;
                //create list of errorData
                List<APIError> errorData = new List<APIError>();
                foreach (List<long> ids in lists)
                {
                    //if (!ids.Contains(submissionId))
                    //{
                    //    continue;
                    //}
                    List<APIStaffSubmission> dataSubmissions = new List<APIStaffSubmission>();
                    List<APISubmissionComment> dataComments = new List<APISubmissionComment>();
                    List<APIResponses> dataResponses = new List<APIResponses>();
                    List<APIAssignmentQuestionResponse> dataGeneralResponses = new List<APIAssignmentQuestionResponse>();
                    //get submissions
                    dataSubmissions.AddRange(submissions.Where(s => ids.Contains(s.SubmissionId)));
                    foreach (long id in ids)
                    {

                        //get comments for submissions
                        dataComments.AddRange(await DBService.GetSubmissionComments(id));
                        //get responses for submissions
                        dataResponses.AddRange(await DBService.GetSubmissionResponses(id));
                        //get general responses
                        dataGeneralResponses.AddRange(await DBService.GetSubmissionGeneralResponses(id));
                        foreach (APIAssignmentQuestionResponse r in dataGeneralResponses)
                        {
                            dataResponses.Add(new APIResponses { CaseId = -1, QuestionId = r.QuestionId, ResponseText = r.ResponseText, SubmissionId = r.SubmissionId });
                        }
                    }
                    //now send data for this list
                    data.Submissions = dataSubmissions;
                    data.Comments = dataComments;
                    data.Responses = dataResponses;
                    data.WebUsername = App.ApplicationUser.WebUsername;
                    //now post
                    List<APIError> messageList = await APIUserService.PostStaffSubmissionData(data);
                    errorData.AddRange(messageList);

                }

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

            await Navigation.NavigateToAsync<StaffNewCompetenciesViewModel>(SelectedAssignment);
        }
    }
}

