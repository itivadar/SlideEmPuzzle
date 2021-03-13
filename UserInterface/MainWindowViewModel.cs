using Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;

namespace UserInterface
{
  /// <summary>
  /// The ViewModel for the MainWindow.
  /// </summary>
  public class MainWindowViewModel : ViewModelBase
  {
    #region Private Fields
    private Page _mainFrame;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Initializes a new ViewModel
    /// </summary>
    /// <param name="navigationService">nagivation service used for displaying pages.</param>
    public MainWindowViewModel(IEventAggregator eventAggregator, INavigationService navigationService) :
            base(eventAggregator, navigationService)
    {
      PlayCommand = new DelegateCommand(OnPlay);
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Gets the command for displaying the about page.
    /// </summary>
    public ICommand AboutCommand { get; private set; }

    /// <summary>
    /// Gets the command for displaying playing page.
    /// </summary>
    public ICommand PlayCommand { get; private set; }

    /// <summary>
    /// Gets or sets the page displayed in the main area.
    /// </summary>
    public Page MainFrame
    {
      get => _mainFrame;
      set
      {
        _mainFrame = value;
        RaisePropertyChanged(nameof(MainFrame));
      }
    }

    #endregion Public Properties

    #region Private Methods

    /// <summary>
    /// Opens the puzzle selection page.
    /// </summary>
    private void OnPlay()
    {
      MainFrame = NavigationService.GetPage(AppPages.PuzzleSelectorPage);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    private void OnExit()
    {
      Application.Current.Shutdown();
    }

    #endregion Private Methods
  }
}