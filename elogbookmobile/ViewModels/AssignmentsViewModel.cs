using elogbookapi.Models.API;
using elogbookmobile.Helpers;
using elogbookmobile.Models.API;
using elogbookmobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace elogbookmobile.ViewModels
{
   public class AssignmentsViewModel : BaseViewModel
    {
        APIAssignment _selectedItem;


        public AssignmentsViewModel()
        {
            Title = "Assignments";
            AssignmentYears = new ObservableCollection<APIYear>();
            Assignments = new ObservableCollection<APIAssignment>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<APIAssignment>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            EditSubmissionCommand = new Command<APIAssignment>(OnEditItem);
        }

        APIYear selectedYear;
        public APIYear SelectedYear { get => selectedYear; set { SetProperty(ref this.selectedYear, value); } }
        public ObservableCollection<APIYear> AssignmentYears { get; }
        public ObservableCollection<APIAssignment> Assignments { get; }


        public Command LoadItemsCommand { get; }

        public Command AddItemCommand { get; }
        public Command EditSubmissionCommand { get; }

        public Command<APIAssignment> ItemTapped { get; }

        public APIAssignment SelectedItem
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
            //SelectedYear = null;
            //if (SelectedYear == null)
            //{
            //    await LoadYears();
            //}
            await ExecuteLoadItemsCommand();
        }
        async Task LoadYears()
        {
            IsBusy = true;

            try
            {
                AssignmentYears.Clear();
                var items = await DBService.GetAssignments(); //await DataStore.GetItemsAsync(true);
                int[] years = (from y in items
                               select y.AcademicYear).Distinct().ToArray();
                foreach(int year in years)
                {
                    AssignmentYears.Add(new APIYear { Year = year });
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
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                //if (SelectedYear == null)
                //{
                //    return;
                //}
                Assignments.Clear();
                var items = await DBService.GetAssignments(); //await DataStore.GetItemsAsync(true);
                //get maximum year
                int maxYear = (from y in items select y.AcademicYear).Max();
                List < APIAssignment > yearItems = items.Where(a => a.AcademicYear == maxYear).ToList();
                foreach (var item in yearItems)
                {
                    Assignments.Add(item);
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
        async void OnEditItem(APIAssignment item)
        {
            await Navigation.NavigateToAsync<NewSubmissionViewModel>(item);
        }

        async void OnItemSelected(APIAssignment item)
        {
            if (item == null)
                return;
            await Navigation.NavigateToAsync<StaffSubmissionsViewModel>(item);
        }
    }
}

