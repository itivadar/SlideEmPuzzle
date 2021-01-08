using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;

namespace UserInterface.Pages.MainMenu
{
    public class MainMenuViewModel : ViewModelBase
    {
        public ICommand OpenAboutCommand { get;  set; }
        public MainMenuViewModel(IEventAggregator eventAggregator, INavigationService navigationService)  :
                    base(eventAggregator, navigationService)
        {
            OpenAboutCommand = new DelegateCommand(OnAbout);
        }


        private void OnAbout()
        {
            NavigationService.ShowPage(AppPages.PuzzleSelectorPage);
        }
        

    }


}
