using System;

using Android.Content;
using Android.Widget;
using nl.datm.yuuta.Droid;
using nl.datm.yuuta.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AddSticker))]
namespace nl.datm.yuuta.Droid
{
    public class AddSticker : AddStickerService
    {
        public const string EXTRA_STICKER_PACK_ID = "sticker_pack_id";
        public const string EXTRA_STICKER_PACK_AUTHORITY = "sticker_pack_authority";
        public const string EXTRA_STICKER_PACK_NAME = "sticker_pack_name";
        private Context _context;

        public AddSticker()
        {
            _context = Android.App.Application.Context;
        }

        public void AddStickerPackToWhatsApp(String identifier, String stickerPackName)
        {
            try
            {
                //if neither WhatsApp Consumer or WhatsApp Business is installed, then tell user to install the apps.
                if (!WhitelistCheck.IsWhatsAppConsumerAppInstalled(_context.PackageManager) && !WhitelistCheck.IsWhatsAppSmbAppInstalled(_context.PackageManager))
                {
                    Toast.MakeText(_context, "Failed to update Whatsapp", ToastLength.Long).Show();
                    return;
                }

                if (!WhitelistCheck.IsStickerPackWhitelistedInWhatsAppConsumer(_context, identifier))
                {
                    LaunchIntentToAddPackToSpecificPackage(identifier, stickerPackName, WhitelistCheck.CONSUMER_WHATSAPP_PACKAGE_NAME);
                }
                else if (!WhitelistCheck.IsStickerPackWhitelistedInWhatsAppSmb(_context, identifier))
                {
                    LaunchIntentToAddPackToSpecificPackage(identifier, stickerPackName, WhitelistCheck.SMB_WHATSAPP_PACKAGE_NAME);
                }
                else
                {
                    Toast.MakeText(_context, "Failed to update Whatsapp", ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                //Log.e(TAG, "error adding sticker pack to WhatsApp", e);
                Toast.MakeText(_context, "Failed to update Whatsapp", ToastLength.Long).Show();
            }

        }

        private void LaunchIntentToAddPackToSpecificPackage(String identifier, String stickerPackName, String whatsappPackageName)
        {
            var intent = new Intent();
            intent.SetAction("com.whatsapp.intent.action.ENABLE_STICKER_PACK");
            intent.PutExtra(EXTRA_STICKER_PACK_ID, identifier);
            intent.PutExtra(EXTRA_STICKER_PACK_AUTHORITY, StickerContentProvider.CONTENT_PROVIDER_AUTHORITY);
            intent.PutExtra(EXTRA_STICKER_PACK_NAME, stickerPackName);
            intent.AddFlags(ActivityFlags.NewTask);
            intent.SetPackage(whatsappPackageName);
            try
            {
                _context.StartActivity(intent);
            }
            catch (ActivityNotFoundException e)
            {
                Toast.MakeText(_context, "Failed to update Whatsapp", ToastLength.Long).Show();
            }
        }
    }
}