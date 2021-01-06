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
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Pages.SliderPage;

namespace UserInterface.Pages.PuzzleSelectorPage
{
    public class PuzzleSelectorViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        private ObservableBoard _puzzleState;

        private ObservableBoard _15puzzleBoard;
        private ObservableBoard _9puzzleBoard;
        private ObservableBoard _2puzzleBoard;

        /// <summary>
        /// Initialize a new class of <see cref="PuzzleSelectorViewModel"/>
        /// </summary>
        /// <param name="navigationService">The service used for navigation between different pages. </param>
        /// <param name="eventAggregate">The eventaggregate used for communicate between different pages.</param>
        public PuzzleSelectorViewModel(INavigationService navigationService, IEventAggregator eventAggregate)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregate;

            OnMouseOverCommand = new DelegateCommand<string>(OnMouseOver);
            OnMouseLeftCommand = new DelegateCommand(OnMouseLeft);
            StartGameCommand = new DelegateCommand<string>(OnStartGame);
            BackCommand = new DelegateCommand(OnGoBack);
        }

        /// <summary>
        /// The goal board of 15-Puzzle.
        /// </summary>
        private ObservableBoard Board15
        {
            get 
            {
                if(_15puzzleBoard is null)
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

        private void OnStartGame(string puzzleTypeSelected)
        {         
            _navigationService.ShowPage(AppPages.SliderPage);
            _eventAggregator.GetEvent<PuzzleTypeSelectedEvent>().Publish(puzzleTypeSelected);
        }

        private void OnGoBack()
        {
            _navigationService.ShowPage(AppPages.MainMenuPage);
        }
    }
}
