﻿using DevExpress.XamarinForms.Editors;
using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using elogbookmobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace elogbookmobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StaffNewAssignmentQuestionsPage : ContentPage
	{
		public StaffNewAssignmentQuestionsPage ()
		{
			InitializeComponent ();
            QuestionElements = new List<QuestionFormElement>();
            BindingContext = ViewModel = new StaffNewAssignmentQuestionsViewModel();

        }
        public StaffNewAssignmentQuestionsViewModel ViewModel { get; }
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
                   // secSL.Children.Add(new Label());

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
                
                LoadCaseData();
                btnSaveData.IsVisible = true;
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);

            }
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

                //check whether intials already exist for this submission


                await DBService.DeleteAllAssignmentResponses(ViewModel.SubmissionIdT);




                //save responses for assignment
                foreach (QuestionFormElement element in QuestionElements)
                {
                    APIAssignmentQuestionResponse response = new APIAssignmentQuestionResponse { SubmissionIdT = ViewModel.SubmissionIdT };
                    
                        //parent question
                        string value = Utility.GetElementValues(element.Question.ResponseType, element.QuestionControl, element.Question.QuestionText);
                        if (value == null)
                        {
                            continue;
                        }
                        response.QuestionId = element.Question.QuestionId;                        
                        response.ResponseText = value;
                        await DBService.AddAssignmentResponses(response);
                    

                }
                await Utility.DisplayInfoMessage("Saved responses to general questions");


            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        async void LoadCaseData()
        {
            try
            {
                APIStaffSubmission submission = ViewModel.SelectedSubmission;
                if (submission == null)
                {
                    return;
                }
                //Get Question Responses
                List<APIAssignmentQuestionResponse> responses = await DBService.GetAssignmentResonses(submission.SubmissionIdT);
                if (responses != null && responses.Count > 0)
                {
                    foreach (APIAssignmentQuestionResponse response in responses)
                    {
                        try
                        {
                            //if childQuestionId <=0 then it's a parent question else its a child
                            QuestionFormElement formElement = QuestionElements.Where(c => c.Question.QuestionId == response.QuestionId).FirstOrDefault();
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