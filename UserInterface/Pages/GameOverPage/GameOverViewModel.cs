using BonusSystem;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UserInterface.BootstraperSpace;
using UserInterface.Events;
using UserInterface.Helpers;

namespace UserInterface.Pages.GameOverPage
{
  internal class GameOverViewModel : ViewModelBase
  {
    #region Constants
    /// <summary>
    /// Duration in ms for counting player score.
    /// The player score will be raised from 0 to its final score in this period of time.
    /// </summary>
    private const int CountAnimationDuration = 750;

    #endregion

    #region Private Fields

    private readonly IScoringSystem _scoringSystem;
    private DispatcherTimer _countingTimer;
    private string _movesMade;
    private ushort _playerScore;
    private ushort _pointsStep;
    private ushort _finalScore;

    private Visibility _firstStarVisibility;
    private Visibility _secondStarVisibility;
    private Visibility _thirdStarVisibiliy;

    private TimeSpan _time;

    #endregion Private Fields

    #region Public Constructors

    public GameOverViewModel(IEventAggregator eventAggregator, INavigationService navigationService, IScoringSystem scoringSystem)
        : base(eventAggregator, navigationService)

    {
      _scoringSystem = scoringSystem;
      OpenMainMenuCommand = new DelegateCommand(OpenMainMenu);
      ConfigureCountingTimer();

      EventAggregator.GetEvent<GameFinishedEvent>().Subscribe(UpdateStats);
    }

    #endregion Public Constructors

    #region Public Properties

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
    /// Command for open MainMenu
    /// </summary>
    public ICommand OpenMainMenuCommand { get; set; }

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
    /// Gets the visibility for the first golden star displayed.
    /// </summary>
    public Visibility FirstStarVisibility
		{
      get => _firstStarVisibility;
      private set 
      {
        _firstStarVisibility = value;
        RaisePropertyChanged(nameof(FirstStarVisibility));
      }
		}

    /// <summary>
    /// Gets the visibiliy for the second golden star displayed.
    /// </summary>
    public Visibility SecondStarVisibility
    {
      get => _secondStarVisibility;
      set
      {
        _secondStarVisibility = value;
        RaisePropertyChanged(nameof(SecondStarVisibility));
      }
    }

    /// <summary>
    /// Gets the visibility for the third golden star displayed.
    /// </summary>
    public Visibility ThirdStarVisibility
    {
      get => _thirdStarVisibiliy;
      set
      {
        _thirdStarVisibiliy = value;
        RaisePropertyChanged(nameof(ThirdStarVisibility));
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

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Called when the page is displayed
    /// </summary>
    public override void OnDisplayed()
    {
      StarCountingAnimation();
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Configure the timer used for counting animation.
    /// At each tick, the player score will be increased by a certain value.
    /// </summary>
    private void ConfigureCountingTimer()
    {
      _countingTimer = new DispatcherTimer();
      _countingTimer.Interval = TimeSpan.FromMilliseconds(15);
      _countingTimer.Tick += OnCountingTick;
    }

    /// <summary>
    /// Increases the player score
    /// </summary>
    /// <param name="sender">the sender object [not use]</param>
    /// <param name="e">the event args [not use] </param>
    private void OnCountingTick(object sender, EventArgs e)
    {
      PlayerScore += _pointsStep;
      if (Math.Abs(PlayerScore - _finalScore) <= _pointsStep)
      {
        PlayerScore = _finalScore;
        _countingTimer.Stop();
      }
    }

    /// <summary>
    /// Opens main menu.
    /// </summary>
    private void OpenMainMenu()
    {
      NavigationService.ShowPage(AppPages.MainMenuPage);
    }

    /// <summary>
    /// Stars the couting animation for the player score.
    /// </summary>
    private void StarCountingAnimation()
    {
      PlayerScore = 0;
      _pointsStep = GetScoreStepForAnimation(_finalScore);
      _countingTimer.Start();
    }

    /// <summary>
    /// Updates the game stats displayed at the end of the game.
    /// </summary>
    /// <param name="gameFinished">The  event received at the game end.</param>
    private void UpdateStats(GameFinishedEvent gameFinished)
    {
      MovesMade = gameFinished.MovesCount.ToString();
      Time = gameFinished.ElapsedTime;

      _finalScore = _scoringSystem.GetPlayerScore(
                              gameFinished.PuzzleRows,
                              gameFinished.MovesCount,
                              gameFinished.MinMoves,
                              (int)gameFinished.ElapsedTime.TotalSeconds);

      var starsCount = _scoringSystem.GetStarCount(_finalScore);

      HideAllStars();
      DisplayStars(starsCount);
    }

    /// <summary>
    /// Displays stars proportional to the input number.
    /// </summary>
    /// <param name="starCount">the number of golder stars that will be displayed to the user</param>
    private void DisplayStars(int starCount)
    {
      if (starCount >= 1)
      {
				FirstStarVisibility = Visibility.Visible;
      }

      if (starCount >= 2)
      {
				SecondStarVisibility = Visibility.Visible;
      }

      if (starCount >= 3)
      {
				ThirdStarVisibility = Visibility.Visible;
      }
    }

    /// <summary>
    /// Set all golden star on Collapsed.
    /// </summary>
    private void HideAllStars()
    {
      FirstStarVisibility = Visibility.Collapsed;
      SecondStarVisibility = Visibility.Collapsed;
      ThirdStarVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Gets the score step wich will be added to the player score at each tick during the animation.
    /// </summary>
    /// <param name="score">the final score of the player</param>
    /// <returns>an ushort number represeting the score step</returns>
    private ushort GetScoreStepForAnimation(int score)
		{
      return (ushort)(score * _countingTimer.Interval.TotalMilliseconds / CountAnimationDuration);
    }
   
    #endregion Private Methods
  }
}
