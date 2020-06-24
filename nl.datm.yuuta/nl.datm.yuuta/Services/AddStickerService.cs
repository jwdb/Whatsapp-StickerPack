using System;
using System.Collections.Generic;
using System.Text;

namespace nl.datm.yuuta.Services
{
    public interface AddStickerService
    {
        void AddStickerPackToWhatsApp(String identifier, String stickerPackName);
    }
}
