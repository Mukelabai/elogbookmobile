using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
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
    public partial class NewCompetenciesPage : ContentPage
    {
        public NewCompetenciesPage()
        {
            InitializeComponent();
            QuestionElements = new List<QuestionFormElement>();
            BindingContext = ViewModel = new NewCompetenciesViewModel();

        }
        public NewCompetenciesViewModel ViewModel { get; }
        public List<QuestionFormElement> QuestionElements { get; set; }

        private void btnLoadPatient_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnLoadComptencies_Clicked(object sender, EventArgs e)
        {
            try
            {
                Panel.Children.Clear();
                AddControls();
                btnLoadComptencies.IsEnabled = false;

                //LoadCaseData();
            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);

            }
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
            List<ElogbookAchievement> allQuestions = ViewModel.Questions;
            allQuestions = allQuestions.Where(q => q.Expected > 0).ToList();
            if (allQuestions.Count <= 0)
            {
                return;
            }

            int[] questionIds = (from q in allQuestions select q.QuestionId).Distinct().ToArray();

           

            foreach (int questionId in questionIds)
            {
                //get options for question
                List<ElogbookAchievement> questionOptions = allQuestions.Where(q => q.QuestionId == questionId).ToList();
                ElogbookAchievement question = questionOptions[0];
                StackLayout secSL = Panel;// new StackLayout();
                Label secLabel = GetSectionLabel(question.QuestionText);
                secSL.Children.Add(secLabel);

                //now display question options
                foreach(ElogbookAchievement option in questionOptions)
                {
                    Label qLabel = new Label();
                    double a = option.Achieved;
                    double ex = option.Expected;
                    double perc = a / ex;
                    qLabel.Text = string.Format("{0}: {1}% ({2}/{3})", option.QuestionOption, Math.Round(perc * 100.0), option.Achieved, option.Expected);
                    qLabel.FontFamily = "Roboto";
                    qLabel.FontSize = 14;
                    secSL.Children.Add(qLabel);
                    ProgressBar qBar = new ProgressBar();
                    qBar.Progress = (double)option.Achieved / (double)option.Expected;
                    qBar.ProgressColor = qBar.Progress >= 1 ? Color.Green : qBar.Progress >= 0.5 ? Color.Orange : qBar.ProgressColor;
                    secSL.Children.Add(qBar);
                }
               

            }


        }
    }
}