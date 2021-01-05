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
using UserInterface.Pages.SliderPage;

namespace UserInterface.Pages.PuzzleSelectorPage
{
    public class PuzzleSelectorViewModel : BindableBase
    {
        private readonly INavigationService _naviagationService;
        private readonly IEventAggregator _eventAggregator;

        private BitmapImage _puzzleImage;
        private ObservableBoard _puzzleState;
    
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
        /// The image of the puzzle selected by the user. 
        /// </summary>
        public BitmapImage PuzzleImageSelected 
        { 
            get => _puzzleImage;
            private set
            {
                _puzzleImage = value;
                RaisePropertyChanged(nameof(PuzzleImageSelected));
            }
        }

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
        /// Initialize a new class of <see cref="PuzzleSelectorViewModel"/>
        /// </summary>
        /// <param name="navigationService">The service used for navigation between different pages. </param>
        /// <param name="eventAggregate">The eventaggregate used for communicate between different pages.</param>
        public PuzzleSelectorViewModel(INavigationService navigationService,IEventAggregator eventAggregate)
        {
            _naviagationService = navigationService;
            _eventAggregator = eventAggregate;

            OnMouseOverCommand = new DelegateCommand<string>(OnMouseOver);
            OnMouseLeftCommand = new DelegateCommand(OnMouseLeft);
            StartGameCommand = new DelegateCommand(OnStartGame);
        }

        /// <summary>
        /// Handles the on mouse event.
        /// </summary>
        /// <param name="args">string argument which type of puzzle the user selected</param>
        private void OnMouseOver(string args)
        {
            if (args == "15")
            {
                PuzzleState = new ObservableBoard("1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 0");
            }

            if (args == "9")
            {
                PuzzleState = new ObservableBoard("1 2 3 4 5 6 7 8 9 0");
            }
        }

        /// <summary>
        /// Handles the on mouse left event.
        /// </summary>
        private void OnMouseLeft()
        {
           PuzzleState = null;
        }

        private void OnStartGame()
        {
            _naviagationService.SetMainPage(AppPages.SliderPage);
        }
        
    }
}
