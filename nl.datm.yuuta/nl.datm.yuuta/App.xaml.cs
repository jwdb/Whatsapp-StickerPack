using Newtonsoft.Json;
using nl.datm.WhaStickerProvider.lib.Models;
using nl.datm.yuuta.Services;
using nl.datm.yuuta.Views;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace nl.datm.yuuta
{
    public partial class App : Application
    {

        public App()
        {
            var assetString = "nl.datm.yuuta.Assets";
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream($"{assetString}.contents.json");
            string text = "";
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            var sContent = JsonConvert.DeserializeObject<StickerContentJson>(text);

            WhaStickerProvider.lib.Provider.Init(assembly, sContent, assetString);

            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
