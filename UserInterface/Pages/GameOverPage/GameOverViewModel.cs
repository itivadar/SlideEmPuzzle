using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;

namespace UserInterface.Pages.GameOverPage
{
    internal class GameOverViewModel : ViewModelBase
    {
        private string _movesMade;
        private TimeSpan _time;

        public GameOverViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
            : base(eventAggregator, navigationService)
            
        {
            OpenMainMenuCommand = new DelegateCommand(OpenMainMenu);
            EventAggregator.GetEvent<GameFinishedEvent>().Subscribe(UpdateStats);
        }

        /// <summary>
        /// Command for open MainMenu
        /// </summary>
        public ICommand OpenMainMenuCommand { get; set; }

        /// <summary>
        /// Gets the moves count made by the player
        /// </summary>
        public string MovesMade
        {
            get => _movesMade;
            private set
            {
                _movesMade = value;
                RaisePropertyChanged(nameof(MovesMade));
            }
        }

        /// <summary>
        /// Gets the player time to solve the puzzle.
        /// </summary>
        public TimeSpan Time
        {
            get => _time;
            private set
            {
                _time = value;
                RaisePropertyChanged(nameof(Time));
            }

        }

        /// <summary>
        /// Updates the game stats displayed at the end of the game.
        /// </summary>
        /// <param name="gameFinished">The  event received at the game end.</param>
        private void UpdateStats(GameFinishedEvent gameFinished)
        {
            MovesMade = gameFinished.MovesCount.ToString();
            Time = gameFinished.ElapsedTime;
        }

        /// <summary>
        /// Opens main menu.
        /// </summary>
        private void OpenMainMenu()
        {
            NavigationService.ShowPage(AppPages.MainMenuPage);
        }
    }
}
