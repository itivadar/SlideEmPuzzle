using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SliderPuzzleGenerator;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UserInterface.BootstraperSpace;
using UserInterface.Events;

namespace UserInterface.Pages.SliderPage
{
    public class SliderPageViewModel : BindableBase
    {
        #region Private Fields

        private readonly IPuzzleGenerator _puzzleGenerator;
        private readonly IPuzzleSolver _puzzleSolver;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAgreggator;

        private short _playerMoves;
        private TimeSpan _playerTime;
        private ObservableBoard _sliderState;
        private DispatcherTimer _timer;

        #endregion Private Fields

        #region Public Constructors

        public SliderPageViewModel(IPuzzleSolver puzzleSolver, 
                                   IPuzzleGenerator puzzleGenerator,
                                   INavigationService navigationService, 
                                   IEventAggregator eventAggregator)
        {
            _puzzleSolver = puzzleSolver;
            _puzzleGenerator = puzzleGenerator;
            _navigationService = navigationService;
            _eventAgreggator = eventAggregator;

            ConfigureTimer();

            RandomizeCommand = new DelegateCommand(OnRandomize);
            OpenMainMenuCommand = new DelegateCommand(OnOpenMainMenu);

            _eventAgreggator.GetEvent<PuzzleTypeSelectedEvent>().Subscribe(OnPuzzleTypeSelected);
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
        public ICommand RandomizeCommand { get; private set; }

        public ICommand OpenMainMenuCommand { get; private set; }

        #endregion
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

        private void OnRandomize()
        {
            ResetPlayerState();
            SliderState = new ObservableBoard("1 2 3 4 5 6 7 8 0");
            _timer.Start();
        }

        private void OnOpenMainMenu()
        {
            _navigationService.ShowPage(AppPages.MainMenuPage);
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
                MessageBox.Show("You did it motherforker");
                OnGameFinished();
                _timer.Stop();
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
            //_navigationService.SetMainPage(AppPages.AboutPage);
            _eventAgreggator.GetEvent<GameFinishedEvent>().Publish();
            
        }

        private void OnPuzzleTypeSelected(string puzzleTypeSelected)
        {
            ResetPlayerState();
            var puzzleRows = int.Parse(puzzleTypeSelected);
            SliderState = new ObservableBoard(_puzzleGenerator.GenerateRandomPuzzle(puzzleRows));     
            _timer.Start();
        }

        #endregion Private Methods
    }
}