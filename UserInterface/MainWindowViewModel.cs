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
        private MenuViewItem _selectedMenu;
        private readonly INavigationService _navigationService;
        private Dictionary<MenuAction, Action> _menuActionMap;

        public MainWindowViewModel(INavigationService navigationService)
        {
            BuildMenuActionMap();
            _navigationService = navigationService;
            ClearCommand = new DelegateCommand(this.OnClear);
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

        public MenuViewItem SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;
                RaisePropertyChanged(nameof(SelectedMenu));
                OnMenuItemSelected(value);
            }
        }

        public ICommand AboutCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }


        private void OnClear()
        {
            MainFrame = default;
        }

        private void OnMenuItemSelected(MenuViewItem menuItem)
        {
            MainFrame = _navigationService.GetPage(menuItem.Page);
            _menuActionMap[menuItem.Action]?.Invoke();
        }

        private void OnExit()
        {
            Application.Current.Shutdown();
        }

        private void BuildMenuActionMap()
        {
            _menuActionMap = new Dictionary<MenuAction, Action>()
            {
             { MenuAction.Exit, OnExit},
             { MenuAction.None, ()=>{ }}
            };
        }
  
    }
}
