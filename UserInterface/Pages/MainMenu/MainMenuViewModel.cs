using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UserInterface.BootstraperSpace;

namespace UserInterface.Pages.MainMenu
{
    public class MainMenuViewModel :BindableBase
    {
        private readonly INavigationService _navigationService;
        public ICommand OpenAboutCommand { get;  set; }
        public MainMenuViewModel(INavigationService nagivation)
        {
            _navigationService = nagivation;
            OpenAboutCommand = new DelegateCommand(OnAbout);
        }


        private void OnAbout()
        {
            _navigationService.SetMainPage(AppPages.PuzzleSelectorPage);
        }
        

    }


}
