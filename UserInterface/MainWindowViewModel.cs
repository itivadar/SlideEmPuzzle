using Prism.Commands;
using System.Windows;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;

namespace UserInterface
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Private Fields

        private readonly INavigationService _navigationService;
        private FrameworkElement _mainFrame;

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PlayCommand = new DelegateCommand(OnClear);
            MainFrame = navigationService.GetPage(AppPages.MainMenuPage);
        }

        #endregion Public Constructors

        #region Public Properties

        public ICommand AboutCommand { get; private set; }

        public FrameworkElement MainFrame
        {
            get => _mainFrame;
            set
            {
                _mainFrame = value;
                RaisePropertyChanged(nameof(MainFrame));
            }
        }
        public ICommand PlayCommand { get; private set; }

        #endregion Public Properties

        #region Private Methods

        private void OnClear()
        {
            MainFrame = default;
            MainFrame = _navigationService.GetPage(AppPages.PuzzleSelectorPage);
        }

        private void OnExit()
        {
            Application.Current.Shutdown();
        }

        #endregion Private Methods
    }
}