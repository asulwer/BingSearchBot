using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using BingSearchBot.DAO;
using BingSearchBot.Model;

namespace BingSearchBot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private SettingsModel model;
        private Guid? Id;

        public Settings()
        {
            this.InitializeComponent();
            
            cbMode.ItemsSource = Enum.GetValues(typeof(Mode));
            model = new SettingsModel();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                Id = Guid.Parse(e.Parameter.ToString());
                using (SettingsDAO dao = new SettingsDAO())
                {
                    model = await dao.Read(Id.Value);
                }
            }

            this.DataContext = model;
        }
        private async void abbSave_Click(object sender, RoutedEventArgs e)
        {
            if (Id.HasValue)
            {
                using (SettingsDAO dao = new SettingsDAO())
                {
                    await dao.Update(model);
                }
            }
            else
            {
                using (SettingsDAO dao = new SettingsDAO())
                {
                    await dao.Insert(model);
                }                
            }

            Frame.Navigate(typeof(MainPage));
        }
        private void abbCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        private async void btnWords_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fp = new FileOpenPicker();

            fp.FileTypeFilter.Add(".txt");

            StorageFile sf = await fp.PickSingleFileAsync();

            await sf.CopyAsync(ApplicationData.Current.LocalFolder, sf.Name, NameCollisionOption.ReplaceExisting);

            model.FilePath = sf.Name;
        }
    }
}
