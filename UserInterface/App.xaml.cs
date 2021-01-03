﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UserInterface.BootstraperSpace;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstraper = new Bootstraper();
            bootstraper.ConfigureContainer();
            bootstraper.SetMainPage(AppPages.MainMenuPage);
            bootstraper.GetView<MainWindow>().Show();
        }
    }
}
