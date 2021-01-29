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

        public int _rowsCount;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The animation duration in ms used when two tiles are exchanged.
        /// </summary>
        private const short AnimationDuration = 300;

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

        private Tile blankTile;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Intializes a new Puzzle Slider control.
        /// </summary>
        public PuzzleSlider()
        {
            _tagToPositionMap = new Dictionary<Tile, int>();
            State = GetEmptyBoard();
            InitSlider();
            StateChangedEvent += OnNewState;
            TileChangedEvent += OnNewTileSize;
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

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Gets the empty board with no tiles.
        /// </summary>
        /// <returns></returns>
        private static ObservableBoard GetEmptyBoard()
        {
            return new ObservableBoard(new byte[,] { });
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

        private void AnimateInDirection(int animationDirection)
        {
            var blankPosition = _tagToPositionMap[blankTile];
            Tile otherTile = null;

            foreach (var keyPair in _tagToPositionMap)
            {
                if (keyPair.Value == blankPosition + animationDirection)
                {
                    otherTile = keyPair.Key;
                }
            }
            if (otherTile != null)
            {
                AnimateSwitching(otherTile);
            }
        }

        /// <summary>
        /// Animate the tile exchanging between the blank tile and the specified tile.
        /// </summary>
        /// <param name="valueTile"></param>
        /// <param name="index"></param>
        private void AnimateSwitching(Tile valueTile, int index = 0)
        {
            var tileAnimation = new ThicknessAnimation
            {
                From = valueTile.DestinationMargin,
                To = blankTile.DestinationMargin,
                Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration))
            };

            var blankAnimation = new ThicknessAnimation
            {
                From = blankTile.DestinationMargin,
                To = valueTile.DestinationMargin,
                Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration))
            };

            ExchangeThickness(blankTile, valueTile);
            ExchangePositions(blankTile, valueTile);

            tileAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, AnimationDuration * index);
            blankAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, AnimationDuration * index);
            Storyboard.SetTarget(blankAnimation, blankTile);
            Storyboard.SetTargetProperty(blankAnimation, new PropertyPath(Tile.MarginProperty));
            Storyboard.SetTarget(tileAnimation, valueTile);
            Storyboard.SetTargetProperty(tileAnimation, new PropertyPath(Tile.MarginProperty));

            SetupStoryBoard(blankAnimation, tileAnimation);
        }

        /// <summary>
        /// Creates a new tile for the puzzler.
        /// </summary>
        /// <param name="tileIndex">The index of the tile.</param>
        /// <returns>a new tile with the specified index</returns>
        private Tile BuildTile(int tileIndex)
        {
            var tileColumn = tileIndex % _rowsCount;
            var tileRow = tileIndex / _rowsCount;
            var tileTag = State[tileIndex];

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
            State = (ObservableBoard)e.NewValue ?? GetEmptyBoard();
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
        /// Triggered when a tile is clicked.
        /// </summary>
        private void OnTileClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedTile = sender as Tile;
            if (IsMovePossible(clickedTile))
            {
                AnimateSwitching(clickedTile);
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
        /// <param name="blankAnimation">The animation of the blank tile moving into its new positon.</param>
        /// <param name="valueTileAnimation">The animation of a tag tile moving into its new position.</param>
        private void SetupStoryBoard(ThicknessAnimation blankAnimation, ThicknessAnimation valueTileAnimation)
        {
            // Create a Storyboard to contain and apply the animation.
            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.AutoReverse = false;
            pathAnimationStoryboard.Children.Add(valueTileAnimation);
            pathAnimationStoryboard.Children.Add(blankAnimation);
            pathAnimationStoryboard.Begin(this);
        }

        #endregion Private Methods
    }
}