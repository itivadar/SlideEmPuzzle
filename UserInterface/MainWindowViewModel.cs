using Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
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
		private bool _shoulAnimate;

		#endregion Private Fields

		#region Public Constructors

		/// <summary>
		/// Initializes a new ViewModel
		/// </summary>
		/// <param name="navigationService">nagivation service used for displaying pages.</param>
		public MainWindowViewModel(IEventAggregator eventAggregator, INavigationService navigationService) :
						base(eventAggregator, navigationService)
		{
			CloseCommand = new DelegateCommand(OnExit);
			EventAggregator.GetEvent<BlinkBorderEvent>().Subscribe(RunEasterEgg);
		}

		#endregion Public Constructors

		#region Public Properties

		/// <summary>
		/// Gets the command for close the game.
		/// </summary>
		public ICommand CloseCommand { get; private set; }

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

		/// <summary>
		/// Gets or sets a value indicating whether the border blinking animation should start.
		/// </summary>
		public bool ShouldAnimateBorder
		{
			get => _shoulAnimate;
			set
			{
				_shoulAnimate = value;
				RaisePropertyChanged(nameof(ShouldAnimateBorder));

				//it needs to be reseted in order to trigger the event once more.
				if (value)
				{
					ShouldAnimateBorder = false;
				}
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Exits the application.
		/// </summary>
		private void OnExit()
		{
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Run the Easter Egg as the border blinking animation
		/// </summary>
		private void RunEasterEgg()
		{
			ShouldAnimateBorder = true;
		}

		#endregion Private Methods
	}
}