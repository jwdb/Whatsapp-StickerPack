using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace nl.datm.yuuta.Droid
{
    internal class StickerPack : Java.Lang.Object, IParcelable, IParcelableCreator
    {
        internal string identifier;
        internal string name;
        internal string publisher;
        internal string trayImageFile;
        internal string publisherEmail;
        internal string publisherWebsite;
        internal string privacyPolicyWebsite;
        internal string licenseAgreementWebsite;
        internal string imageDataVersion;
        internal bool avoidCache;

        internal String iosAppStoreLink;
        private List<Sticker> stickers;
        private long totalSize;
        internal String androidPlayStoreLink;
        private bool isWhitelisted;

        internal byte[] trayImage;

        private Dictionary<(string name, string[] emoji), Func<byte[]>> _resolveSticker;

        internal StickerPack(String identifier, String name, String publisher, String trayImageFile, String publisherEmail, String publisherWebsite, String privacyPolicyWebsite, String licenseAgreementWebsite, String imageDataVersion, bool avoidCache)
        {
            this.identifier = identifier;
            this.name = name;
            this.publisher = publisher;
            this.trayImageFile = trayImageFile;
            this.publisherEmail = publisherEmail;
            this.publisherWebsite = publisherWebsite;
            this.privacyPolicyWebsite = privacyPolicyWebsite;
            this.licenseAgreementWebsite = licenseAgreementWebsite;
            this.imageDataVersion = imageDataVersion;
            this.avoidCache = avoidCache;
        }

        internal StickerPack(string identifier, string name, string trayImageFile, byte[] trayImage, Dictionary<(string name, string[] emoji), Func<byte[]>> resolveSticker)
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
            _resolveSticker = resolveSticker;
            this.trayImage = trayImage;
        }

        void setIsWhitelisted(bool isWhitelisted)
        {
            this.isWhitelisted = isWhitelisted;
        }

        bool getIsWhitelisted()
        {
            return isWhitelisted;
        }

        private StickerPack(Parcel inparcel)
        {
            identifier = inparcel.ReadString();
            name = inparcel.ReadString();
            publisher = inparcel.ReadString();
            trayImageFile = inparcel.ReadString();
            publisherEmail = inparcel.ReadString();
            publisherWebsite = inparcel.ReadString();
            privacyPolicyWebsite = inparcel.ReadString();
            licenseAgreementWebsite = inparcel.ReadString();
            iosAppStoreLink = inparcel.ReadString();
            stickers = getStickers();
            totalSize = inparcel.ReadLong();
            androidPlayStoreLink = inparcel.ReadString();
            isWhitelisted = inparcel.ReadByte() != 0;
            imageDataVersion = inparcel.ReadString();
            avoidCache = inparcel.ReadByte() != 0;
        }

        internal void setStickers(List<Sticker> stickers)
        {
            if (_resolveSticker != null)
                return;

            this.stickers = stickers;
            totalSize = 0;
            foreach (Sticker sticker in stickers)
            {
                totalSize += sticker.size;
            }
        }

        internal void setAndroidPlayStoreLink(String androidPlayStoreLink)
        {
            this.androidPlayStoreLink = androidPlayStoreLink;
        }

        internal void setIosAppStoreLink(String iosAppStoreLink)
        {
            this.iosAppStoreLink = iosAppStoreLink;
        }

        internal List<Sticker> getStickers()
        {
            if (_resolveSticker != null)
            {
                return _resolveSticker.Select(item => new Sticker(item.Key.name, item.Key.emoji.ToList(), item.Value)).ToList();
            }

            return stickers;
        }

        public Java.Lang.Object CreateFromParcel(Parcel inparcel)
        {
            return new StickerPack(inparcel);
        }

        public Java.Lang.Object[] NewArray(int size)
        {
            return new StickerPack[size];
        }

        long getTotalSize()
        {
            return totalSize;
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteString(identifier);
            dest.WriteString(name);
            dest.WriteString(publisher);
            dest.WriteString(trayImageFile);
            dest.WriteString(publisherEmail);
            dest.WriteString(publisherWebsite);
            dest.WriteString(privacyPolicyWebsite);
            dest.WriteString(licenseAgreementWebsite);
            dest.WriteString(iosAppStoreLink);
            dest.WriteTypedList(stickers);
            dest.WriteLong(totalSize);
            dest.WriteString(androidPlayStoreLink);
            dest.WriteByte((sbyte)(isWhitelisted ? 1 : 0));
            dest.WriteString(imageDataVersion);
            dest.WriteByte((sbyte)(avoidCache ? 1 : 0));
        }
    }
}