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

        private int _playerMoves;
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

        private void ConfigureTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        private void OnRandomize()
        {
            ResetPlayerState();
            var board = _puzzleGenerator.GenerateRandomPuzzle(4);
            SliderState = new ObservableBoard("1 2 3 4 5 6 7 8 0");
            _timer.Start();
        }

        private void OnOpenMainMenu()
        {
            _navigationService.SetMainPage(AppPages.MainMenuPage);
        }

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

        private void OnTimerTick(object sender, EventArgs e)
        {
            PlayerTime = PlayerTime.Add(TimeSpan.FromSeconds(1));
        }

        private void ResetPlayerState()
        {
            PlayerMoves = 0;
            PlayerTime = TimeSpan.Zero;
        }

        private void OnGameFinished()
        {
            //_navigationService.SetMainPage(AppPages.AboutPage);
            _eventAgreggator.GetEvent<GameFinishedEvent>().Publish();
            
        }

        #endregion Private Methods
    }
}