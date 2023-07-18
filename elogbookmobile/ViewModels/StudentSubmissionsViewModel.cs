using elogbookapi.Models.API;
using elogbookmobile.Services;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
    public class StudentSubmissionsViewModel:BaseViewModel
    {
        APISubmission _selectedItem;


        public StudentSubmissionsViewModel()
        {
            Title = "Submissions";
            StudentSubmissions = new ObservableCollection<APISubmission>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<APISubmission>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            EditSubmissionCommand = new Command<APISubmission>(OnEditItem);
            DeleteSubmissionCommand = new Command<APISubmission>(OnDeleteItem);
        }


        public ObservableCollection<APISubmission> StudentSubmissions { get; }

        
        public Command LoadItemsCommand { get; }

        public Command AddItemCommand { get; }
        public Command EditSubmissionCommand { get; }
        public Command DeleteSubmissionCommand { get; }
        public Command<APISubmission> ItemTapped { get; }

        public APISubmission SelectedItem
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
                StudentSubmissions.Clear();
                var items = await DBService.GetSubmissions(); //await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    StudentSubmissions.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void OnAddItem(object obj)
        {
            //check if there any assignments to be aswered, otehrwise tell the student
            List<APIAssignment> assignments = await DBService.GetAssignmentsWithoutSubmission();
            if (assignments.Count <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Information", "You have already created submissions for all available assignments. Please logout and logback in to check if there any new assignments for you.", "Ok");
            }
            else
            {
                await Navigation.NavigateToAsync<NewSubmissionViewModel>(null);
            }
        }
        async void OnEditItem(APISubmission item)
        {
            await Navigation.NavigateToAsync<NewSubmissionViewModel>(item);
        }
        async void OnDeleteItem(APISubmission item)
        {
            var accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", "To remove this submission from the server, login to the web portal. Do you want to remove it from your device?", "Yes", "No");
            if (accept)
            {
                await DBService.DeleteSubmission(item.SubmissionIdT);
                await ExecuteLoadItemsCommand();
            }
        }

        async void OnItemSelected(APISubmission item)
        {
            if (item == null)
                return;
            await Navigation.NavigateToAsync<SubmissionDetailViewModel>(item.SubmissionIdT);
        }
    }
}
