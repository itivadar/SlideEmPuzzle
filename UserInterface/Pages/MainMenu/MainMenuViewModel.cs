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
			OpenAboutCommand = new DelegateCommand(OpenAboutPage);
			OpenHowToPlayPageCommand = new DelegateCommand(OpenHowToPlayPage);
		}

		#endregion Public Constructors


		#region Public Properties

		/// <summary>
		/// Gets the command to open the About Page.
		/// </summary>
		public ICommand OpenAboutCommand { get; set; }

		/// <summary>
		/// Gets the command for opening the How To Play Page.
		/// </summary>
		public ICommand OpenHowToPlayPageCommand { get; set; }

		#endregion Public Properties


		#region Private Methods

		/// <summary>
		/// Opens the About page.
		/// </summary>
		private void OpenAboutPage()
		{
			NavigationService.ShowPage(AppPages.PuzzleSelectorPage);
		}

		private void OpenHowToPlayPage()
		{
			NavigationService.ShowPage(AppPages.HowToPlayPage);
		}
		#endregion Private Methods
	}
}