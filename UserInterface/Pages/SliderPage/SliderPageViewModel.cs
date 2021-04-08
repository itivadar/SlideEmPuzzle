using Prism.Commands;
using Prism.Events;
using SliderPuzzleGenerator;
using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;

namespace UserInterface.Pages.SliderPage
{
	public class SliderPageViewModel : ViewModelBase
	{
		#region Constants

		/// <summary>
		/// Gets the maximum size of the puzzle.
		/// The height of puzzle is considered.
		/// </summary>
		private const int MaxPuzzleSize = 480;

		/// <summary>
		/// Gets the minimum size of the puzzle.
		/// The height of puzzle is considered.
		/// </summary>
		private const int MinPuzzleSize = 290;

		/// <summary>
		/// Gets the spacing between tiles of the puzzles.
		/// Defined in the slider puzzle control.
		/// TODO: extract it in a separate constant class.
		/// </summary>
		private const int TileSpacing = 5;
		#endregion Constants

		#region Private Fields

		private readonly IPuzzleGenerator _puzzleGenerator;
		private readonly IPuzzleSolver _puzzleSolver;
		private bool _isAutoSolved;
		private bool _isPuzzleEnabled;
		private int _minMovesToSolution;
		private short _playerMoves;
		private TimeSpan _playerTime;
		private double _puzzleScale;
		private Visibility _puzzleScaleVisibility;
		private byte _selectedPuzzleRows;
		private ObservableBoard _sliderState;
		private IReadOnlyCollection<SlideDirection> _solutionSteps;
		private short _tileSize;
		private DispatcherTimer _timer;
		#endregion Private Fields

		#region Public Constructors

		public SliderPageViewModel(IPuzzleSolver puzzleSolver,
															 IPuzzleGenerator puzzleGenerator,
															 INavigationService navigationService,
															 IEventAggregator eventAggregator) : base(eventAggregator, navigationService)
		{
			_puzzleGenerator = puzzleGenerator;
			_puzzleSolver = puzzleSolver;
			ConfigureTimer();

			EventAggregator.GetEvent<PuzzleTypeSelectedEvent>().Subscribe(OnPuzzleTypeSelected);
		}

		#endregion Public Constructors


		#region Private Properties
		/// <summary>
		/// Gets a value indicating whether the puzzle has been autosolved
		/// </summary>
		private bool IsAutoSolved
		{
			get => _isAutoSolved;
			set
			{
				_isAutoSolved = value;
				RaisePropertyChanged(nameof(IsAutoSolved));
			}
		}

		#endregion Private Properties

		#region Public Properties

		/// <summary>
		/// Determines if the player can make moves.
		/// </summary>
		public bool IsPuzzleEnabled
		{
			get => _isPuzzleEnabled;
			set
			{
				_isPuzzleEnabled = value;
				RaisePropertyChanged(nameof(IsPuzzleEnabled));
			}
		}

		/// <summary>
		/// Gets the numbers of moves the player made.
		/// A move is considered when the blank tile changes its position in any direction.
		/// </summary>
		public short PlayerMoves
		{
			get => _playerMoves;
			private set
			{
				_playerMoves = value;
				RaisePropertyChanged(nameof(PlayerMoves));
			}
		}

		/// <summary
		/// Gets the time since the player has started to solve the puzzle.
		/// </summary>
		public TimeSpan PlayerTime
		{
			get => _playerTime;
			private set
			{
				_playerTime = value;
				RaisePropertyChanged(nameof(PlayerTime));
			}
		}

		/// <summary>
		/// The state of the puzzle.
		/// Its a puzzle board determining the position of each tile.
		/// </summary>
		public ObservableBoard PuzzleBoard
		{
			get => _sliderState;
			set
			{
				_sliderState = value;
				RaisePropertyChanged(nameof(PuzzleBoard));

				if (value is null) return;
				_sliderState.StateChanged -= OnStateChanged;
				_sliderState.StateChanged += OnStateChanged;
			}
		}

		/// <summary>
		/// Gets or sets the puzzle scale on a scale from 1 to 100.
		/// </summary>
		public double PuzzleScale
		{
			get => _puzzleScale;
			set
			{
				_puzzleScale = value;
				RaisePropertyChanged(nameof(PuzzleScale));
				ScalePuzzle();
			}
		}

		/// <summary>
		/// Determines the visibility of the scaling control.
		/// Due to some WCF limitation, the tiles can not be scaled during animations.
		/// </summary>
		public Visibility PuzzleScaleVisibility
		{
			get => _puzzleScaleVisibility;
			private set
			{
				_puzzleScaleVisibility = value;
				RaisePropertyChanged(nameof(PuzzleScaleVisibility));
			}
		}

		/// <summary>
		/// Gets the solutions steps in directions to solve the puzzle.
		/// Sets only when the player starts the autosolving.
		/// </summary>
		public IReadOnlyCollection<SlideDirection> SolutionSteps
		{
			get => _solutionSteps;
			set
			{
				_solutionSteps = value;
				RaisePropertyChanged(nameof(SolutionSteps));
			}
		}

		/// <summary>
		/// Gets or sets the tile size of the puzzle.
		/// </summary>
		public short TileSize
		{
			get => _tileSize;
			set
			{
				_tileSize = value;
				RaisePropertyChanged(nameof(TileSize));
			}
		}
		#endregion Public Properties

		#region Public Commands

		/// <summary>
		/// Gets the command triggered when the sliding animations stops.
		/// </summary>
		public ICommand AnimationStateChanged
		{
			get => new DelegateCommand(OnAnimationStopped);
		}

		public ICommand NewGameCommand
		{
			get => new DelegateCommand(StartGame);
		}

		/// <summary>
		/// Gets the command to open the MainMenu
		/// </summary>
		public ICommand OpenMainMenuCommand
		{
			get => new DelegateCommand(OnOpenMainMenu);
		}

		/// <summary>
		/// Gets the command to start solving the puzzle.
		/// </summary>
		public ICommand SolveCommand
		{
			get => new DelegateCommand(OnSolve, CanSolve).ObservesProperty(() => IsAutoSolved);
		}
		#endregion Public Commands

		#region Public

		/// <summary>
		/// Triggered when the pages is deactivated
		/// </summary>
		public override void OnDeactivated()
		{
			//avoid playing the animations after the page is dismissed
			PuzzleBoard = null;
		}

		/// <summary>
		/// Triggered when the page is displayed.
		/// </summary>
		public override void OnDisplayed()
		{
			//starts the timer only when the page displayed to avoid delays.
			StartGame();
			PuzzleScale = 50;
		}
		#endregion Public

		#region Private Methods

		/// <summary>
		/// Determines if the solve button should be active.
		/// The solve button is active only if the puzzle is not currently auto solved.
		/// </summary>
		/// <returns>true if the puzzle can be autosolve</returns>
		private bool CanSolve()
		{
			return !IsAutoSolved;
		}

		/// <summary>
		/// Configure timer to have 1s interval ticks.
		/// </summary>
		private void ConfigureTimer()
		{
			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += OnTimerTick;
		}

		/// <summary>
		/// Triggered when the animations regardind the sliding stopped.
		/// </summary>
		private void OnAnimationStopped()
		{
			if (PuzzleBoard.IsSolved)
			{
				OnGameFinished();
			}
		}

		/// <summary>
		/// Handles the game finished event.
		/// Triggered when the user managed to solve the puzzle.
		/// </summary>
		private void OnGameFinished()
		{
			_timer.Stop();
			EventAggregator.GetEvent<GameFinishedEvent>().Publish(new GameFinishedEvent
			{
				IsAutoSolved = IsAutoSolved,
				ElapsedTime = PlayerTime,
				MovesCount = PlayerMoves,
				PuzzleRows = PuzzleBoard.Rows,
				MinMoves = _minMovesToSolution
			});

			NavigationService.ShowPageAfterDelay(AppPages.GameOverPage, TimeSpan.FromMilliseconds(600));
		}

		/// <summary>
		/// Displays MainMenu window.
		/// </summary>
		private void OnOpenMainMenu()
		{
			NavigationService.ShowPage(AppPages.MainMenuPage);
		}

		/// <summary>
		/// Draw the selected slider type.
		/// </summary>
		/// <param name="puzzleTypeSelected">The type selected by the puzzle</param>
		private void OnPuzzleTypeSelected(string puzzleTypeSelected)
		{
			_selectedPuzzleRows = byte.Parse(puzzleTypeSelected);
		}

		/// <summary>
		/// Solve the puzzle
		/// </summary>
		private async void OnSolve()
		{
			IsAutoSolved = true;
			PuzzleScaleVisibility = Visibility.Collapsed;
			IsPuzzleEnabled = false;

			SolutionSteps = await SolvePuzzleInBackground();
		}

		/// <summary>
		/// Handles the state changed event.
		/// Triggered every time the user moves a tile.
		/// </summary>
		private void OnStateChanged()
		{
			PlayerMoves++;
		}

		/// <summary>
		/// Handles the timer ticks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerTick(object sender, EventArgs e)
		{
			PlayerTime = PlayerTime.Add(TimeSpan.FromSeconds(1));
		}
		/// <summary>
		/// Resets the state (including timer and player moves).
		/// </summary>
		private void ResetPlayerState()
		{
			PlayerMoves = 0;
			PlayerTime = TimeSpan.Zero;
			PuzzleScaleVisibility = Visibility.Visible;
			IsPuzzleEnabled = true;
			IsAutoSolved = false;
		}

		/// <summary>
		/// Scales the puzzle sizes acording to the slider value.
		/// </summary>
		private void ScalePuzzle()
		{
			var puzzleMaxSize = (MaxPuzzleSize - (PuzzleBoard.Rows - 1) * 2 * TileSpacing) / PuzzleBoard.Rows;
			var puzzleMinSize = (MinPuzzleSize - (PuzzleBoard.Rows - 1) * 2 * TileSpacing) / PuzzleBoard.Rows;
			var scale = (PuzzleScale / 100);
			TileSize = (short)((puzzleMaxSize - puzzleMinSize) * scale + puzzleMinSize);
		}
		/// <summary>
		/// Calculates async the minimum number of steps required to solve the puzzle.
		/// </summary>
		/// <returns></returns>
		private async Task SetMinimumMoves()
		{
			var steps = await SolvePuzzleInBackground();
			_minMovesToSolution = steps.Count;
		}

		/// <summary>
		/// Solves async the puzzle.
		/// </summary>
		/// <returns>steps to solve the puzzle</returns>
		private async Task<IReadOnlyCollection<SlideDirection>> SolvePuzzleInBackground()
		{
			return await Task.Run(() =>
			 {
				 var solutionSteps = _puzzleSolver.GetSolutionDirections(PuzzleBoard.CloneBoard);
				 return solutionSteps;
			 });
		}

		/// <summary>
		/// Starts the game after generating a random board.
		/// </summary>
		private async void StartGame()
		{
			StartTimer();
			PuzzleBoard = new ObservableBoard(_puzzleGenerator.GenerateRandomPuzzle(_selectedPuzzleRows));
			await SetMinimumMoves();
		}

		/// <summary>
		/// Starts the timer for a new game.
		/// </summary>
		private void StartTimer()
		{
			ResetPlayerState();
			_timer.Start();
		}
		#endregion Private Methods
	}
}