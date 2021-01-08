using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;

namespace UserInterface.Pages.GameOverPage
{
    internal class GameOverViewModel : ViewModelBase
    {
        private string _movesMade;

        public GameOverViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
            : base(eventAggregator, navigationService)
            
        {
            EventAggregator.GetEvent<GameFinishedEvent>().Subscribe(UpdateStats);
        }

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

        private void UpdateStats(GameFinishedEvent gameFinished)
        {
            MovesMade = gameFinished.MovesCount.ToString();   
        }
    }
}
