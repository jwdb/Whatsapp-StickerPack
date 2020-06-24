using Android.Content;

namespace nl.datm.yuuta.Droid
{
    [ContentProvider(new string[] { "nl.datm.yuuta.Droid.stickercontentprovider" }, Enabled = true, Exported = true, ReadPermission = "com.whatsapp.sticker.READ")]
    public class StickerContentProvider : WhaStickerProvider.lib.Droid.StickerContentProvider
    {
        public StickerContentProvider() : base("nl.datm.yuuta.Droid.stickercontentprovider") { }
    }
}