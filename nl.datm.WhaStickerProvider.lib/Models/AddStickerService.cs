using System.Collections.Generic;

namespace nl.datm.WhaStickerProvider.lib.Models
{
    public interface AddStickerService
    {
        void SetStickerResolver(List<StickerPack> stickerResolver);
        List<StickerPack> GetStickerResolver();
        void AddStickerPackToWhatsApp(string identifier, string stickerPackName);
    }
}
