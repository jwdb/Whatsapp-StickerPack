using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Text;
using Java.IO;
using nl.datm.WhaStickerProvider.lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace nl.datm.WhaStickerProvider.lib.Droid
{

    public abstract class StickerContentProvider : ContentProvider
    {
        /**
       * Do not change the strings listed below, as these are used by WhatsApp. And changing these will break the interface between sticker app and WhatsApp.
       */
        public const string STICKER_PACK_IDENTIFIER_IN_QUERY = "sticker_pack_identifier";
        public const string STICKER_PACK_NAME_IN_QUERY = "sticker_pack_name";
        public const string STICKER_PACK_PUBLISHER_IN_QUERY = "sticker_pack_publisher";
        public const string STICKER_PACK_ICON_IN_QUERY = "sticker_pack_icon";
        public const string ANDROID_APP_DOWNLOAD_LINK_IN_QUERY = "android_play_store_link";
        public const string IOS_APP_DOWNLOAD_LINK_IN_QUERY = "ios_app_download_link";
        public const string PUBLISHER_EMAIL = "sticker_pack_publisher_email";
        public const string PUBLISHER_WEBSITE = "sticker_pack_publisher_website";
        public const string PRIVACY_POLICY_WEBSITE = "sticker_pack_privacy_policy_website";
        public const string LICENSE_AGREENMENT_WEBSITE = "sticker_pack_license_agreement_website";
        public const string IMAGE_DATA_VERSION = "image_data_version";
        public const string AVOID_CACHE = "whatsapp_will_not_cache_stickers";

        public const string STICKER_FILE_NAME_IN_QUERY = "sticker_file_name";
        public const string STICKER_FILE_EMOJI_IN_QUERY = "sticker_emoji";

        public static Android.Net.Uri AUTHORITY_URI = new Android.Net.Uri.Builder()
                .Scheme(ContentResolver.SchemeContent)
                .Authority(CONTENT_PROVIDER_AUTHORITY)
                .AppendPath(StickerContentProvider.METADATA)
                .Build();

        internal static string CONTENT_PROVIDER_AUTHORITY = ".stickercontentprovider";

        /**
         * Do not change the values in the UriMatcher because otherwise, WhatsApp will not be able to fetch the stickers from the ContentProvider.
         */
        private static UriMatcher MATCHER = new UriMatcher(UriMatcher.NoMatch);
        private const string METADATA = "metadata";
        private const int METADATA_CODE = 1;

        private const int METADATA_CODE_FOR_SINGLE_PACK = 2;

        const string STICKERS = "stickers";
        private const int STICKERS_CODE = 3;

        const string STICKERS_ASSET = "stickers_asset";
        private const int STICKERS_ASSET_CODE = 4;

        private const int STICKER_PACK_TRAY_ICON_CODE = 5;

        private List<StickerPack> stickerPackList;

        public StickerContentProvider(string provider) {
            CONTENT_PROVIDER_AUTHORITY = provider;
        }

        public override ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            try
            {
                int code = MATCHER.Match(uri);
                return code switch
                {
                    METADATA_CODE => GetPackForAllStickerPacks(uri),
                    METADATA_CODE_FOR_SINGLE_PACK => GetCursorForSingleStickerPack(uri),
                    STICKERS_CODE => GetStickersForAStickerPack(uri),
                    _ => throw new Java.Lang.IllegalArgumentException("Unknown URI: " + uri)
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override AssetFileDescriptor OpenAssetFile(Android.Net.Uri uri, String mode)
        {
            int matchCode = MATCHER.Match(uri);
            if (matchCode == STICKERS_ASSET_CODE || matchCode == STICKER_PACK_TRAY_ICON_CODE)
            {
                return GetImageAsset(uri);
            }
            return null;
        }

        public override string GetType(Android.Net.Uri uri)
        {
            int matchCode = MATCHER.Match(uri);
            return matchCode switch
            {
                METADATA_CODE => "vnd.android.cursor.dir/vnd." + CONTENT_PROVIDER_AUTHORITY + "." + METADATA,
                METADATA_CODE_FOR_SINGLE_PACK => "vnd.android.cursor.item/vnd." + CONTENT_PROVIDER_AUTHORITY + "." + METADATA,
                STICKERS_CODE => "vnd.android.cursor.dir/vnd." + CONTENT_PROVIDER_AUTHORITY + "." + STICKERS,
                STICKERS_ASSET_CODE => "image/webp",
                STICKER_PACK_TRAY_ICON_CODE => "image/png",
                _ => throw new Java.Lang.IllegalArgumentException("Unknown URI: " + uri),
            };
        }

        private List<StickerPack> getStickerPackList()
        {
            if (stickerPackList == null && Forms.IsInitialized)
            {
                var stickerResolver = DependencyService.Get<AddStickerService>().GetStickerResolver();


                stickerPackList = stickerResolver;

                foreach (StickerPack stickerPack in stickerPackList)
                {
                    MATCHER.AddURI(CONTENT_PROVIDER_AUTHORITY, STICKERS_ASSET + "/" + stickerPack.identifier + "/" + stickerPack.trayImageFile, STICKER_PACK_TRAY_ICON_CODE);
                    foreach (Sticker sticker in stickerPack.Stickers)
                    {
                        MATCHER.AddURI(CONTENT_PROVIDER_AUTHORITY, STICKERS_ASSET + "/" + stickerPack.identifier + "/" + sticker.imageFileName, STICKERS_ASSET_CODE);
                    }
                }
            }
            return stickerPackList;
        }

        private ICursor GetPackForAllStickerPacks(Android.Net.Uri uri)
        {
            return GetStickerPackInfo(uri, getStickerPackList());
        }

        private ICursor GetCursorForSingleStickerPack(Android.Net.Uri uri)
        {
            var identifier = uri.LastPathSegment;
            var stickerPack = getStickerPackList().FirstOrDefault(stickerPack => identifier.Equals(stickerPack.identifier));
            if (stickerPack != null)
                return GetStickerPackInfo(uri, new List<StickerPack> { stickerPack });

            return GetStickerPackInfo(uri, new List<StickerPack>());
        }

        private ICursor GetStickerPackInfo(Android.Net.Uri uri, List<StickerPack> stickerPackList)
        {
            MatrixCursor cursor = new MatrixCursor(
                    new string[]{
                        STICKER_PACK_IDENTIFIER_IN_QUERY,
                        STICKER_PACK_NAME_IN_QUERY,
                        STICKER_PACK_PUBLISHER_IN_QUERY,
                        STICKER_PACK_ICON_IN_QUERY,
                        ANDROID_APP_DOWNLOAD_LINK_IN_QUERY,
                        IOS_APP_DOWNLOAD_LINK_IN_QUERY,
                        PUBLISHER_EMAIL,
                        PUBLISHER_WEBSITE,
                        PRIVACY_POLICY_WEBSITE,
                        LICENSE_AGREENMENT_WEBSITE,
                        IMAGE_DATA_VERSION,
                        AVOID_CACHE,
                    });

            foreach (StickerPack stickerPack in stickerPackList)
            {
                MatrixCursor.RowBuilder builder = cursor.NewRow();
                builder.Add(stickerPack.identifier);
                builder.Add(stickerPack.name);
                builder.Add(stickerPack.publisher);
                builder.Add(stickerPack.trayImageFile);
                builder.Add(stickerPack.androidPlayStoreLink);
                builder.Add(stickerPack.iosAppStoreLink);
                builder.Add(stickerPack.publisherEmail);
                builder.Add(stickerPack.publisherWebsite);
                builder.Add(stickerPack.privacyPolicyWebsite);
                builder.Add(stickerPack.licenseAgreementWebsite);
                builder.Add(stickerPack.imageDataVersion);
                builder.Add(stickerPack.avoidCache ? 1 : 0);
            }
            cursor.SetNotificationUri(Context.ContentResolver, uri);
            return cursor;
        }

        private ICursor GetStickersForAStickerPack(Android.Net.Uri uri)
        {
            var identifier = uri.LastPathSegment;
            MatrixCursor cursor = new MatrixCursor(new string[] { STICKER_FILE_NAME_IN_QUERY, STICKER_FILE_EMOJI_IN_QUERY });
            var stickerPack = getStickerPackList().FirstOrDefault(stickerPack => identifier.Equals(stickerPack.identifier));
            foreach (Sticker sticker in stickerPack.Stickers)
            {
                cursor.AddRow(new Java.Lang.Object[] { sticker.imageFileName, string.Join(",", sticker.emojis) });
            }
            cursor.SetNotificationUri(Context.ContentResolver, uri);
            return cursor;
        }

        private AssetFileDescriptor GetImageAsset(Android.Net.Uri uri)
        {
            var pathSegments = uri.PathSegments;
            if (pathSegments.Count != 3)
            {
                throw new ArgumentException("path segments should be 3, uri is: " + uri);
            }
            var fileName = pathSegments[pathSegments.Count - 1];
            var identifier = pathSegments[pathSegments.Count - 2];

            if (TextUtils.IsEmpty(identifier))
            {
                throw new ArgumentException("identifier is empty, uri: " + uri);
            }
            if (TextUtils.IsEmpty(fileName))
            {
                throw new ArgumentException("file name is empty, uri: " + uri);
            }

            //making sure the file that is trying to be fetched is in the list of stickers.
            var stickerPack = getStickerPackList().FirstOrDefault(stickerPack => identifier.Equals(stickerPack.identifier));
            if (stickerPack != null)
            {
                var file = new File(Context.GetExternalCacheDirs().FirstOrDefault(), fileName);

                if (fileName.Equals(stickerPack.trayImageFile))
                {
                    var fileOutputStream = new FileOutputStream(file);
                    fileOutputStream.Write(stickerPack.trayImage);
                }
                else
                {
                    var sticker = stickerPack.Stickers.Where(sticker => fileName.Equals(sticker.imageFileName)).FirstOrDefault();
                    if (sticker != null)
                    {
                        var fileOutputStream = new FileOutputStream(file);
                        fileOutputStream.Write(sticker.loader?.Invoke());
                    }
                }

                return new AssetFileDescriptor(Android.OS.ParcelFileDescriptor.Open(file, Android.OS.ParcelFileMode.ReadOnly), 0, AssetFileDescriptor.UnknownLength);
            }
            return null;
        }

        public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException("Not supported");
        }

        public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues values)
        {
            throw new NotImplementedException("Not supported");
        }

        public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException("Not supported");
        }

        public override bool OnCreate()
        {
            string authority = CONTENT_PROVIDER_AUTHORITY;
            if (!authority.StartsWith(Context.PackageName))
            {
                throw new Exception("your authority (" + authority + ") for the content provider should start with your package name: " + Context.PackageName);
            }
            //the call to get the metadata for the sticker packs.
            MATCHER.AddURI(authority, METADATA, METADATA_CODE);

            //the call to get the metadata for single sticker pack. * represent the identifier
            MATCHER.AddURI(authority, METADATA + "/*", METADATA_CODE_FOR_SINGLE_PACK);

            //gets the list of stickers for a sticker pack, * respresent the identifier.
            MATCHER.AddURI(authority, STICKERS + "/*", STICKERS_CODE);

            return true;
        }
    }
}