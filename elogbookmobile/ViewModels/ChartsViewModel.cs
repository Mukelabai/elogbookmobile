using elogbookmobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace elogbookmobile.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        public ChartsViewModel()
        {
            Title = "ChartsView";
            Items = new ObservableCollection<Item>();
        }

        public ObservableCollection<Item> Items { get; private set; }

        async public void OnAppearing()
        {
            IEnumerable<Item> items = await DataStore.GetItemsAsync(true);
            Items.Clear();
            foreach (Item item in items)
            {
                Items.Add(item);
            }
        }
    }
}
