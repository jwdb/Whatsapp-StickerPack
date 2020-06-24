namespace nl.datm.WhaStickerProvider.lib.Models
{

    public class StickerContentJson
    {
        public string android_play_store_link { get; set; }
        public string ios_app_store_link { get; set; }
        public Sticker_Packs[] sticker_packs { get; set; }

        public class Sticker_Packs
        {
            public string identifier { get; set; }
            public string name { get; set; }
            public string publisher { get; set; }
            public string tray_image_file { get; set; }
            public string image_data_version { get; set; }
            public bool avoid_cache { get; set; }
            public string publisher_email { get; set; }
            public string publisher_website { get; set; }
            public string privacy_policy_website { get; set; }
            public string license_agreement_website { get; set; }
            public Sticker[] stickers { get; set; }
        }

        public class Sticker
        {
            public string image_file { get; set; }
            public string[] emojis { get; set; }
        }
    }
}
