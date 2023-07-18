using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models;
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
    class CasesViewModel : BaseViewModel
    {
        APICase _selectedItem;


        public CasesViewModel()
        {
            //submissions
            Submissions = new ObservableRangeCollection<APISubmission>();
            Title = "Cases";
            Items = new ObservableRangeCollection<APICase>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<APICase>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            DeleteCaseCommand = new Command<APICase>(DeleteCase);
        }


        public ObservableRangeCollection<APICase> Items { get; }

        public Command DeleteCaseCommand { get; }
        
        public Command LoadItemsCommand { get; }
         async void DeleteCase(APICase patientCase)
        {
            var accept = await Application.Current.MainPage.DisplayAlert("Confirm Action", string.Format("Do you want to delete patient {0}?", patientCase.Patient), "Yes", "No");
            if (!accept)
            {
                return;
            }
            //delete case then delete responses for case
            await DBService.DeleteCase(patientCase);
            await DBService.DeleteCaseResponses(patientCase.CaseUID);
            await ExecuteLoadItemsCommand();

        }

        public Command AddItemCommand { get; }

        public Command<APICase> ItemTapped { get; }

        public APICase SelectedItem
        {
            get => this._selectedItem;
            set
            {
                SetProperty(ref this._selectedItem, value);
                OnItemSelected(value);
            }
        }

        APISubmission selectedSubmission;
        public APISubmission SelectedSubmission { get=>selectedSubmission; set=> SetProperty(ref selectedSubmission,value); }
        public ObservableRangeCollection<APISubmission> Submissions { get; }

        public async void OnAppearing()
        {
            SelectedItem = null;
            //load submissions
            Submissions.Clear();
            Submissions.AddRange(await DBService.GetSubmissions());
            await ExecuteLoadItemsCommand();
            
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (SelectedSubmission == null)
                {
                    return;
                }
                Items.Clear();
                var items = await DBService.GetSubmissionCases(SelectedSubmission.SubmissionIdT);
                Items.AddRange(items);
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
            await Navigation.NavigateToAsync<NewCaseViewModel>(SelectedSubmission);
        }

        async void OnItemSelected(APICase item)
        {
            if (item == null)
                return;
            await Navigation.NavigateToAsync<NewCaseViewModel>(item);
        }

        public async Task LoadItemId(Guid itemId)
        {
            try
            {
                var item = await DBService.GetSubmission(itemId);
                SelectedSubmission = item;
                await ExecuteLoadItemsCommand();
            }
            catch (Exception ex)
            {
              await  Utility.DisplayErrorMessage(ex);
                Debug.WriteLine("Failed to Load Item");
            }
        }

        public override async Task InitializeAsync(object parameter)
        {
            await LoadItemId(Guid.Parse(parameter.ToString()));
        }
    }
}