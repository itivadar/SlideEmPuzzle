using Prism.Commands;
using Prism.Events;
using SliderPuzzleGenerator;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;

namespace UserInterface.Pages.SliderPage
{
    public class SliderPageViewModel : ViewModelBase
    {
        #region Private Fields

        private readonly IPuzzleGenerator _puzzleGenerator;
        private readonly IPuzzleSolver _puzzleSolver;

        private short _playerMoves;
        private TimeSpan _playerTime;
        private ObservableBoard _sliderState;
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

            OpenMainMenuCommand = new DelegateCommand(OnOpenMainMenu);

            EventAggregator.GetEvent<PuzzleTypeSelectedEvent>().Subscribe(OnPuzzleTypeSelected);
        }

        #endregion Public Constructors

        #region Public Properties

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

        /// <summary>
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
        public ObservableBoard SliderState
        {
            get => _sliderState;
            set
            {
                _sliderState = value;
                _sliderState.StateChanged -= OnStateChanged;
                _sliderState.StateChanged += OnStateChanged;
                RaisePropertyChanged(nameof(SliderState));
            }
        }

        #endregion Public Properties

        #region Public Commands

        /// <summary>
        /// Gets the command to open the MainMenu
        /// </summary>
        public ICommand OpenMainMenuCommand { get; private set; }

        #endregion Public Commands

        #region Public

        /// <summary>
        /// Triggered when the page is displayed.
        /// </summary>
        public override void OnDisplayed()
        {
            //starts the timer only when the page displayed to avoid delays.
            StartGame();
            var bestMoves = _puzzleSolver.SolutionSteps(_sliderState.Board);
        }

        #endregion Public

        #region Private Methods

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
        /// Handles the timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            PlayerTime = PlayerTime.Add(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Displays MainMenu window.
        /// </summary>
        private void OnOpenMainMenu()
        {
            NavigationService.ShowPage(AppPages.MainMenuPage);
        }

        /// <summary>
        /// Handles the state changed event.
        /// Triggered every time the user moves a tile.
        /// </summary>
        private void OnStateChanged()
        {
            PlayerMoves++;
            if (SliderState.IsSolved)
            {
                OnGameFinished();
            }
        }

        /// <summary>
        /// Resets the state (including timer and player moves).
        /// </summary>
        private void ResetPlayerState()
        {
            PlayerMoves = 0;
            PlayerTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Handles the game finished event.
        /// Triggered when the user managed to solve the puzzle.
        /// </summary>
        private void OnGameFinished()
        {
            _timer.Stop();
            NavigationService.ShowPage(AppPages.GameOverPage);

            EventAggregator.GetEvent<GameFinishedEvent>().Publish(new GameFinishedEvent { ElapsedTime = PlayerTime, MovesCount = PlayerMoves });
        }

        /// <summary>
        /// Draw the selected slider type.
        /// </summary>
        /// <param name="puzzleTypeSelected">The type selected by the puzzle</param>
        private void OnPuzzleTypeSelected(string puzzleTypeSelected)
        {
            var puzzleRows = int.Parse(puzzleTypeSelected);
            SliderState = new ObservableBoard(_puzzleGenerator.GenerateRandomPuzzle(puzzleRows));
        }

        /// <summary>
        /// Starts the timer for a new game.
        /// </summary>
        private void StartGame()
        {
            ResetPlayerState();
            _timer.Start();
        }

        #endregion Private Methods
    }
}