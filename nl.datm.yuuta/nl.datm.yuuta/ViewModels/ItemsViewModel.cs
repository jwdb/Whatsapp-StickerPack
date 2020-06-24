using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using nl.datm.yuuta.Models;
using nl.datm.yuuta.Views;
using nl.datm.WhaStickerProvider.lib;

namespace nl.datm.yuuta.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public Item SelectedItem { get; set; }

        public Command SelectChangedCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SelectChangedCommand = new Command(() =>
            {
                if (SelectedItem != null)
                    Provider.AddStickerPackToWhatsApp(SelectedItem.Id, SelectedItem.Text);

                SelectedItem = null;
                this.OnPropertyChanged(nameof(SelectedItem));
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = Provider.GetStickerPacks();
                foreach (var item in items)
                {
                    Items.Add(new Item()
                    {
                        Id = item.identifier,
                        Text = item.name,
                        Description = ""
                    });
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
    }
}