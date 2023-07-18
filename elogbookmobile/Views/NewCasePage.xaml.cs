﻿using DevExpress.XamarinForms.DataForm;
using DevExpress.XamarinForms.Editors;
using elogbookapi.Models;
using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using elogbookmobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace elogbookmobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewCasePage : ContentPage
    {
        public NewCasePage()
        {
            InitializeComponent();
            QuestionElements = new List<QuestionFormElement>();
            BindingContext = ViewModel = new NewCaseViewModel();

        }
        public NewCaseViewModel ViewModel { get; }
        public List<QuestionFormElement> QuestionElements { get; set; }
        protected override void OnAppearing()
        {
            base.OnAppearing();


        }
        private Label GetSectionLabel(string title)
        {
            return new Label
            {
                Text = title,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
        }
        protected void AddControls()
        {
            bool userIsStudent = App.ApplicationUser.RoleName.ToLower().Contains("student");
            QuestionElements.Clear();
            List<APIQuestion> allQuestions = ViewModel.Questions;
            if (allQuestions.Count <= 0)
            {
                return;
            }
            //get sections
            string[] sections = (from sec in allQuestions
                                 orderby sec.SectionOrder
                                 select sec.SectionName
                                 ).Distinct().ToArray();
            foreach (string section in sections)
            {
                APIQuestion[] secrecords = allQuestions.Where(q => q.SectionName == section).OrderBy(t => t.DisplayOrder).ToArray();
                //create section expanders
                //Expander secRP = new Expander
                //{
                //    Header = GetSectionLabel(section)
                //};
                //secRP.IsExpanded = true;


                StackLayout secSL = Panel;// new StackLayout();
                Label secLabel = GetSectionLabel(section);
                secSL.Children.Add(secLabel);
                //secRP.Content = secSL;
                //Panel.Children.Add(secRP);
                //set elements

                foreach (APIQuestion question in secrecords)
                {
                    //now display question
                    Label qLabel = new Label();
                    qLabel.Text = question.QuestionText;


                    if (question.IsSub)
                    {
                        qLabel.IsVisible = false;//hide label since it will be activated by parent choice
                    }

                    secSL.Children.Add(qLabel);

                    View questionControl = null;


                    //now get the control
                    questionControl = Utility.GetElement(question, question.IsSub);

                    secSL.Children.Add(questionControl);

                    //add empty label
                    secSL.Children.Add(new Label());

                    //if the question has sub questions, let's add event listeners to show the sub questions
                    if (question.HasSub && !string.IsNullOrWhiteSpace(question.QuestionOptions))
                    {
                        //create event
                        //create event
                        if (question.ResponseType.ToLower() == "multiselect")
                        {
                            FilterChipGroup chkP = questionControl as FilterChipGroup;
                            chkP.SelectionChanged += FilterChipGroup_SelectionChanged;


                        }
                        else if (question.ResponseType.ToLower() == "singleselect")
                        {
                            ChoiceChipGroup chkP = questionControl as ChoiceChipGroup;
                            chkP.SelectionChanged += ChoiceChipGroup_SelectionChanged;

                        }
                    }

                    //add question controls to form elements
                    QuestionElements.Add(new QuestionFormElement() { SectionControl = secSL, QuestionControl = questionControl, QuestionLabel = qLabel, Question = question });

                }

            }
            //add save button


            ViewModel.QuestionElements = QuestionElements;
        }





        private async void btnLoadPatient_Clicked(object sender, EventArgs e)
        {
            try
            {
                Panel.Children.Clear();
                AddControls();
                btnLoadPatient.IsEnabled = false;
                btnSaveData.IsVisible = true;
                LoadCaseData();
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);

            }
        }

        private void Patient_TextChanged(object sender, EventArgs e)
        {
            btnLoadPatient.IsEnabled = !string.IsNullOrWhiteSpace(Patient.Text);
            Title = string.Format("{0}'s Case Details", Patient.Text.ToUpper());
            Patient.Text = Patient.Text.ToUpper();
        }

        private void FilterChipGroup_SelectionChanged(object sender, EventArgs e)
        {
            FilterChipGroup parentCtrl = sender as FilterChipGroup;
            QuestionFormElement parentFormElement = QuestionElements.Where(q => q.QuestionControl == parentCtrl).FirstOrDefault();
            if (parentFormElement == null)
            {
                return;
            }
            //get controls where child control is not null and parent control is this one
            QuestionFormElement[] childElements = QuestionElements.Where(q => q.Question.ParentId == parentFormElement.Question.QuestionId).ToArray();
            //set visbile
            foreach (Chip chip in parentCtrl.SelectedChips)
            {
                //get element whose parent option matches this chip's text
                QuestionFormElement[] cfes = childElements.Where(q => q.Question.ParentOption.Trim() == chip.Text.Trim()).ToArray();
                foreach (QuestionFormElement cfe in cfes)
                {
                    cfe.QuestionControl.IsVisible = true;
                    cfe.QuestionLabel.IsVisible = true;
                }

            }
            //set invisible
            foreach (Chip nonSelectedchip in parentCtrl.Chips)
            {
                //get element whose parent option matches this chip's text
                if (!nonSelectedchip.IsSelected)
                {
                    QuestionFormElement[] cfes = childElements.Where(q => q.Question.ParentOption.Trim() == nonSelectedchip.Text.Trim()).ToArray();
                    foreach (QuestionFormElement cfe in cfes)
                    {
                        cfe.QuestionControl.IsVisible = false;
                        cfe.QuestionLabel.IsVisible = false;
                    }
                }

            }




        }

        private void ChoiceChipGroup_SelectionChanged(object sender, EventArgs e)
        {
            ChoiceChipGroup parentCtrl = sender as ChoiceChipGroup;
            QuestionFormElement parentFormElement = QuestionElements.Where(q => q.QuestionControl == parentCtrl).FirstOrDefault();
            if (parentFormElement == null)
            {
                return;
            }
            //get controls where parent control is this one
            QuestionFormElement[] childElements = QuestionElements.Where(q => q.Question.ParentId == parentFormElement.Question.QuestionId).ToArray();
            //set visbile
            Chip chip = parentCtrl.SelectedChip;

            //get element whose parent option matches this chip's text
            QuestionFormElement[] cfes = childElements.Where(q => q.Question.ParentOption.Trim() == chip.Text.Trim()).ToArray();
            foreach (QuestionFormElement cfe in cfes)
            {
                cfe.QuestionControl.IsVisible = true;
                cfe.QuestionLabel.IsVisible = true;
            }


            //set invisible
            foreach (Chip nonSelectedchip in parentCtrl.Chips)
            {
                //get element whose parent option matches this chip's text
                if (!nonSelectedchip.IsSelected)
                {
                    QuestionFormElement[] ncfes = childElements.Where(q => q.Question.ParentOption.Trim() == nonSelectedchip.Text.Trim()).ToArray();
                    foreach (QuestionFormElement ncfe in ncfes)
                    {
                        ncfe.QuestionControl.IsVisible = false;
                        ncfe.QuestionLabel.IsVisible = false;
                    }
                }

            }
        }

        async void btnSaveData_Clicked(object sender, EventArgs e)
        {
            try
            {
                //check patient intials exist
                if (string.IsNullOrWhiteSpace(Patient.Text))
                {
                    throw new Exception("Specify Patient initials");
                }
                string initials = Patient.Text.Trim();
                //check whether intials already exist for this submission

                APICase patientCase = await DBService.GetSubmissionCase(ViewModel.Submission.SubmissionIdT, initials);
                //delete existing case so that we re add it.

                //if (caseExists)
                //{
                //    throw new Exception("A case with the same patient initials exists, please use different intials. If this is a different patient with the same initials, then append a number.");
                //}
                User user = App.ApplicationUser;
                //create case
                if (patientCase == null)
                {
                    patientCase = new APICase
                    {
                        CaseUID = Guid.NewGuid(),
                        Patient = initials,
                        SubmissionIdT = ViewModel.Submission.SubmissionIdT,
                        StudentId = user.UserId,
                        CreatedBy = user.WebUsername,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = user.WebUsername,
                        UpdatedOn = DateTime.Now
                    };
                    //save case
                    await DBService.AddCase(patientCase);
                }
                else
                {
                    patientCase.Patient = initials;
                    patientCase.UpdatedBy = user.WebUsername;
                    patientCase.UpdatedOn = DateTime.Now;
                    await DBService.UpdateCase(patientCase);
                    await DBService.DeleteCaseResponses(patientCase.CaseUID);
                }



                //save responses for case
                foreach (QuestionFormElement element in QuestionElements)
                {

                    APIQuestionResponse response = new APIQuestionResponse { CaseUID = patientCase.CaseUID, SubmissionIdT = patientCase.SubmissionIdT };

                    //parent question

                    string value = Utility.GetElementValues(element.Question.ResponseType, element.QuestionControl, element.Question.QuestionText);
                    if (value == null)
                    {
                        continue;
                    }
                    response.QuestionId = element.Question.QuestionId;
                    response.ResponseText = value;
                    await DBService.AddResponses(response);


                }
                //update submission case count
                await DBService.UpdateSubmissionCaseCount(ViewModel.Submission.SubmissionIdT);

                //update ahievement details
                await UpdateAchievements();

                await Utility.DisplayInfoMessage(string.Format("Saved case details for {0}", patientCase.Patient));


            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async Task UpdateAchievements()
        {
            //delete existing achievements
            await DBService.DeleteAllSubmissionAchievements(ViewModel.Submission.SubmissionIdT);
            //first get all comptencies
            List<APIElogbookCompetence> competences = await DBService.GetComptencies(ViewModel.Submission.AssignmentId);
            //then get all responses for submission
            List<APIQuestionResponse> responses = await DBService.GetSubmissionResponses(ViewModel.Submission.SubmissionIdT);
            //add assignment responses to list
            List<APIAssignmentQuestionResponse> assignmentResponses = await DBService.GetAssignmentResonses(ViewModel.Submission.SubmissionIdT);
            foreach (APIAssignmentQuestionResponse r in assignmentResponses)
            {
                responses.Add(new APIQuestionResponse
                {
                    QuestionId = r.QuestionId,
                    ResponseText = r.ResponseText,
                    SubmissionIdT = ViewModel.Submission.SubmissionIdT

                });
            }

            //fr each expected competence, find how much has been acheived
            foreach (APIElogbookCompetence ec in competences)
            {
                //find at least one response that contains this competence (question option)

                //in case multiple options, interepreted as OR. e.g., Lumbar puncture or Interpretation of results
                string[] options = ec.QuestionOption.Split(';');
                APIQuestionResponse response = null;
                APIQuestion question = ViewModel.Questions.Where(q => q.QuestionId == ec.QuestionId).FirstOrDefault();
                foreach (string option in options)
                {
                    response = responses.Where(a => a.QuestionId == ec.QuestionId && a.ResponseText.Contains(option.Trim())).FirstOrDefault();
                    if (response != null)
                    {
                        break;
                    }
                }
                int achieved = 0;

                if (response != null)
                {
                    //get respnses that contain the target questionoption

                    foreach (string option in options)
                    {
                        achieved += responses.Where(a => a.QuestionId == ec.QuestionId && a.ResponseText.Contains(option.Trim())).Count();
                    }

                }
                else
                {
                    //if no response matches competence then achiveed is 0
                    achieved = 0;
                }
                ElogbookAchievement ea = new ElogbookAchievement
                {
                    ElogbookId = ec.ElogbookId,
                    AssignmentId = ViewModel.Submission.AssignmentId,
                    SubmissionId = ViewModel.Submission.SubmissionId,
                    SubmissionIdT = ViewModel.Submission.SubmissionIdT,
                    QuestionId = ec.QuestionId,
                    QuestionOption = ec.QuestionOption,
                    QuestionText = ec.QuestionText,
                    StudentId = ViewModel.Submission.StudentId,
                    Achieved = achieved,
                    Expected = ec.Expected,
                    AchievedPercentage = ec.Expected == 0 ? 100 : Math.Round(((double)achieved / (double)ec.Expected) * 100)
                };
                await DBService.AddElogbookAchievement(ea);
            }
        }

        async void LoadCaseData()
        {
            try
            {
                APICase patient = ViewModel.SelectedCase;
                if (patient == null)
                {
                    return;
                }
                //Get Question Responses
                List<APIQuestionResponse> responses = await DBService.GetCaseResponses(patient.SubmissionIdT, patient.CaseUID);
                if (responses != null && responses.Count > 0)
                {
                    foreach (APIQuestionResponse response in responses)
                    {
                        try
                        {
                            //if childQuestionId <=0 then it's a parent question else its a child
                            QuestionFormElement formElement = null;

                            //parent
                            formElement = QuestionElements.Where(c => c.Question.QuestionId == response.QuestionId).FirstOrDefault();
                            Utility.SetElementValues(formElement.Question.ResponseType, response.ResponseText, formElement.QuestionControl);



                        }
                        catch (Exception ex)
                        {
                            //do nothing for responses that fail to load
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }
    }
}