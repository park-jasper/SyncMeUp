﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace SyncMeUp.GTK
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();

            var app = new App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Sync Me Up!");
            window.Show();

            Gtk.Application.Run();
        }
    }
}