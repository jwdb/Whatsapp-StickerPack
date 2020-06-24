using System;
using System.Collections.Generic;

namespace nl.datm.WhaStickerProvider.lib.Models
{
    public class Sticker
    {
        public string imageFileName { get; set; }
        public List<string> emojis { get; set; }
        public long size { get; set; }
        public Func<byte[]> loader { get; set; }

        public Sticker(string imageFileName, List<string> emojis, Func<byte[]> loader)
        {
            this.imageFileName = imageFileName;
            this.emojis = emojis;
            this.loader = loader;
        }
    }
}