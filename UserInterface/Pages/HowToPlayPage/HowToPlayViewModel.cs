using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;

namespace UserInterface.Pages.HowToPlayPage
{
	internal class HowToPlayViewModel : ViewModelBase

	{

		#region Internal Constructors

		internal HowToPlayViewModel(IEventAggregator eventAggregator, INavigationService navigationService) :
							base(eventAggregator, navigationService)
		{
			OpenMainMenuCommand = new DelegateCommand(OnOpenMainMenu);
		}

		#endregion Internal Constructors

		#region Public Properties

		/// <summary>
		/// Gets the command to open Main Menu
		/// </summary>
		public ICommand OpenMainMenuCommand { get; private set; }

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Opens Main Menu page.
		/// </summary>
		private void OnOpenMainMenu()
		{
			NavigationService.ShowPage(AppPages.MainMenuPage);
		}

		#endregion Private Methods

	}
}
