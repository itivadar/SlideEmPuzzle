using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface.CustomControls;

namespace UserInterface
{
    class MainWindowViewModel : BindableBase
    {

        private Uri _mainFrame;
        private MenuViewItem _selectedMenu;
        private Dictionary<MenuAction, Action> _menuActionMap;

        public MainWindowViewModel()
        {
            BuildMenuActionMap();
            ClearCommand = new DelegateCommand(this.OnClear);
        }

        public Uri MainFrame
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
        public DelegateCommand ClearCommand { get; private set; }


        private void OnClear()
        {
            MainFrame = default(Uri);
        }

        private void OnMenuItemSelected(MenuViewItem menuItem)
        {
            MainFrame = menuItem.PageUri;
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
