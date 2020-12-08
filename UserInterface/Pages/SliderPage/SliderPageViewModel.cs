using Prism.Commands;
using Prism.Mvvm;
using SliderPuzzleGenerator;
using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace UserInterface.Pages.SliderPage
{
    public class SliderPageViewModel : BindableBase
    {
        private readonly IPuzzleSolver _puzzleSolver;
        private readonly IPuzzleGenerator _puzzleGenerator;

        private DispatcherTimer _timer;
        private ObservableBoard _sliderState;
        private int  _playerMoves;
        private TimeSpan _playerTime;
        

        public SliderPageViewModel(IPuzzleSolver puzzleSolver, IPuzzleGenerator puzzleGenerator)
        {
            _puzzleSolver = puzzleSolver;
            _puzzleGenerator = puzzleGenerator;
      
            RandomizeCommand = new DelegateCommand(OnRandomize);

            ConfigureTimer();
        }

        public ICommand RandomizeCommand { get; set; }

        public TimeSpan PlayerTime
        {
            get => _playerTime;
            private set
            {
                _playerTime = value;
                RaisePropertyChanged(nameof(PlayerTime));
            }
        }

        public int PlayerMoves
        {
            get => _playerMoves;
            private set
            {
                _playerMoves = value;
                RaisePropertyChanged(nameof(PlayerMoves));
            }
            
        }
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

        private void ConfigureTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
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
    }
}

