using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;
using UserInterface.Pages.SliderPage;

namespace UserInterface.Pages.PuzzleSelectorPage
{
    public class PuzzleSelectorViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGreetingsProvider _greetingsProvider;
        private readonly DispatcherTimer _timer;

        private ObservableBoard _puzzleState;
        private Visibility _greetingsVisibility;
        private string _greetings;

        private ObservableBoard _15puzzleBoard;
        private ObservableBoard _9puzzleBoard;
        private ObservableBoard _2puzzleBoard;
        private string _puzzleSelected;

        /// <summary>
        /// The state of the preview puzzle.
        /// </summary>
        public ObservableBoard PuzzleState
        {
            get => _puzzleState;
            set
            {
                _puzzleState = value;
                RaisePropertyChanged(nameof(PuzzleState));
            }
        }

        /// <summary>
        /// The goal board of 15-Puzzle.
        /// </summary>
        private ObservableBoard Board15
        {
            get
            {
                if (_15puzzleBoard is null)
                {
                    _15puzzleBoard = ObservableBoard.GetGoalState(4);
                }
                return _15puzzleBoard;
            }
        }

        /// <summary>
        /// The goal board of 9-Puzzle.
        /// </summary>
        private ObservableBoard Board9
        {
            get
            {
                if (_9puzzleBoard is null)
                {
                    _9puzzleBoard = ObservableBoard.GetGoalState(3);
                }
                return _9puzzleBoard;
            }
        }

        /// <summary>
        /// The goal board of 9-Puzzle.
        /// </summary>
        private ObservableBoard Board2
        {
            get
            {
                if (_2puzzleBoard is null)
                {
                    _2puzzleBoard = ObservableBoard.GetGoalState(2);
                }
                return _2puzzleBoard;
            }
        }

        /// <summary>
        /// Initialize a new class of <see cref="PuzzleSelectorViewModel"/>
        /// </summary>
        /// <param name="navigationService">The service used for navigation between different pages. </param>
        /// <param name="eventAggregate">The eventaggregate used for communicate between different pages.</param>
        public PuzzleSelectorViewModel(
                    INavigationService navigationService, 
                    IEventAggregator eventAggregate,
                    IGreetingsProvider greetingsProvider)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregate;
            _greetingsProvider = greetingsProvider;

            OnMouseOverCommand = new DelegateCommand<string>(OnMouseOver, CanExecute).ObservesProperty(() => PuzzleSelected);
            OnMouseLeftCommand = new DelegateCommand(OnMouseLeft, CanExecute).ObservesProperty(() => PuzzleSelected);
            StartGameCommand = new DelegateCommand<string>(OnStartGame, CanExecute).ObservesProperty(() => PuzzleSelected);
            BackCommand = new DelegateCommand(OnGoBack, CanExecute).ObservesProperty(() => PuzzleSelected);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1200);
            _timer.Tick += OnTimerTick ;
        }

       public string PuzzleSelected
       {
            get => _puzzleSelected;
            private set 
            {
                _puzzleSelected = value;
                RaisePropertyChanged(nameof(PuzzleSelected));
            }
       }

        /// <summary>
        /// Sets the visibility of the greetings text after selecting a puzzle.
        /// </summary>
        public Visibility GreetingsVisibility
        {
            get => _greetingsVisibility;
            private set
            {
                _greetingsVisibility = value;
                RaisePropertyChanged(nameof(GreetingsVisibility));
            }
        }

        /// <summary>
        /// Gets the greetings for the user after chosing to start the game.
        /// </summary>
        public string Greetings
        {
            get => _greetings;
            private set
            {
                _greetings = value;
                RaisePropertyChanged(nameof(Greetings));
            }
        }

        /// <summary>
        /// Command triggered when the mouse is over the button.
        /// </summary>
        public ICommand OnMouseOverCommand { get; private set; }

        /// <summary>
        /// Command triggered when the mouse is leaving the button.
        /// </summary>
        public ICommand OnMouseLeftCommand { get; private set; }

        /// <summary>
        /// Gets the command for starting puzzlin'.
        /// </summary>
        public ICommand StartGameCommand { get; private set; } 

        /// <summary>
        /// Gets the command for displaying the main menu.
        /// </summary>
        public ICommand BackCommand { get; private set; }

        /// <summary>
        /// Triggered on page displayed.
        /// </summary>
        public override void OnDisplayed()
        {
            GreetingsVisibility = Visibility.Collapsed;
            PuzzleSelected = string.Empty;
        }

        /// <summary>
        /// Handles the on mouse event.
        /// </summary>
        /// <param name="args">string argument which type of puzzle the user selected</param>
        private void OnMouseOver(string args)
        {
            if (args == "4")
            {
                PuzzleState = Board15;
            }

            if (args == "3")
            {
                PuzzleState = Board9;
            }

            if(args == "2")
            {
                PuzzleState = Board2;
            }
        }

        /// <summary>
        /// Handles the on mouse left event.
        /// </summary>
        private void OnMouseLeft()
        {
           PuzzleState = null;
        }

        /// <summary>
        /// Starts the game after the user has chosen the puzzle type.
        /// </summary>
        /// <param name="puzzleTypeSelected"></param>
        private void OnStartGame(string puzzleTypeSelected)
        {
            GreetingsVisibility = Visibility.Visible;
            Greetings = _greetingsProvider.GetRandomGreeting();
            PuzzleSelected = puzzleTypeSelected;
            _timer.Start();        
        }

        /// <summary>
        /// Gets the playe back to the main menu. 
        /// </summary>
        private void OnGoBack()
        {
            _navigationService.ShowPage(AppPages.MainMenuPage);
        }

        /// <summary>
        /// Handles the timer tick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            _navigationService.ShowPage(AppPages.SliderPage);
            _eventAggregator.GetEvent<PuzzleTypeSelectedEvent>().Publish(_puzzleSelected);
        }

        /// <summary>
        /// Determines if the buttons are enabled.
        /// Buttons are enabled until the user choices a puzzle type.
        /// </summary>
        /// <param name="args">command parameters</param>
        /// <returns>true, if the a puzzle type has been selected</returns>
        private bool CanExecute(string args)
        {
            return CanExecute();
        }

        /// <summary>
        /// Determines if the buttons are enabled.
        /// Buttons are enabled until the user choices a puzzle type.
        /// </summary>
        /// <param name="args">command parameters</param>
        /// <returns>true, if the a puzzle type has been selected</returns>
        private bool CanExecute()
        {
            return PuzzleSelected == string.Empty;
        }
        
    }
}
