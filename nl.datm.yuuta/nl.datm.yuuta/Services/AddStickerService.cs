using System;
using System.Collections.Generic;
using System.Text;

namespace nl.datm.yuuta.Services
{
    public interface AddStickerService
    {
        void SetStickerResolver(Dictionary<(string identifier, string name, string trayImageFile, byte[] trayImage), Dictionary<(string name, string[] emoji), Func<byte[]>>> stickerResolver);
        Dictionary<(string identifier, string name, string trayImageFile, byte[] trayImage), Dictionary<(string name, string[] emoji), Func<byte[]>>> GetStickerResolver();
        void AddStickerPackToWhatsApp(String identifier, String stickerPackName);
    }
}
