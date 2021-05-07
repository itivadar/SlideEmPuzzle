using Prism.Commands;
using Prism.Events;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;

namespace UserInterface.Pages.About
{
	/// <summary>
	/// The ViewModel for the About page.
	/// </summary>
	internal class AboutPageViewModel : ViewModelBase
	{
		#region Private Fields

		private Visibility _memeImageVisibility;
		private byte _tapsOnPage;
		private bool _shouldMemeBlink;
		private string _gameVersion;

		#endregion Private Fields
		#region Internal Constructors

		/// <summary>
		/// Initializes a <see cref="AboutPageViewModel"/> instance.
		/// </summary>
		/// <param name="eventAggregator">the eventaggregator used for communication.</param>
		/// <param name="navigationService">the navigation service</param>
		internal AboutPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
						: base(eventAggregator, navigationService)
		{
			MouseDownCommand = new DelegateCommand(OnMouseDown);
		}

		#endregion Internal Constructors

		#region Public Methods

		/// <summary>
		/// Method invoked when the About page is displayed.
		/// </summary>
		public override void OnDisplayed()
		{
			_tapsOnPage = 0;
			MemeImageVisibility = Visibility.Collapsed;
			SetGameVersion();
		}

		#endregion Public Methods

		#region Public Properties
		/// <summary>
		/// Gets a value indicating if the pictures should blink
		/// Used like a triggered when the value is true
		/// </summary>
		public bool ShouldMemeBlink
		{
			get => _shouldMemeBlink;
			private set 
			{
				_shouldMemeBlink = value;
				RaisePropertyChanged(nameof(ShouldMemeBlink));
				if (value)
				{
					ShouldMemeBlink = false;
				}
			}
		}

		/// <summary>
		/// Displays the game version
		/// </summary>
		public string GameVersion
		{
			get => _gameVersion;
			private set
			{
				_gameVersion = value;
				RaisePropertyChanged(nameof(GameVersion));
			}
		}

		/// <summary>
		/// Gets the command to go back to Main Menu page.
		/// </summary>
		public ICommand GoBackCommand
		{
			get => new DelegateCommand(OpenMainMenu);
		}

		/// <summary>
		/// Gets the Visibiliy of Easter Egg image.
		/// </summary>
		public Visibility MemeImageVisibility
		{
			get => _memeImageVisibility;
			private set
			{
				_memeImageVisibility = value;
				RaisePropertyChanged(nameof(MemeImageVisibility));
			}
		}

		/// <summary>
		/// Gets the command invoked when the mouse is pressed on the page.
		/// </summary>
		public ICommand MouseDownCommand { get; private set; }

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Triggered when the mouse is pressed on the page.
		/// </summary>
		private void OnMouseDown()
		{
			_tapsOnPage++;
			if (_tapsOnPage == 3)
			{
				ShouldMemeBlink = true;
				MemeImageVisibility = Visibility.Visible;
				EventAggregator.GetEvent<BlinkBorderEvent>().Publish();
				_tapsOnPage = 0;
			}
		}

		/// <summary>
		/// Opens Main Menu page.
		/// </summary>
		private void OpenMainMenu()
		{
			NavigationService.ShowPage(AppPages.MainMenuPage);
		}

		/// <summary>
		/// Sets the displayed gameversion
		/// </summary>
		private void SetGameVersion()
		{
			GameVersion = $"v {Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}";
		}

		#endregion Private Methods
	}
}