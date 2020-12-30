using Prism.Commands;
using Prism.Mvvm;
using SliderPuzzleGenerator;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace UserInterface.Pages.SliderPage
{
    public class SliderPageViewModel : BindableBase
    {
        #region Private Fields

        private readonly IPuzzleGenerator _puzzleGenerator;
        private readonly IPuzzleSolver _puzzleSolver;
        private int _playerMoves;
        private TimeSpan _playerTime;
        private ObservableBoard _sliderState;
        private DispatcherTimer _timer;

        #endregion Private Fields

        #region Public Constructors

        public SliderPageViewModel(IPuzzleSolver puzzleSolver, IPuzzleGenerator puzzleGenerator)
        {
            _puzzleSolver = puzzleSolver;
            _puzzleGenerator = puzzleGenerator;

            RandomizeCommand = new DelegateCommand(OnRandomize);

            ConfigureTimer();
        }

        #endregion Public Constructors

        #region Public Properties

        public int PlayerMoves
        {
            get => _playerMoves;
            private set
            {
                _playerMoves = value;
                RaisePropertyChanged(nameof(PlayerMoves));
            }
        }

        public TimeSpan PlayerTime
        {
            get => _playerTime;
            private set
            {
                _playerTime = value;
                RaisePropertyChanged(nameof(PlayerTime));
            }
        }

        public ICommand RandomizeCommand { get; set; }

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

        #region Private Methods

        private void ConfigureTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        private void OnRandomize()
        {
            ResetPlayerState();
            var board = _puzzleGenerator.GenerateRandomPuzzle(3);
            SliderState = new ObservableBoard(board);
            _timer.Start();
        }

        private void OnStateChanged()
        {
            PlayerMoves++;
            if (SliderState.IsSolved)
            {
                MessageBox.Show("You did it motherforker");
                _timer.Stop();
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            PlayerTime = PlayerTime.Add(TimeSpan.FromSeconds(1));
        }

        private void ResetPlayerState()
        {
            PlayerMoves = 0;
            PlayerTime = TimeSpan.Zero;
        }

        #endregion Private Methods
    }
}