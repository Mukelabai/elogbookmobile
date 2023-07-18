using DevExpress.XamarinForms.Editors;
using elogbookapi.Models;
using elogbookapi.Models.API;
using elogbookmobile.Services;
using elogbookmobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.Helpers
{
    public class Utility
    {

        public static View GetElement(APIQuestion question, bool isChild)
        {
            string responseType = question.ResponseType;
            string options = question.QuestionOptions;

            View ctrl = null;

            if (responseType.ToLower() == "integer")
            {
                NumericEdit txt = new NumericEdit();
                txt.Value = null;
                txt.IsVisible = !isChild;


                ctrl = txt;

            }
            else if (responseType.ToLower() == "date")
            {
                DateEdit txt = new DateEdit();
                txt.IsVisible = !isChild;
                ctrl = txt;
            }
            else if (responseType.ToLower() == "float")
            {
                NumericEdit txt = new NumericEdit();
                txt.Value = null;
                txt.IsVisible = !isChild;
                ctrl = txt;
            }
            else if (responseType.ToLower() == "multiline")
            {
                MultilineEdit txt = new MultilineEdit();
                txt.IsVisible = !isChild;
                ctrl = txt;
            }
            else if (responseType.ToLower() == "singleselect")
            {

                ChoiceChipGroup txt = new ChoiceChipGroup();
                txt.IsMultiline = true;
                if (options != null)
                {
                    string[] items = options.Split(';');
                    for (int i = 0; i < items.Length; i++)
                    {
                        txt.Chips.Add(new Chip() { Text = items[i].Trim() });
                    }

                }
                txt.IsVisible = !isChild;
                ctrl = txt;
            }
            else if (responseType.ToLower() == "multiselect")
            {


                FilterChipGroup txt = new FilterChipGroup();
                txt.IsMultiline = true;

                if (options != null)
                {
                    string[] items = options.Split(';');

                    for (int i = 0; i < items.Length; i++)
                    {
                        txt.Chips.Add(new Chip() { Text = items[i].Trim() });
                    }

                }
                txt.IsVisible = !isChild;

                ctrl = txt;

            }

            else
            {
                TextEdit txt = new TextEdit();

                txt.IsVisible = !isChild;
                ctrl = txt;
            }

            //enable or disable supervisor controls
            
                
                if (question.IsForSupervisor)
                {
                    ctrl.IsEnabled = !App.ApplicationUser.IsStudent;
                }
                else
                {
                    ctrl.IsEnabled = App.ApplicationUser.IsStudent;
                }
            


            return ctrl;
        }
        public static void SetElementValues(string responseType, string response, View ctrl)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");



            if (responseType.ToLower() == "integer")
            {
                NumericEdit txt = ctrl as NumericEdit;
                txt.Value = Convert.ToInt32(response);

            }
            else if (responseType.ToLower() == "date")
            {
                DateEdit txt = ctrl as DateEdit;
                txt.Date = Convert.ToDateTime(response);
            }
            else if (responseType.ToLower() == "float")
            {
                NumericEdit txt = ctrl as NumericEdit;
                txt.Value = Convert.ToDecimal(response);
            }
            else if (responseType.ToLower() == "multiline")
            {
                MultilineEdit txt = ctrl as MultilineEdit;
                txt.Text = response.Trim();
            }
            else if (responseType.ToLower() == "singleselect")
            {

                ChoiceChipGroup txt = ctrl as ChoiceChipGroup;
                txt.IsMultiline = true;
                if (response != null)
                {
                    List<string> items = new List<string>();
                    foreach (string s in response.Split(';'))
                    {
                        items.Add(s.Trim());
                    }

                    foreach (Chip chip in txt.Chips)
                    {
                        chip.IsSelected = items.Contains(chip.Text.Trim());
                    }

                }

            }
            else if (responseType.ToLower() == "multiselect")
            {


                FilterChipGroup txt = ctrl as FilterChipGroup;
                txt.IsMultiline = true;
                if (response != null)
                {
                    List<string> items = new List<string>();
                    foreach (string s in response.Split(';'))
                    {
                        items.Add(s.Trim());
                    }

                    foreach (Chip chip in txt.Chips)
                    {
                        chip.IsSelected = items.Contains(chip.Text.Trim());
                    }

                }

            }

            else
            {
                TextEdit txt = ctrl as TextEdit;
                txt.Text = response.Trim();

            }



        }

        public static string GetElementValues(string responseType, View ctrl, string question)
        {
            string response = null;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            string errorMessage = string.Format("Please answer the question: {0}", question);


            if (responseType.ToLower() == "integer")
            {
                NumericEdit txt = ctrl as NumericEdit;
                if (txt.IsVisible && txt.Value == null && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                response = txt.IsVisible ? txt.Value.ToString() : null;

            }
            else if (responseType.ToLower() == "date")
            {
                DateEdit txt = ctrl as DateEdit;
                if (txt.IsVisible && txt.Date == null && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                response = txt.IsVisible ? txt.Date.ToString() : null;

            }
            else if (responseType.ToLower() == "float")
            {
                NumericEdit txt = ctrl as NumericEdit;
                if (txt.IsVisible && txt.Value == null && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                response = txt.IsVisible ? txt.Value.ToString() : null;
            }
            else if (responseType.ToLower() == "multiline")
            {
                MultilineEdit txt = ctrl as MultilineEdit;
                if (txt.IsVisible && string.IsNullOrWhiteSpace(txt.Text) && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                response = txt.IsVisible ? txt.Text.Trim().ToString() : null;
            }
            else if (responseType.ToLower() == "singleselect")
            {

                ChoiceChipGroup txt = ctrl as ChoiceChipGroup;
                txt.IsMultiline = true;
                if (txt.IsVisible && txt.SelectedChip == null && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                response = txt.IsVisible&& txt.SelectedChip!=null ? txt.SelectedChip.Text.Trim().ToString() : null;


            }
            else if (responseType.ToLower() == "multiselect")
            {


                FilterChipGroup txt = ctrl as FilterChipGroup;
                txt.IsMultiline = true;

                if (txt.IsVisible && (txt.SelectedChips == null || txt.SelectedChips.Count <= 0) && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                List<string> items = new List<string>();
                foreach (Chip c in txt.SelectedChips)
                {
                    items.Add(c.Text);
                }
                response = txt.IsVisible&& items.Count>0 ? string.Join(";", items.ToArray()) : null;



            }

            else
            {
                TextEdit txt = ctrl as TextEdit;
                if (txt.IsVisible && string.IsNullOrWhiteSpace(txt.Text) && txt.IsEnabled)
                {
                    txt.Focus();
                    throw new Exception(errorMessage);
                }

                response = txt.IsVisible ? txt.Text.Trim().ToString() : null;

            }



            return response;

        }

        public static async Task DisplayInfoMessage(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Information", message, "Ok");
        }
        public static async Task DisplayErrorMessage(Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Information", ex.Message, "Ok");
        }
        public static async Task DisplayErrorMessage(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Information", message, "Ok");
        }
        public static void setMenuItems(User u)
        {
            App.MenuViewModel.MenuItems.Clear();
            App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "About", ViewModelType = typeof(AboutViewModel), ImageName = "ic_info" });
            if (u != null && u.RoleName.ToLower().Contains("student"))
            {
                App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Submissions", ViewModelType = typeof(StudentSubmissionsViewModel), ImageName = "ic_popup" });
            }
            else
            {
                //staff
                App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Assignments", ViewModelType = typeof(AssignmentsViewModel), ImageName = "ic_popup" });
            }


            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Browse", ViewModelType = typeof(ItemsViewModel), ImageName = "ic_browse" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Scheduler", ViewModelType = typeof(SchedulerViewModel), ImageName = "ic_scheduler" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "DataGrid", ViewModelType = typeof(DataGridViewModel), ImageName = "ic_datagrid" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Charts", ViewModelType = typeof(ChartsViewModel), ImageName = "ic_charts" });
            //MenuItems.Add(new CustomDrawerMenuItem() { Name = "Popup", ViewModelType = typeof(PopupViewModel), ImageName = "ic_popup" });
            App.MenuViewModel.MenuItems.Add(new CustomDrawerMenuItem() { Name = "Logout", ViewModelType = typeof(LoginViewModel), ImageName = "ic_logout" });
            App.MenuViewModel.SelectedMenuItem = App.MenuViewModel.MenuItems[0];
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

       public static async Task LoadSubmissionDataForStudent(Guid SubmissionIdT)
        {
            APISubmissionData data = await APIUserService.GetSubmissionData(SubmissionIdT.ToString());
            //List<APIQuestion> questions = data.Questions;// await APIUserService.GetQuestionsForSubmission(Id.ToString());
            //await DBService.DeleteQuestions(Id);
            await DBService.DeleteSubmissionCases(SubmissionIdT);
            await DBService.DeleteSubmissionResponses(SubmissionIdT);
            await DBService.DeleteAllAssignmentResponses(SubmissionIdT);
            await DBService.DeleteAllComments(SubmissionIdT);
            await DBService.DeleteAllSubmissionAchievements(SubmissionIdT);


            //add cases
            await DBService.AddCase(data.Cases);
            
            //get responses
            List<APIQuestionResponse> responses = data.Responses;
            await DBService.AddResponses(responses);
            
            //save comments
            await DBService.AddComments(data.Comments);
            //save general questio responses
            await DBService.AddAssignmentResponses(data.AssignmentResponses);
            //save achievemnts
            await DBService.AddSubmissionAchievements(data.ElogbookAchievements);
        }

        public static async Task DownloadSubmissionsForStaff(int AssignmentId)
        {
            //get submission data
            APIAssignmentData data = await APIUserService.GetAssignmentData(AssignmentId);

            //delete questions, submissions, and competencies for assignment
            await DBService.DeleteQuestions(AssignmentId);
            await DBService.DeleteSubmissionsForAssignment(AssignmentId);
            await DBService.DeleteComptenciesForAssignment(AssignmentId);
            await DBService.DeleteachievementsForAssignment(AssignmentId);

            StringBuilder stringBuilder = new StringBuilder();
            //add submissions
            foreach (APIStaffSubmission s in data.Submissions)
            {
                //delete comments
                await DBService.DeleteSubmissionComments(s.SubmissionId);
                //delete responses
                await DBService.DeleteAllAssignmentResponses(s.SubmissionIdT);
                //now save submission
                await DBService.AddStaffSubmission(s);

            }
           // stringBuilder.AppendLine(string.Format("Submissions: {0}", data.Submissions.Count));

            
            //add questions

           await DBService.AddQuestion(data.Questions);

            //add competencies
            await DBService.AddComptencies(data.Competences);

            
           // stringBuilder.AppendLine(string.Format("Questions: {0}", data.Questions.Count));
            


            //  await Application.Current.MainPage.DisplayAlert("Saved Data", string.Format("Saved the following:\n {0}", stringBuilder.ToString()), "OK");

            //now download cases
            //get data for 10 submissions at a time
            int submissionCount = data.Submissions.Count;
            long[] submissionIds = (from s in data.Submissions
                                    select s.SubmissionId).Distinct().ToArray();
            var lists = SplitList(submissionIds.ToList(), 5);
            await DBService.DeleteAllCases();
            await DBService.DeleteAllResponses();

            int caseCount = 0;
            int responseCount = 0;
            int commentCount = 0;
            //for each set of ids, get case data
            foreach (List<long> ids in lists)
            {
                string commaIds = string.Join(",", ids.ToArray());
                APICaseData caseData = await APIUserService.GetCaseData(commaIds);
                if (caseData != null)
                {
                    foreach (APICase c in caseData.Cases)
                    {
                        await DBService.AddCase(c);
                        caseCount++;
                    }
                    foreach (APIResponses r in caseData.Responses)
                    {
                        await DBService.AddCaseResponses(r);
                        responseCount++;
                    }
                    //add comments
                    await DBService.AddComments(caseData.Comments);
                    commentCount += caseData.Comments.Count;

                    //save assignment responses
                    await DBService.AddAssignmentResponses(caseData.AssignmentResponses);

                    //save achievements
                    await DBService.AddSubmissionAchievements(caseData.Achievements);
                }
            }
        }
    }

    
}
