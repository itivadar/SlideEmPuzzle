using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace UserInterface
{
    class MainWindowViewModel : BindableBase
    {

        private Uri _mainFrame;
        private ListViewItem _selectedMenu;

        public Uri MainFrame
        {
            get => _mainFrame;
            set
            {
                _mainFrame = value;
                RaisePropertyChanged(nameof(MainFrame));
            }
        }

        public ListViewItem SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                _selectedMenu = value;
                RaisePropertyChanged(nameof(SelectedMenu));
            }
        }

        public ICommand AboutCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        

        public MainWindowViewModel()
        {

            AboutCommand = new DelegateCommand(this.OpenAboutPage);
            ClearCommand = new DelegateCommand(this.OnClear);
            OpenAboutPage();
        }

        private void OnClear()
        {
            MainFrame = default(Uri);
        }

        private void OpenAboutPage()
        {
            MainFrame = new Uri("Pages/About/AboutPage.xaml", UriKind.Relative);
        }
    }
}
