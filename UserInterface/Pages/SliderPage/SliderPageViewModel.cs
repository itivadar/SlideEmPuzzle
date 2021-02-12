using Prism.Commands;
using Prism.Events;
using SliderPuzzleGenerator;
using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// Gets the spacing between tiles of the puzzles.
        /// Defined in the slider puzzle control.
        /// TODO: extract it in a separate constant class.
        /// </summary>
        private const int TileSpacing = 5;

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
        #endregion

        #region Private Fields

        private readonly IPuzzleGenerator _puzzleGenerator;
        private readonly IPuzzleSolver _puzzleSolver;

       
        private TimeSpan _playerTime;
        private ObservableBoard _sliderState;
        private DispatcherTimer _timer;

        private double _puzzleScale;
        private short _tileSize;
        private short _playerMoves;
        private IEnumerable<SlideDirection> _solutionSteps;

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
            AutoSolveCommand = new DelegateCommand(OnAutoSolve);

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

        /// <summary>
        /// Gets the solutions steps in directions to solve the puzzle.
        /// Sets only when the player starts the autosolving.
        /// </summary>
        public IEnumerable<SlideDirection> SolutionSteps
        {
            get => _solutionSteps;
            private set
            {
                _solutionSteps = value;
                RaisePropertyChanged(nameof(SolutionSteps));
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
                _sliderState.StateChanged -= OnStateChanged;
                _sliderState.StateChanged += OnStateChanged;
                RaisePropertyChanged(nameof(PuzzleBoard));
            }
        }

        #endregion Public Properties

        #region Public Commands

        /// <summary>
        /// Gets the command to open the MainMenu
        /// </summary>
        public ICommand OpenMainMenuCommand { get; private set; }

        /// <summary>
        /// Gets the command to start solving the puzzle.
        /// </summary>
        public ICommand AutoSolveCommand { get; private set; }

        #endregion Public Commands

        #region Public

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
        /// Solve the puzzle
        /// </summary>
        private void OnAutoSolve()
        {
            Task.Run(() =>
            {
                SolutionSteps = _puzzleSolver.GetSolutionDirections(PuzzleBoard.Board);
            });
        }

        /// <summary>
        /// Handles the state changed event.
        /// Triggered every time the user moves a tile.
        /// </summary>
        private void OnStateChanged()
        {
            PlayerMoves++;
            if (PuzzleBoard.IsSolved)
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
        /// Scales the puzzle sizes acording to the slider value. 
        /// </summary>
        private void ScalePuzzle()
        {
           var  puzzleMaxSize = (MaxPuzzleSize - (PuzzleBoard.Rows - 1) * 2 * TileSpacing) / PuzzleBoard.Rows;
           var  puzzleMinSize = (MinPuzzleSize - (PuzzleBoard.Rows - 1) * 2 * TileSpacing) / PuzzleBoard.Rows;
           var scale = (PuzzleScale / 100);
           TileSize = (short)((puzzleMaxSize - puzzleMinSize)*scale + puzzleMinSize);
        }

        /// <summary>
        /// Handles the game finished event.
        /// Triggered when the user managed to solve the puzzle.
        /// </summary>
        private void OnGameFinished()
        {
            _timer.Stop();
            //NavigationService.ShowPage(AppPages.GameOverPage);

            EventAggregator.GetEvent<GameFinishedEvent>().Publish(new GameFinishedEvent { ElapsedTime = PlayerTime, MovesCount = PlayerMoves });
        }

        /// <summary>
        /// Draw the selected slider type.
        /// </summary>
        /// <param name="puzzleTypeSelected">The type selected by the puzzle</param>
        private void OnPuzzleTypeSelected(string puzzleTypeSelected)
        {
            var puzzleRows = int.Parse(puzzleTypeSelected);
            //PuzzleBoard = new ObservableBoard("0 3 2 1");
            PuzzleBoard = new ObservableBoard("2 3 0 8 15 12 6 7 13 1 4 9 14 11 10 5");
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