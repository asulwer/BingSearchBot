using System;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Storage;

using BingSearchBot.DAO;
using BingSearchBot.Model;

namespace BingSearchBot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<SettingsModel> ModelList;
        private Task TaskSearch;
        private CancellationTokenSource TokenSource;

        public MainPage()
        {
            this.InitializeComponent();
            ModelList = new ObservableCollection<SettingsModel>();
            
            wbSearch.Navigate(new Uri("https://login.live.com"));
        }

        #region TaskSearch
        private void Search(SettingsModel model, IProgress<HttpRequestMessage> progress)
        {
            CancellationToken ct = TokenSource.Token;

            ct.ThrowIfCancellationRequested();

            int iInterval = model.Count; //interval is reset everytime we cancel and restart

            while (!ct.IsCancellationRequested && iInterval > 0)
            {
                int delay = CreateRandomNumber(model.Min, model.Max);

                string query = CreateRandomSentence(CreateRandomNumber(3, 6), model.FilePath).Result;

                if (!string.IsNullOrEmpty(query))
                {
                    Uri uri = new Uri("http://bing.com/search?q=" + query);

                    HttpRequestMessage hrm = new HttpRequestMessage(HttpMethod.Get, uri);

                    if (model.Mode == Mode.Mobile)
                        hrm.Headers.Add("User-Agent", "Mozilla/5.0 (iPhone; U; CPU iPhone OS 5_1_1 like Mac OS X; en) AppleWebKit/534.46.0 (KHTML, like Gecko) CriOS/19.0.1084.60 Mobile/9B206 Safari/7534.48.3");

                    progress.Report(hrm);

                    iInterval -= 1;
                    Task.Delay(delay).Wait();
                }
            }            
        }
        private int CreateRandomNumber(int min, int max)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            return random.Next(min, max);
        }
        private async Task<string> CreateRandomSentence(int wordcount, string file)
        {
            string sentence = null;

            Random rnd = new Random((int)DateTime.Now.Ticks);

            IStorageItem sf = await ApplicationData.Current.LocalFolder.TryGetItemAsync(file);

            if (sf != null)
            {
                string[] words = System.IO.File.ReadAllLines(sf.Path);

                StringBuilder builder = new StringBuilder();

                //select random word from array
                for (int i = 0; i < wordcount; i++)
                    builder.Append(words[rnd.Next(words.Length)]).Append(" ");

                sentence = builder.ToString().Trim();

                //capitalize first word
                if (wordcount >= 4)
                    sentence = char.ToUpper(sentence[0]) + sentence.Substring(1) + ".";
            }

            return sentence;
        }        
        
        #endregion
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            using (SettingsDAO dao = new SettingsDAO())
            {
                ModelList = await dao.ReadAll();
            }

            cbList.ItemsSource = ModelList.OrderByDescending(i => i.Id).ToList();

            if(cbList.Items.Count > 0)
                cbList.SelectedIndex = 0;
        }        
        private void abbStart_Click(object sender, RoutedEventArgs e)
        {
            if (cbList.SelectedIndex != -1)
            {
                if(TaskSearch != null)
                    if (TaskSearch.Status == TaskStatus.Running)
                        TaskSearch.Wait();

                SettingsModel model = cbList.SelectedItem as SettingsModel;

                int iInterval = model.Count; //interval is reset everytime we cancel and restart

                abbStart.IsEnabled = false;
                
                var p = new Progress<HttpRequestMessage>();
                p.ProgressChanged += (senderOfProgressedChanged, nextItem) =>
                {
                    wbSearch.NavigateWithHttpRequestMessage(nextItem);
                    
                    //countdown
                    iInterval -= 1;
                    tbCount.Text = $"{iInterval}";
                };

                TokenSource = new CancellationTokenSource();
                TaskSearch = Task.Factory.StartNew(() =>
                {
                    Search(model, p);
                }, TokenSource.Token).ContinueWith(t =>
                {
                    abbStart.IsEnabled = true;
                    abbStop.IsEnabled = false;                    
                }, TaskScheduler.FromCurrentSynchronizationContext());

                abbStop.IsEnabled = true;
            }
        }
        private void abbStop_Click(object sender, RoutedEventArgs e)
        {
            //stop searching
            TokenSource.Cancel();
            TokenSource.Dispose();
        }        
        private void cbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbList.SelectedIndex != -1)
            {
                SettingsModel model = cbList.SelectedItem as SettingsModel;

                tbMode.Text = Enum.GetName(typeof(Mode), model.Mode);
                tbCount.Text = $"{model.Count}";
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (cbList.SelectedIndex != -1)
            {
                //edit settings
                SettingsModel listitem = cbList.SelectedItem as SettingsModel;
                Frame.Navigate(typeof(Settings), listitem.Id);
            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }
    }
}
