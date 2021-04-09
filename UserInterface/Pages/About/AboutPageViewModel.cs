using Prism.Commands;
using Prism.Events;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;

namespace UserInterface.Pages.About
{
	/// <summary>
	/// The ViewModel for the About page.
	/// </summary>
	internal class AboutPageViewModel : ViewModelBase
	{
		#region Internal Constructors

		/// <summary>
		/// Initializes a <see cref="AboutPageViewModel"/> instance.
		/// </summary>
		/// <param name="eventAggregator">the eventaggregator used for communication.</param>
		/// <param name="navigationService">the navigation service</param>
		internal AboutPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
						: base(eventAggregator, navigationService)
		{
		}

		#endregion Internal Constructors

		#region Public Properties

		/// <summary>
		/// Gets the command to go back to Main Menu page.
		/// </summary>
		public ICommand GoBackCommand
		{
			get => new DelegateCommand(OpenMainMenu);
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Opens Main Menu page.
		/// </summary>
		private void OpenMainMenu()
		{
			NavigationService.ShowPage(AppPages.MainMenuPage);
		}

		#endregion Private Methods
	}
}