using BonusSystem;
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
    private ushort _playerScore;
    private readonly IScoringSystem _scoringSystem;

    public GameOverViewModel(
        IEventAggregator eventAggregator, INavigationService navigationService, IScoringSystem scoringSystem)
        : base(eventAggregator, navigationService)

    {
      _scoringSystem = scoringSystem;
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
    /// The score obtained by the player after solving the puzzle.
    /// </summary>
    public ushort PlayerScore
    {
      get => _playerScore;
      private set
      {
        _playerScore = value;
        RaisePropertyChanged(nameof(PlayerScore));
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
      PlayerScore = _scoringSystem.GetPlayerScore(
                      gameFinished.PuzzleRows,
                      gameFinished.MovesCount,
                      gameFinished.MinMoves,
                      (int)gameFinished.ElapsedTime.TotalSeconds);
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
