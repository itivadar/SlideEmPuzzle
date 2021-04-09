using System;
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
    /// <summary>
    /// Invoked when the application is fired-up.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      var bootstraper = new Bootstraper();
      bootstraper.ConfigureContainer();
      //the inital page displayed when the game is started
      bootstraper.ShowPage(AppPages.MainMenuPage);
      bootstraper.GetView<MainWindow>().Show();
    }
  }
}
