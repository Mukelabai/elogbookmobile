using DevExpress.XamarinForms.DataForm;
using elogbookapi.Models;
using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class NewSubmissionViewModel:BaseViewModel
    {
        public const string ViewName = "NewSubmissionPage";


       

        public  NewSubmissionViewModel()
        {
            //get mentors
            Mentors = new ObservableRangeCollection<APIMentor>();
            Hospitals = new ObservableRangeCollection<APIHospital>();
            Assignments = new ObservableRangeCollection<APIAssignment>();
           
          
            
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        
        
        public ObservableRangeCollection<APIMentor> Mentors { get; set; }

        
        public ObservableRangeCollection<APIHospital> Hospitals { get; set; }
        
        public ObservableRangeCollection<APIAssignment> Assignments { get; set; }

        APIMentor selectedMentor;
        public APIMentor SelectedMentor { get=> this.selectedMentor; set=>SetProperty(ref selectedMentor,value); }

        APIAssignment selectedAssignment;
        public APIAssignment SelectedAssignment { get => this.selectedAssignment; set => SetProperty(ref selectedAssignment, value); }
        APIHospital selectedHospital;
        public APIHospital SelectedHospital { get => this.selectedHospital; set => SetProperty(ref selectedHospital, value); }




        public Command SaveCommand { get; }

        
        public Command CancelCommand { get; }


        bool ValidateSave()
        {
            return selectedMentor != null && selectedHospital != null && selectedAssignment != null;
                //&& !String.IsNullOrWhiteSpace(this.description);
        }

        async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }

        async void OnSave()
        {
            User user = await DBService.GetApplicationUser();
            if (SelectedSubmission != null)
            {
                SelectedSubmission.SubmissionIdT = SelectedSubmission.SubmissionIdT;
                SelectedSubmission.AssignmentId = SelectedAssignment.AssignmentId;
                SelectedSubmission.HospitalId = SelectedHospital.HospitalId;
                SelectedSubmission.MentorId = SelectedMentor.MentorId;
                SelectedSubmission.StudentId = user.UserId;
                SelectedSubmission.CreatedBy = user.WebUsername;
                SelectedSubmission.CreatedOn = SelectedSubmission.CreatedOn;
                SelectedSubmission.UpdatedBy = SelectedSubmission.UpdatedBy;
                SelectedSubmission.UpdatedOn = DateTime.Now;
                SelectedSubmission.Rotation = SelectedAssignment.AssignmentName;
                SelectedSubmission.Hospital = SelectedHospital.HospitalName;
                SelectedSubmission.Mentor = SelectedMentor.MentorName;
                SelectedSubmission.Cases = SelectedSubmission.Cases;
                SelectedSubmission.Status = SelectedSubmission.Status;
                SelectedSubmission.Grade = SelectedSubmission.Grade;
                SelectedSubmission.IsPublished = SelectedSubmission.IsPublished;
                await DBService.AddSubmission(SelectedSubmission);
            }
            else
            {
                APISubmission newItem = new APISubmission()
                {
                    SubmissionIdT = Guid.NewGuid(),
                    StudentId = user.UserId,
                    MentorId = SelectedMentor.MentorId,
                    HospitalId = SelectedHospital.HospitalId,
                    AssignmentId = SelectedAssignment.AssignmentId,
                    CreatedBy = user.WebUsername,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = user.WebUsername,
                    UpdatedOn = DateTime.Now,
                    Rotation = SelectedAssignment.AssignmentName,
                    Hospital = SelectedHospital.HospitalName,
                    Mentor = SelectedMentor.MentorName,
                    Cases = 0,
                    Status = "Pending Approval",
                    Grade = "None",
                    IsPublished = false



                };

                await DBService.AddSubmission(newItem);
            }

            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }

        public APISubmission SelectedSubmission { get; set; }
        public async Task LoadItemId(APISubmission submission)
        {
            try
            {
                
                SelectedSubmission = submission;


                //mentors
                Mentors.Clear();
                Mentors.AddRange(await DBService.GetMentors());
                //Hospitals
                Hospitals.Clear();
                var hospitals = await DBService.GetHospitals();
                Hospitals.AddRange(hospitals);
                //Assignments
                Assignments.Clear();
                if (SelectedSubmission == null)
                {
                    Assignments.AddRange(await DBService.GetAssignmentsWithoutSubmission());
                }
                else
                {
                    var asses = await DBService.GetAssignments();
                    Assignments.AddRange(asses);//since editing, load all asignments
                    SelectedAssignment = await DBService.GetAssignment(SelectedSubmission.AssignmentId);
                    SelectedMentor = await DBService.GetMentor(SelectedSubmission.MentorId);
                    SelectedHospital = await DBService.GetHospital(SelectedSubmission.HospitalId);
                }
                ///get selected assignment, mentor and hospital
                ///
                


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
