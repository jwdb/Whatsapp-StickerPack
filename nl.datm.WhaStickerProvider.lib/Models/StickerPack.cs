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

        public StickerPack(string identifier, string name, string trayImageFile, byte[] trayImage, List<Sticker> resolveSticker)
        {
            this.identifier = identifier;
            this.name = name;
            this.publisher = "Jan-Willem de Bruyn";
            this.trayImageFile = trayImageFile;
            this.publisherEmail = "a@b.nl";
            this.publisherWebsite = "datm.nl";
            this.privacyPolicyWebsite = "datm.nl";
            this.licenseAgreementWebsite = "datm.nl";
            this.imageDataVersion = "1";
            this.avoidCache = false;
            Stickers = resolveSticker;
            this.trayImage = trayImage;
        }
    }
}