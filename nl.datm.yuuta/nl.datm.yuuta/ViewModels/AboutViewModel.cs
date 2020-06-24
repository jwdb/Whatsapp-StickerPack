﻿using nl.datm.yuuta.Services;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace nl.datm.yuuta.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ICommand AddStickerPackCommand { get; set; }
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
            AddStickerPackCommand = new Command(() => DependencyService.Get<AddStickerService>().AddStickerPackToWhatsApp("1", "Cuppy"));
        }

        public ICommand OpenWebCommand { get; }
    }
}