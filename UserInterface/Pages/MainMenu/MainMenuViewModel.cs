using Prism.Commands;
using Prism.Events;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;

namespace UserInterface.Pages.MainMenu
{
	public class MainMenuViewModel : ViewModelBase
	{

		#region Public Constructors

		/// <summary>
		/// Initializes the Viewmodel for the Main Menu page.
		/// </summary>
		/// <param name="eventAggregator">The event aggregator</param>
		/// <param name="navigationService">The navigation service</param>
		public MainMenuViewModel(IEventAggregator eventAggregator, INavigationService navigationService) :
								base(eventAggregator, navigationService)
		{
		}

		#endregion Public Constructors


		#region Public Properties
		/// <summary>
		/// Gets the command for starting the game.
		/// </summary>
		public ICommand StartGameCommand
		{
			get => new DelegateCommand(StartGame);
		}

		/// <summary>
		/// Gets the command to open the About Page.
		/// </summary>
		public ICommand OpenAboutPageCommand 
		{ 
			get => new DelegateCommand(OpenAboutPage);
		}

		/// <summary>
		/// Gets the command for opening the How To Play Page.
		/// </summary>
		public ICommand OpenHowToPlayPageCommand 
		{ 
			get => new DelegateCommand(OpenHowToPlayPage);
		}

		#endregion Public Properties


		#region Private Methods

		/// <summary>
		/// Opens the About page.
		/// </summary>
		private void OpenAboutPage()
		{
			NavigationService.ShowPage(AppPages.AboutPage);
		}

		/// <summary>
		/// Opens How To Play page.
		/// </summary>
		private void OpenHowToPlayPage()
		{
			NavigationService.ShowPage(AppPages.HowToPlayPage);
		}

		/// <summary>
		/// Starts the game
		/// </summary>
		private void StartGame()
		{
			NavigationService.ShowPage(AppPages.PuzzleSelectorPage);
		}
		#endregion Private Methods
	}
}