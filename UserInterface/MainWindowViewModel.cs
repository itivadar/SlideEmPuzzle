using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.CustomControls;
using UserInterface.Pages.SliderPage;

namespace UserInterface
{
    public class MainWindowViewModel : BindableBase
    {

        private FrameworkElement _mainFrame;
        private readonly INavigationService _navigationService;

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PlayCommand = new DelegateCommand(OnClear);
            MainFrame = navigationService.GetPage(AppPages.MainMenuPage);
        }

        public FrameworkElement MainFrame
        {
            get => _mainFrame;
            set
            {
                _mainFrame = value;
                RaisePropertyChanged(nameof(MainFrame));
            }
        }

        public ICommand AboutCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }


        private void OnClear()
        {
            MainFrame = default;
            MainFrame = _navigationService.GetPage(AppPages.SliderPage);
        }

        private void OnExit()
        {
            Application.Current.Shutdown();
        }
 
    }
}
