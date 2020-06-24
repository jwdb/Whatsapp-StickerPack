﻿using nl.datm.WhaStickerProvider.lib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace nl.datm.WhaStickerProvider.lib
{
    public static class Provider
    {
        public static void Init(Assembly resourceAssembly, StickerContentJson sContent, string assetString)
        {
            var stickers = new List<StickerPack>();

            foreach (var stickerPack in sContent.sticker_packs)
            {
                byte[] trayImage = null;
                using (Stream traystream = resourceAssembly.GetManifestResourceStream($"{assetString}._{stickerPack.identifier}.{stickerPack.tray_image_file}"))
                {
                    trayImage = new byte[traystream.Length];
                    traystream.Read(trayImage, 0, trayImage.Length);
                }

                var stickerSource = stickerPack.stickers.Select(item => new Sticker(item.image_file, item.emojis.ToList(), new Func<byte[]>(() =>
                {
                    byte[] stickImage = null;
                    using (Stream stickstream = resourceAssembly.GetManifestResourceStream($"{assetString}._{stickerPack.identifier}.{item.image_file}"))
                    {
                        stickImage = new byte[stickstream.Length];
                        stickstream.Read(stickImage, 0, stickImage.Length);
                    }

                    return stickImage;
                })
                )).ToList();

                stickers.Add(new StickerPack(stickerPack.identifier, stickerPack.name, stickerPack.tray_image_file, trayImage, stickerSource));
            }

            DependencyService.Get<AddStickerService>().SetStickerResolver(stickers);
        }

        public static void AddStickerPackToWhatsApp(string identifier, string stickerPackName) {
            DependencyService.Get<AddStickerService>().AddStickerPackToWhatsApp(identifier, stickerPackName);
        }
    }
}
