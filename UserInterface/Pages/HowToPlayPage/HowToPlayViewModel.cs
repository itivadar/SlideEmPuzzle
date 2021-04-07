using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Helpers;
using UserInterface.Pages.SliderPage;

namespace UserInterface.Pages.HowToPlayPage
{
	internal class HowToPlayViewModel : ViewModelBase

	{
		private ObservableBoard _goalState = new ObservableBoard("1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 0");

		#region Internal Constructors

		internal HowToPlayViewModel(IEventAggregator eventAggregator, INavigationService navigationService) :
							base(eventAggregator, navigationService)
		{
			OpenMainMenuCommand = new DelegateCommand(OnOpenMainMenu);
		}

		#endregion Internal Constructors

		#region Public Properties

		public ObservableBoard GoalState
		{
			get => _goalState;
			set
			{
				_goalState = value;
			}
		}

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
