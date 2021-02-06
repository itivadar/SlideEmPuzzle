using SliderPuzzleSolver;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
        #region Public Fields

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

        /// <summary>
        /// Determines if the puzzle is locked or not.
        /// If the puzzle is locked the user cannot move tiles.
        /// </summary>
        public static readonly DependencyProperty SolutionStepsProperty =
             DependencyProperty.RegisterAttached(nameof(SolutionSteps),
             typeof(IEnumerable<SlideDirection>),
             typeof(PuzzleSlider),
             new FrameworkPropertyMetadata(null,
                 new PropertyChangedCallback(OnSolutionStepsChanged)));

        public int _rowsCount;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The animation duration in ms used when two tiles are exchanged by the player
        /// </summary>
        private const short PlayerAnimationDuration = 300;

        /// <summary>
        /// The animation duration in ms used when two tiles are exchanged when the autosolving is enabled.
        /// </summary>
        private const short AutomaticAnimationDuration = 470;

        /// <summary>
        /// The tag associated with the blank tile.
        /// </summary>
        private const byte BlankTileTag = 0;

        /// <summary>
        /// The default size of the tiles (height and width).
        /// </summary>
        private const short DefaultTileSize = 100;

        /// <summary>
        /// The margin between two tiles.
        /// </summary>
        private const byte Spacing = 5;

        private readonly Dictionary<Tile, int> _tagToPositionMap;

        // Create a Storyboard to contain and apply the animation.
        private readonly Storyboard _animationStoryboard;

        private Tile blankTile;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Intializes a new Puzzle Slider control.
        /// </summary>
        public PuzzleSlider()
        {
            _tagToPositionMap = new Dictionary<Tile, int>();
            _animationStoryboard = new Storyboard
            {
                AutoReverse = false
            };

            State = GetDefaultBoard();
            InitSlider();
            StateChangedEvent += OnNewState;
            TileChangedEvent += OnNewTileSize;
            SolutionStepsChangeEvent += OnSolutionStepsChangeEvent;
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Triggered when the puzzle state changes.
        /// </summary>
        public static event DependencyPropertyChanged StateChangedEvent;

        /// <summary>
        /// Triggered when the locking property is changed.
        /// </summary>
        public static event DependencyPropertyChanged TileChangedEvent;

        /// <summary>
        /// Triggered when the solution steps are changed.
        /// </summary>
        public static event DependencyPropertyChanged SolutionStepsChangeEvent;

        #endregion Public Events

        #region Public Properties

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
                InitSlider();
                IsEnabled = true;
            }
        }

        /// <summary>
        /// Gets or set the tils size.
        /// </summary>
        public short TileSize
        {
            get => (short)GetValue(TileSizeProperty);
            set => SetValue(TileSizeProperty, value);
        }

        /// <summary>
        /// Gets or set the tils size.
        /// </summary>
        public IEnumerable<SlideDirection> SolutionSteps
        {
            get => (IEnumerable<SlideDirection>)GetValue(SolutionStepsProperty);
            set => SetValue(SolutionStepsProperty, value);
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
        /// Triggered  when the tile size has been changed.
        /// </summary>
        private static void OnSolutionStepsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SolutionStepsChangeEvent?.Invoke(sender, args);
        }

        /// <summary>
        /// Plays a sliding animation of moving the blank tile in a certain direction.
        /// </summary>
        /// <param name="moveDirection">the direction in which the animation is played.</param>
        /// <param name="index">index of the animation if multe animations needs to be played sequentially </param>
        private void AnimateInDirection(SlideDirection moveDirection, int index)
        {
            int positionDelta = -1;

            if (moveDirection == SlideDirection.Down) positionDelta = _rowsCount;
            if (moveDirection == SlideDirection.Up) positionDelta = -_rowsCount;
            if (moveDirection == SlideDirection.Right) positionDelta = 1;

            var blankPosition = _tagToPositionMap[blankTile];
            Tile otherTile = null;

            foreach (var keyPair in _tagToPositionMap)
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
            var beginTime = TimeSpan.FromMilliseconds(duration * index);

            var tileAnimation = CreateSlidingAnimationForTile(valueTile, blankTile.DestinationMargin, beginTime);
            var blankAnimation = CreateSlidingAnimationForTile(blankTile, valueTile.DestinationMargin, beginTime);

            _animationStoryboard.Children.Add(tileAnimation);
            _animationStoryboard.Children.Add(blankAnimation);

            ExchangeThickness(blankTile, valueTile);
            ExchangePositions(blankTile, valueTile);
        }

        /// <summary>
        /// Creates sliding animation for a specific tile.
        /// </summary>
        /// <param name="tile">the tile which need to be animated.</param>
        /// <param name="toThickness">destination thickness after the animation is done.</param>
        /// <param name="beginTime">time when the animation will begin</param>
        /// <returns></returns>
        private ThicknessAnimation CreateSlidingAnimationForTile(Tile tile, Thickness toThickness, TimeSpan beginTime)
        {
            var tileAnimation = new ThicknessAnimation
            {
                From = tile.DestinationMargin,
                To = toThickness,
                Duration = new Duration(TimeSpan.FromMilliseconds(PlayerAnimationDuration))
            };

            tileAnimation.BeginTime = beginTime;
            Storyboard.SetTarget(tileAnimation, tile);
            Storyboard.SetTargetProperty(tileAnimation, new PropertyPath(MarginProperty));

            return tileAnimation;
        }

        /// <summary>
        /// Creates a new tile for the puzzler
        /// </summary>
        /// <param name="tileIndex">The index of the tile.</param>
        /// <returns>a new tile with the specified index</returns>
        private Tile BuildTile(int tileIndex)
        {
            var tileColumn = tileIndex % _rowsCount;
            var tileRow = tileIndex / _rowsCount;
            var tileTag = State[tileRow, tileColumn];

            var sliderTile = new Tile
            {
                Width = TileSize,
                Height = TileSize,
                TileTag = tileTag,
                IsBlankTile = tileTag == BlankTileTag,
                Margin = new Thickness(tileColumn * (TileSize + Spacing), tileRow * (TileSize + Spacing), 0, 0),
                DestinationMargin = new Thickness(tileColumn * (TileSize + Spacing), tileRow * (TileSize + Spacing), 0, 0),
            };

            if (tileTag == BlankTileTag)
            {
                blankTile = sliderTile;
            }

            //react to user's click
            sliderTile.PreviewMouseLeftButtonDown += OnTileClicked;

            return sliderTile;
        }

        /// <summary>
        /// Exchanges positions between two given tiles.
        /// </summary>
        /// <param name="blankTile"></param>
        /// <param name="valueTile"></param>
        private void ExchangePositions(Tile blankTile, Tile valueTile)
        {
            var blankPosition = _tagToPositionMap[blankTile];
            var valuePosition = _tagToPositionMap[valueTile];

            //exchange position in the mapping
            _tagToPositionMap[blankTile] = valuePosition;
            _tagToPositionMap[valueTile] = blankPosition;

            //move the blank tile to the new position
            State.MoveBlankTile(valuePosition, blankPosition);
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
        /// Intializes the slider controls.
        /// Created the tiles on specifed positions.
        /// </summary>
        private void InitSlider()
        {
            Children.Clear();          
            _tagToPositionMap.Clear();
            _rowsCount = State.Rows;

            for (int tileIndex = 0; tileIndex < _rowsCount * _rowsCount; tileIndex++)
            {
                var sliderTile = BuildTile(tileIndex);
                Children.Add(sliderTile);
                _tagToPositionMap.Add(sliderTile, tileIndex);
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
            var tilePosition = _tagToPositionMap[sliderTile];
            var blankPosition = _tagToPositionMap[blankTile];
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
            InitSlider();
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

            PlaySlidingAnimationForSteps(args.NewValue as IEnumerable<SlideDirection>);
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
        /// Keeps enabled only tiles the playes can switch them.
        /// </summary>
        private void DisableImposibleMoves()
        {
            foreach (var tile in _tagToPositionMap.Keys)
            {
                tile.IsEnabled = false;
                if (IsMovePossible(tile) || tile == blankTile)
                {
                    tile.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Setup a storyboard to play animations.
        /// </summary>
        private void InitializeStoryboard()
        {
            _animationStoryboard.Children.Clear();
        }

        /// <summary>
        /// Begin to play the animations added to the storyboard.
        /// </summary>
        private void BeginAnimations()
        {
            _animationStoryboard.Begin(this);
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
        /// Plays the animations following the steps.
        /// </summary>
        /// <param name="steps">an IEnumerable of steps required to solve the puzzel</param>
        private void PlaySlidingAnimationForSteps(IEnumerable<SlideDirection> steps)
        {
            if (steps is null) return;
            int stepCount = 0;
            IsEnabled = false;
            InitializeStoryboard();
            foreach (var step in steps)
            {
                AnimateInDirection(step, stepCount++);
            }
            BeginAnimations();
        }

        #endregion Private Methods
    }
}