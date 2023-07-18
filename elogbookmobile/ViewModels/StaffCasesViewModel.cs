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
   public class StaffCasesViewModel : BaseViewModel
    {
        APICase _selectedItem;


        public StaffCasesViewModel()
        {
            //submissions
            
            Title = "Cases";
            Items = new ObservableRangeCollection<APICase>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<APICase>(OnItemSelected);
            
        }


        public ObservableRangeCollection<APICase> Items { get; }

        
        public Command LoadItemsCommand { get; }
        

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

        APIStaffSubmission selectedSubmission;
        public APIStaffSubmission SelectedSubmission { get => selectedSubmission; set => SetProperty(ref selectedSubmission, value); }
      

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
                if (SelectedSubmission == null)
                {
                    return;
                }
                Items.Clear();
                var items = await DBService.GetSubmissionCases(SelectedSubmission.SubmissionId);
                Items.AddRange(items);
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

        

        async void OnItemSelected(APICase item)
        {
            if (item == null)
                return;

            item.SubmissionIdT = SelectedSubmission.SubmissionIdT;
            item.AssignmentId = SelectedSubmission.AssignmentId;
            await Navigation.NavigateToAsync<StaffNewCaseViewModel>(item);
        }

        public async Task LoadItemId(APIStaffSubmission item)
        {
            try
            {
                
                SelectedSubmission = item;
                await ExecuteLoadItemsCommand();
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
    }
}