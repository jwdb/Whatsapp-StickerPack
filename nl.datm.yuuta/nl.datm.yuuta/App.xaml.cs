using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using nl.datm.yuuta.Services;
using nl.datm.yuuta.Views;
using System.IO;
using Newtonsoft.Json;
using nl.datm.yuuta.Models;
using System.Collections.Generic;
using System.Reflection;

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

            var stickers = new Dictionary<(string identifier, string name, string trayImageFile, byte[] trayImage), Dictionary<(string name, string[] emoji), Func<byte[]>>>();

            foreach (var stickerPack in sContent.sticker_packs)
            {

                byte[] trayImage = null;
                using (Stream traystream = assembly.GetManifestResourceStream($"{assetString}._{stickerPack.identifier}.{stickerPack.tray_image_file}")) { 
                    trayImage = new byte[traystream.Length];
                    traystream.Read(trayImage, 0, trayImage.Length);
                }

                var stickerSource = new Dictionary<(string name, string[] emoji), Func<byte[]>>();

                foreach (var item in stickerPack.stickers)
                {
                    stickerSource.Add((item.image_file, item.emojis), () => {
                        byte[] stickImage = null;
                        using (Stream stickstream = assembly.GetManifestResourceStream($"{assetString}._{stickerPack.identifier}.{item.image_file}")) {
                            stickImage = new byte[stickstream.Length];
                            stickstream.Read(stickImage, 0, stickImage.Length);
                        }

                        return stickImage;
                    });
                }

                stickers.Add((stickerPack.identifier, stickerPack.name, stickerPack.tray_image_file, trayImage), stickerSource);
            }

            DependencyService.Get<AddStickerService>().SetStickerResolver(stickers);

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
