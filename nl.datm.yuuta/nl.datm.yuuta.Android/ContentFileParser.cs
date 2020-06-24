using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace nl.datm.yuuta.Droid
{
    internal class ContentFileParser
    {
        internal static List<StickerPack> parseStickerPacks(System.IO.Stream contentsInputStream)
        {
            try
            {
                JsonReader reader = new JsonReader(new InputStreamReader(contentsInputStream));
                return readStickerPacks(reader);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static List<StickerPack> readStickerPacks(JsonReader reader)
        {
            List<StickerPack> stickerPackList = new List<StickerPack>();
            String androidPlayStoreLink = null;
            String iosAppStoreLink = null;
            reader.BeginObject();
            while (reader.HasNext)
            {
                String key = reader.NextName();
                if ("android_play_store_link".Equals(key))
                {
                    androidPlayStoreLink = reader.NextString();
                }
                else if ("ios_app_store_link".Equals(key))
                {
                    iosAppStoreLink = reader.NextString();
                }
                else if ("sticker_packs".Equals(key))
                {
                    reader.BeginArray();
                    while (reader.HasNext)
                    {
                        StickerPack stickerPack = readStickerPack(reader);
                        stickerPackList.Add(stickerPack);
                    }
                    reader.EndArray();
                }
                else
                {
                    throw new Java.Lang.IllegalStateException("unknown field in json: " + key);
                }
            }
            reader.EndObject();
            if (stickerPackList.Count == 0)
            {
                throw new Java.Lang.IllegalStateException("sticker pack list cannot be empty");
            }
            foreach (StickerPack stickerPack in stickerPackList)
            {
                stickerPack.setAndroidPlayStoreLink(androidPlayStoreLink);
                stickerPack.setIosAppStoreLink(iosAppStoreLink);
            }
            return stickerPackList;
        }

        private static StickerPack readStickerPack(JsonReader reader)
        {
            reader.BeginObject();
            string identifier = null;
            string name = null;
            string publisher = null;
            string trayImageFile = null;
            string publisherEmail = null;
            string publisherWebsite = null;
            string privacyPolicyWebsite = null;
            string licenseAgreementWebsite = null;
            string imageDataVersion = "";
            bool avoidCache = false;
            List<Sticker> stickerList = null;
            while (reader.HasNext)
            {
                String key = reader.NextName();
                switch (key)
                {
                    case "identifier":
                        identifier = reader.NextString();
                        break;
                    case "name":
                        name = reader.NextString();
                        break;
                    case "publisher":
                        publisher = reader.NextString();
                        break;
                    case "tray_image_file":
                        trayImageFile = reader.NextString();
                        break;
                    case "publisher_email":
                        publisherEmail = reader.NextString();
                        break;
                    case "publisher_website":
                        publisherWebsite = reader.NextString();
                        break;
                    case "privacy_policy_website":
                        privacyPolicyWebsite = reader.NextString();
                        break;
                    case "license_agreement_website":
                        licenseAgreementWebsite = reader.NextString();
                        break;
                    case "stickers":
                        stickerList = readStickers(reader);
                        break;
                    case "image_data_version":
                        imageDataVersion = reader.NextString();
                        break;
                    case "avoid_cache":
                        avoidCache = reader.NextBoolean();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            if (TextUtils.IsEmpty(identifier))
            {
                throw new Java.Lang.IllegalStateException("identifier cannot be empty");
            }
            if (TextUtils.IsEmpty(name))
            {
                throw new Java.Lang.IllegalStateException("name cannot be empty");
            }
            if (TextUtils.IsEmpty(publisher))
            {
                throw new Java.Lang.IllegalStateException("publisher cannot be empty");
            }
            if (TextUtils.IsEmpty(trayImageFile))
            {
                throw new Java.Lang.IllegalStateException("tray_image_file cannot be empty");
            }
            if (stickerList == null || stickerList.Count == 0)
            {
                throw new Java.Lang.IllegalStateException("sticker list is empty");
            }
            if (identifier.Contains("..") || identifier.Contains("/"))
            {
                throw new Java.Lang.IllegalStateException("identifier should not contain .. or / to prevent directory traversal");
            }
            if (TextUtils.IsEmpty(imageDataVersion))
            {
                throw new Java.Lang.IllegalStateException("image_data_version should not be empty");
            }
            reader.EndObject();
            StickerPack stickerPack = new StickerPack(identifier, name, publisher, trayImageFile, publisherEmail, publisherWebsite, privacyPolicyWebsite, licenseAgreementWebsite, imageDataVersion, avoidCache);
            stickerPack.setStickers(stickerList);
            return stickerPack;
        }

        private static List<Sticker> readStickers(JsonReader reader)
        {
            reader.BeginArray();
            List<Sticker> stickerList = new List<Sticker>();

            while (reader.HasNext)
            {
                reader.BeginObject();
                String imageFile = null;
                List<String> emojis = new List<string>(3);
                while (reader.HasNext)
                {
                    String key = reader.NextName();
                    if ("image_file".Equals(key))
                    {
                        imageFile = reader.NextString();
                    }
                    else if ("emojis".Equals(key))
                    {
                        reader.BeginArray();
                        while (reader.HasNext)
                        {
                            String emoji = reader.NextString();
                            if (!TextUtils.IsEmpty(emoji))
                            {
                                emojis.Add(emoji);
                            }
                        }
                        reader.EndArray();
                    }
                    else
                    {
                        throw new Java.Lang.IllegalStateException("unknown field in json: " + key);
                    }
                }
                reader.EndObject();
                if (TextUtils.IsEmpty(imageFile))
                {
                    throw new Java.Lang.IllegalStateException("sticker image_file cannot be empty");
                }
                if (!imageFile.EndsWith(".webp"))
                {
                    throw new Java.Lang.IllegalStateException("image file for stickers should be webp files, image file is: " + imageFile);
                }
                if (imageFile.Contains("..") || imageFile.Contains("/"))
                {
                    throw new Java.Lang.IllegalStateException("the file name should not contain .. or / to prevent directory traversal, image file is:" + imageFile);
                }
                stickerList.Add(new Sticker(imageFile, emojis));
            }
            reader.EndArray();
            return stickerList;
        }
    }
}