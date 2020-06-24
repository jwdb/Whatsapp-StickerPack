using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace nl.datm.yuuta.Droid
{
    internal class WhitelistCheck
    {
        internal static String AUTHORITY_QUERY_PARAM = "authority";
        internal static String IDENTIFIER_QUERY_PARAM = "identifier";
        internal static String STICKER_APP_AUTHORITY = StickerContentProvider.CONTENT_PROVIDER_AUTHORITY;
        internal static String CONSUMER_WHATSAPP_PACKAGE_NAME = "com.whatsapp";
        internal static String SMB_WHATSAPP_PACKAGE_NAME = "com.whatsapp.w4b";
        internal static String CONTENT_PROVIDER = ".provider.sticker_whitelist_check";
        internal static String QUERY_PATH = "is_whitelisted";
        internal static String QUERY_RESULT_COLUMN_NAME = "result";

        static bool IsWhitelisted([NotNull] Context context, [NotNull] String identifier)
        {
            try
            {
                if (!IsWhatsAppConsumerAppInstalled(context.PackageManager) && !IsWhatsAppSmbAppInstalled(context.PackageManager))
                {
                    return false;
                }
                var consumerResult = IsStickerPackWhitelistedInWhatsAppConsumer(context, identifier);
                var smbResult = IsStickerPackWhitelistedInWhatsAppSmb(context, identifier);
                return consumerResult && smbResult;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static bool IsWhitelistedFromProvider(Context context, String identifier, String whatsappPackageName)
        {
            PackageManager packageManager = context.PackageManager;
            if (IsPackageInstalled(whatsappPackageName, packageManager))
            {
                String whatsappProviderAuthority = whatsappPackageName + CONTENT_PROVIDER;
                ProviderInfo providerInfo = packageManager.ResolveContentProvider(whatsappProviderAuthority, PackageInfoFlags.MetaData);
                // provider is not there. The WhatsApp app may be an old version.
                if (providerInfo == null)
                {
                    return false;
                }
                Android.Net.Uri queryUri = new Android.Net.Uri.Builder()
                    .Scheme(ContentResolver.SchemeContent)
                    .Authority(whatsappProviderAuthority)
                    .AppendPath(QUERY_PATH)
                    .AppendQueryParameter(AUTHORITY_QUERY_PARAM, STICKER_APP_AUTHORITY)
                    .AppendQueryParameter(IDENTIFIER_QUERY_PARAM, identifier)
                    .Build();

                var cursor = context.ContentResolver.Query(queryUri, null, null, null, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int whiteListResult = cursor.GetInt(cursor.GetColumnIndexOrThrow(QUERY_RESULT_COLUMN_NAME));
                    return whiteListResult == 1;
                }
            }

            return false;
        }

        internal static bool IsPackageInstalled(String packageName, PackageManager packageManager)
        {
            try
            {
                ApplicationInfo applicationInfo = packageManager.GetApplicationInfo(packageName, 0);
                //noinspection SimplifiableIfStatement
                if (applicationInfo != null)
                {
                    return applicationInfo.Enabled;
                }
                else
                {
                    return false;
                }
            }
            catch (PackageManager.NameNotFoundException e)
            {
                return false;
            }
        }

        internal static bool IsWhatsAppConsumerAppInstalled(PackageManager packageManager)
        {
            return IsPackageInstalled(CONSUMER_WHATSAPP_PACKAGE_NAME, packageManager);
        }

        internal static bool IsWhatsAppSmbAppInstalled(PackageManager packageManager)
        {
            return IsPackageInstalled(SMB_WHATSAPP_PACKAGE_NAME, packageManager);
        }

        internal static bool IsStickerPackWhitelistedInWhatsAppConsumer(Context context, String identifier)
        {
            return IsWhitelistedFromProvider(context, identifier, CONSUMER_WHATSAPP_PACKAGE_NAME);
        }

        internal static bool IsStickerPackWhitelistedInWhatsAppSmb(Context context, String identifier)
        {
            return IsWhitelistedFromProvider(context, identifier, SMB_WHATSAPP_PACKAGE_NAME);
        }
    }
}