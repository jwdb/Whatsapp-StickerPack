using System;
using System.Collections.Generic;
using System.Linq;

namespace nl.datm.WhaStickerProvider.lib.Models
{
    public class StickerPack
    {
        public string identifier { get; set; }
        public string name { get; set; }
        public string publisher { get; set; }
        public string trayImageFile { get; set; }
        public string publisherEmail { get; set; }
        public string publisherWebsite { get; set; }
        public string privacyPolicyWebsite { get; set; }
        public string licenseAgreementWebsite { get; set; }
        public string imageDataVersion { get; set; }
        public string androidPlayStoreLink { get; set; }
        public string iosAppStoreLink { get; set; }
        public bool avoidCache { get; set; }
        public byte[] trayImage { get; set; }
        public List<Sticker> Stickers { get; set; }

        public StickerPack(StickerContentJson.Sticker_Packs jsonPack, byte[] trayImage, List<Sticker> resolveSticker)
        {
            identifier = jsonPack.identifier;
            name = jsonPack.name;
            publisher = jsonPack.publisher;
            trayImageFile = jsonPack.tray_image_file;
            publisherEmail = jsonPack.publisher_email;
            publisherWebsite = jsonPack.publisher_website;
            privacyPolicyWebsite = jsonPack.privacy_policy_website;
            licenseAgreementWebsite = jsonPack.license_agreement_website;
            imageDataVersion = jsonPack.image_data_version;
            avoidCache = jsonPack.avoid_cache;

            Stickers = resolveSticker;
            this.trayImage = trayImage;
        }
    }
}