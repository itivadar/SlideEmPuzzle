using SliderPuzzleSolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UserInterface.Pages.SliderPage;

namespace UserInterface.CustomControls
{
	/// <summary>
	/// The delegate for defining events related to the dependecy property changing value.
	/// </summary>
	/// <param name="sender"> The objects which triggered the property changed event. </param>
	/// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/>of the event</param>
	public delegate void DependencyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args);

	public class PuzzleSlider : Canvas
	{

		#region Public Events
		/// <summary>
		/// Event triggered when all the animations are stopped.
		/// </summary>
		public event EventHandler<EventArgs> AnimationChangedEvent;

		#endregion Public Events

		#region Public Fields

		/// <summary>
		/// Determines if the puzzle is locked or not.
		/// If the puzzle is locked the user cannot move tiles.
		/// </summary>
		public static readonly DependencyProperty SolutionStepsProperty =
				 DependencyProperty.RegisterAttached(nameof(SolutionSteps),
				 typeof(IReadOnlyCollection<SlideDirection>),
				 typeof(PuzzleSlider),
				 new FrameworkPropertyMetadata(null,
						FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
						 new PropertyChangedCallback(OnSolutionStepsChanged)));

		/// <summary>
		/// The state of the puzzle representing an <see cref="ObservableBoard"/>
		/// Defines the positions of the tiles.
		/// </summary>
		public static readonly DependencyProperty StateProperty =
				DependencyProperty.RegisterAttached(nameof(State),
				typeof(ObservableBoard),
				typeof(PuzzleSlider),
				new FrameworkPropertyMetadata(null,
						FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
						new PropertyChangedCallback(OnStateChanged)));

		/// <summary>
		/// Determines if the puzzle is locked or not.
		/// If the puzzle is locked the user cannot move tiles.
		/// </summary>
		public static readonly DependencyProperty TileSizeProperty =
				 DependencyProperty.RegisterAttached(nameof(TileSize),
				 typeof(short),
				 typeof(PuzzleSlider),
				 new FrameworkPropertyMetadata(DefaultTileSize,
						 FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
						 new PropertyChangedCallback(OnTileSizeChanged)));

		#endregion Public Fields

		#region Private Fields

		/// <summary>
		/// The animation duration in ms used when two tiles are exchanged when the autosolving is enabled.
		/// </summary>
		private const short AutomaticAnimationDuration = 500;

		/// <summary>
		/// The tag associated with the blank tile.
		/// </summary>
		private const byte BlankTileTag = 0;

		/// <summary>
		/// The default size of the tiles (height and width).
		/// </summary>
		private const short DefaultTileSize = 100;

		/// <summary>
		/// The animation duration in ms used when two tiles are exchanged by the player
		/// </summary>
		private const short PlayerAnimationDuration = 300;

		/// <summary>
		/// The margin between two tiles.
		/// </summary>
		private const byte Spacing = 5;

		/// <summary>
		/// Media player used for sounds
		/// </summary>
		private static readonly MediaPlayer _mediaPlayer = new MediaPlayer();

		// Create a Storyboard to contain and apply the animation.
		private readonly Storyboard _animationStoryboard;

		/// <summary>
		/// Set of tiles whose margin will be updated to the final position after the animation is ended.
		/// </summary>
		private readonly HashSet<Tile> _dirtyTilesMargin;

		/// <summary>
		/// Maps a tile to its position in the board as a number from 0 to 15.
		/// </summary>
		private readonly Dictionary<Tile, int> _tileToPositionMap;

		/// <summary>
		/// Reference to the blank tile.
		/// </summary>
		private Tile _blankTile;

		/// <summary>
		/// The number of rows of the board (equal to the numbers of columns)
		/// </summary>
		private int _rowsCount;

		/// <summary>
		/// Queue containing the moves that will be played when the animation is played.
		/// </summary>
		private Queue<(int, int)> _movesQueue;

		#endregion Private Fields

		#region Public Constructors

		/// <summary>
		/// Intializes a new Puzzle Slider control.
		/// </summary>
		public PuzzleSlider()
		{
			_tileToPositionMap = new Dictionary<Tile, int>();
			_dirtyTilesMargin = new HashSet<Tile>();
			_movesQueue = new Queue<(int, int)>();

			_animationStoryboard = new Storyboard
			{
				AutoReverse = false,
				FillBehavior = FillBehavior.Stop,
				SlipBehavior = SlipBehavior.Grow
			};

			State = GetDefaultBoard();
			InitSlider();

			//Register to events
			StateChangedEvent += OnNewState;
			TileChangedEvent += OnNewTileSize;
			SolutionStepsChangeEvent += OnSolutionStepsChangeEvent;
			_animationStoryboard.Completed += SlidingAnimationsCompleted;
		}

		/// <summary>
		/// Called when activating each sliding animation.
		/// </summary>
		private void OnAnimationActivated(object sender, EventArgs e)
		{
			var clock = (AnimationClock)sender;
			if (clock.CurrentState == ClockState.Active)
			{
				PlaySound();
				MakeQueuedMove();
			}
		}

		#endregion Public Constructors

		#region Public Events

		/// <summary>
		/// Triggered when the solution steps are changed.
		/// </summary>
		public static event DependencyPropertyChanged SolutionStepsChangeEvent;

		/// <summary>
		/// Triggered when the puzzle state changes.
		/// </summary>
		public static event DependencyPropertyChanged StateChangedEvent;

		/// <summary>
		/// Triggered when the locking property is changed.
		/// </summary>
		public static event DependencyPropertyChanged TileChangedEvent;

		#endregion Public Events

		#region Public Properties

		/// <summary>
		/// Gets or set the tils size.
		/// </summary>
		public IReadOnlyCollection<SlideDirection> SolutionSteps
		{
			get => (IReadOnlyCollection<SlideDirection>)GetValue(SolutionStepsProperty);
			set => SetValue(SolutionStepsProperty, value);
		}

		/// <summary>
		/// Gets or sets the slider state
		/// Every element of state[i] represents the tag of the tile at position i.
		/// Element 0 is for blank tile
		/// </summary>
		public ObservableBoard State
		{
			get { return (ObservableBoard)GetValue(StateProperty); }
			set
			{
				SetValue(StateProperty, value);
				StopAllAnimations();
				InitSlider();
			}
		}

		/// <summary>
		/// Gets or set the tils size.
		/// </summary>
		public short TileSize
		{
			get => (short)GetValue(TileSizeProperty);
			set
			{
				SetValue(TileSizeProperty, value);
				ResizeTiles();
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// Gets the default board.
		/// Its a solved 2x2 board
		/// </summary>
		/// <returns></returns>
		private static ObservableBoard GetDefaultBoard()
		{
			return new ObservableBoard("1 2 3 0");
		}

		/// <summary>
		/// Triggered  when the tile size has been changed.
		/// </summary>
		private static void OnSolutionStepsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			SolutionStepsChangeEvent?.Invoke(sender, args);
		}

		/// <summary>
		/// Triggered when the state has been changed.
		/// </summary>
		private static void OnStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			StateChangedEvent?.Invoke(sender, args);
		}

		/// <summary>
		/// Triggered  when the tile size has been changed.
		/// </summary>
		private static void OnTileSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			TileChangedEvent?.Invoke(sender, args);
		}

		/// <summary>
		/// Plays a sliding animation of moving the blank tile in a certain direction.
		/// </summary>
		/// <param name="moveDirection">the direction in which the animation is played.</param>
		/// <param name="index">index of the animation if multe animations needs to be played sequentially </param>
		private void AddAnimationForDirection(SlideDirection moveDirection, int index)
		{
			int positionDelta = -1;

			if (moveDirection == SlideDirection.Down) positionDelta = _rowsCount;
			if (moveDirection == SlideDirection.Up) positionDelta = -_rowsCount;
			if (moveDirection == SlideDirection.Right) positionDelta = 1;

			var blankPosition = _tileToPositionMap[_blankTile];
			Tile otherTile = null;

			foreach (var keyPair in _tileToPositionMap)
			{
				if (keyPair.Value == blankPosition + positionDelta)
				{
					otherTile = keyPair.Key;
				}
			}
			if (otherTile != null)
			{
				AddAnimationsToStoryBoard(otherTile, index, AutomaticAnimationDuration);
			}
		}

		/// <summary>
		/// Animate the tile exchanging between the blank tile and the specified tile.
		/// </summary>
		/// <param name="valueTile"></param>
		/// <param name="index"></param>
		private void AddAnimationsToStoryBoard(Tile valueTile, int index = 0, int duration = PlayerAnimationDuration)
		{
			var beginTime = TimeSpan.FromMilliseconds((duration + 50) * index);

			var tileAnimation = CreateSlidingAnimation(valueTile, _blankTile.DestinationMargin, beginTime, duration);
			var blankAnimation = CreateSlidingAnimation(_blankTile, valueTile.DestinationMargin, beginTime, duration);

			_animationStoryboard.Children.Add(tileAnimation);
			_animationStoryboard.Children.Add(blankAnimation);
			tileAnimation.CurrentStateInvalidated += OnAnimationActivated;

			_dirtyTilesMargin.Add(valueTile);
			_dirtyTilesMargin.Add(_blankTile);

			ExchangeThickness(_blankTile, valueTile);
			ExchangePositions(_blankTile, valueTile);
		}

		/// <summary>
		/// Begin to play the animations added to the storyboard.
		/// </summary>
		private void BeginAnimations()
		{
			_animationStoryboard.Begin();
		}

		/// <summary>
		/// Creates a new tile for the puzzler
		/// </summary>
		/// <param name="tilePosition">The position of the tile.</param>
		/// <returns>a new tile with the specified index</returns>
		private Tile BuildTileAtPositin(int tilePosition)
		{
			var tileColumn = tilePosition % _rowsCount;
			var tileRow = tilePosition / _rowsCount;
			var tileTag = State[tileRow, tileColumn];

			var sliderTile = new Tile
			{
				Width = TileSize,
				Height = TileSize,
				TileTag = tileTag,
				IsBlankTile = tileTag == BlankTileTag,
				Margin = GetMarginForTileIndex(tilePosition),
				DestinationMargin = GetMarginForTileIndex(tilePosition),
			};

			if (tileTag == BlankTileTag)
			{
				//the blank tile will be drawn below other tiles
				SetZIndex(sliderTile, -1);
				_blankTile = sliderTile;
			}

			//react to user's click
			sliderTile.PreviewMouseLeftButtonDown += OnTileClicked;

			return sliderTile;
		}

		/// <summary>
		/// Creates sliding animation for a specific tile.
		/// </summary>
		/// <param name="tile">the tile which need to be animated.</param>
		/// <param name="toThickness">destination thickness after the animation is done.</param>
		/// <param name="beginTime">time when the animation will begin</param>
		/// <returns></returns>
		private ThicknessAnimation CreateSlidingAnimation(Tile tile, Thickness toThickness, TimeSpan beginTime, int duration)
		{
			var tileAnimation = new ThicknessAnimation
			{
				From = tile.DestinationMargin,
				To = toThickness,
				Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
				FillBehavior = FillBehavior.HoldEnd,
				BeginTime = beginTime,
			};

			Storyboard.SetTarget(tileAnimation, tile);
			Storyboard.SetTargetProperty(tileAnimation, new PropertyPath(MarginProperty));

			return tileAnimation;
		}

		/// <summary>
		/// Keeps enabled only tiles the playes can switch them.
		/// </summary>
		private void DisableImposibleMoves()
		{
			foreach (var tile in _tileToPositionMap.Keys)
			{
				tile.IsEnabled = false;
				if (IsMovePossible(tile) || tile == _blankTile)
				{
					tile.IsEnabled = true;
				}
			}
		}

		/// <summary>
		/// Exchanges positions between two given tiles.
		/// </summary>
		/// <param name="blankTile"></param>
		/// <param name="valueTile"></param>
		private void ExchangePositions(Tile blankTile, Tile valueTile)
		{
			var blankPosition = _tileToPositionMap[blankTile];
			var valuePosition = _tileToPositionMap[valueTile];

			//exchange position in the mapping
			_tileToPositionMap[blankTile] = valuePosition;
			_tileToPositionMap[valueTile] = blankPosition;

			//queue the move to played in sync with animation
			_movesQueue.Enqueue((valuePosition, blankPosition));
			
		}

		/// <summary>
		/// Exchanges thickness between two tiles. Used on animating.
		/// </summary>
		private void ExchangeThickness(Tile blankTile, Tile valueTile)
		{
			var tempThickness = blankTile.DestinationMargin;
			blankTile.DestinationMargin = valueTile.DestinationMargin;
			valueTile.DestinationMargin = tempThickness;
		}

		/// <summary>
		/// Calculates the margin needed to display a tile on a certain position.
		/// </summary>
		/// <param name="tilePosition">the tile position in the board</param>
		/// <returns>a <see cref="Thickness"/> object  need to place the tile at a certain position /></returns>
		private Thickness GetMarginForTileIndex(int tilePosition)
		{
			var tileColumn = tilePosition % State.Rows;
			var tileRow = tilePosition / State.Rows;

			return new Thickness(tileColumn * (TileSize + Spacing), tileRow * (TileSize + Spacing), 0, 0);
		}

		/// <summary>
		/// Opens the file for the sliding sound
		/// </summary>
		private void InitializeMediaPlayer()
		{
			_mediaPlayer.Open(new Uri(Directory.GetCurrentDirectory() + "/Resources/Sounds/slidingSound.wav", UriKind.Absolute));
			_mediaPlayer.Volume = 0.2;
		}

		/// <summary>
		/// Setup a storyboard to play animations.
		/// </summary>
		private void InitializeStoryboard()
		{
			_animationStoryboard.Children.Clear();
			_movesQueue.Clear();
			InitializeMediaPlayer();
		}

		/// <summary>
		/// Intializes the slider controls.
		/// Created the tiles on specifed positions.
		/// </summary>
		private void InitSlider()
		{
			Children.Clear();
			_tileToPositionMap.Clear();
			_rowsCount = State.Rows;

			for (int tilePosition = 0; tilePosition < _rowsCount * _rowsCount; tilePosition++)
			{
				var sliderTile = BuildTileAtPositin(tilePosition);
				Children.Add(sliderTile);
				_tileToPositionMap.Add(sliderTile, tilePosition);
			}

			DisableImposibleMoves();
		}

		/// <summary>
		/// Determines if the given tile can be exchanged with the blank tile.
		/// </summary>
		/// <param name="sliderTile"></param>
		/// <returns></returns>
		private bool IsMovePossible(Tile sliderTile)
		{
			var tilePosition = _tileToPositionMap[sliderTile];
			var blankPosition = _tileToPositionMap[_blankTile];
			var tileRow = tilePosition / _rowsCount;
			var blankRow = blankPosition / _rowsCount;

			if (Math.Abs(tilePosition - blankPosition) == _rowsCount ||  //the tiles have to be on the same colomn
				(Math.Abs(tilePosition - blankPosition) == 1 && tileRow == blankRow)) //the tiles have to be adjacent
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Handles the state changing event.
		/// </summary>
		private void OnNewState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if ((PuzzleSlider)sender != this)
			{
				return;
			}
			State = (ObservableBoard)e.NewValue ?? GetDefaultBoard();
		}

		/// <summary>
		/// Handles the tile changing event.
		/// </summary>
		private void OnNewTileSize(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			if ((PuzzleSlider)sender != this)
			{
				return;
			}

			TileSize = (short?)args.NewValue ?? DefaultTileSize;
		}

		/// <summary>
		/// Handles the solutions steps event.
		/// </summary>
		private void OnSolutionStepsChangeEvent(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			if ((PuzzleSlider)sender != this)
			{
				return;
			}

			PlaySlidingAnimationForSteps(args.NewValue as IReadOnlyCollection<SlideDirection>);
		}

		/// <summary>
		/// Triggered when a tile is clicked.
		/// </summary>
		private void OnTileClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var clickedTile = sender as Tile;
			if (IsMovePossible(clickedTile))
			{
				PlaySlidingAnimationForTile(clickedTile);
				DisableImposibleMoves();
			}
		}

		/// <summary>
		/// Plays the animations following the steps.
		/// </summary>
		/// <param name="steps">an IEnumerable of steps required to solve the puzzel</param>
		private void PlaySlidingAnimationForSteps(IReadOnlyCollection<SlideDirection> steps)
		{
			if (steps is null) return;

			int stepCount = 0;
			IsEnabled = false;
			InitializeStoryboard();
			foreach (var step in steps)
			{
				AddAnimationForDirection(step, stepCount++);
			}
			BeginAnimations();
		}

		/// <summary>
		/// Plays animation for sliding a value tile on the blank position.
		/// </summary>
		/// <param name="clickedTile">the tile which needs to be animated</param>
		private void PlaySlidingAnimationForTile(Tile clickedTile)
		{
			InitializeStoryboard();
			AddAnimationsToStoryBoard(clickedTile);
			BeginAnimations();
		}

		/// <summary>
		/// Plays a sliding sound
		/// </summary>
		private void PlaySound()
		{
			_mediaPlayer.Position = TimeSpan.Zero;
			_mediaPlayer.Play();
		}

		/// <summary>
		/// Resize the tiles.
		/// </summary>
		private void ResizeTiles()
		{
			if (_dirtyTilesMargin.Count > 0) return;
			foreach (Tile tile in Children)
			{
				var tilePosition = _tileToPositionMap[tile];
				tile.Height = TileSize;
				tile.Width = TileSize;
				tile.Margin = GetMarginForTileIndex(tilePosition);
				tile.DestinationMargin = GetMarginForTileIndex(tilePosition);
			}
		}

		/// <summary>
		/// Sets the correct margins for tile which were animated.
		/// </summary>
		private void SlidingAnimationsCompleted(object sender, EventArgs e)
		{
			foreach (Tile tile in _dirtyTilesMargin)
			{
				tile.Margin = tile.DestinationMargin;
			}
			_dirtyTilesMargin.Clear();
			RaiseAnimationChangedEvent();
		}

		/// <summary>
		/// Stops all the animations
		/// </summary>
		private void StopAllAnimations()
		{
			_mediaPlayer.Stop();
			_animationStoryboard.Stop();
			_animationStoryboard.Children.Clear();
			_dirtyTilesMargin.Clear();
			_movesQueue.Clear();
		}

		/// <summary>
		/// Raises the Animation changed event.
		/// </summary>
		private void RaiseAnimationChangedEvent()
		{
			AnimationChangedEvent?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// The State Board is updated in sync with the animations.
		/// The moves are queued until the animations is played.
		/// </summary>
		private void MakeQueuedMove()
		{
			(int valuePosition, int blankPosition) = _movesQueue.Dequeue();
			//move the blank tile to the new position
			State.MoveBlankTile(valuePosition, blankPosition);
		}

		#endregion Private Methods
	}
}