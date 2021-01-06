using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
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
   /// <param name="d"></param>
   /// <param name="e"></param>
    public delegate void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
    public class PuzzleSlider : Canvas
    {
        /// <summary>
        /// The state of the puzzle representing an <see cref="ObservableBoard"/>
        /// Defines the positions of the tiles. 
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(nameof(State),
                                                                                              typeof(ObservableBoard),
                                                                                              typeof(PuzzleSlider),
                                                                                              new FrameworkPropertyMetadata(null,
                                                                                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                new PropertyChangedCallback(OnStateChanged)));
       /// <summary>
       /// Triggered when the puzzle state changes.
       /// </summary>
        public static event DependencyPropertyChanged StateChangedEvent;

        /// <summary>
        /// Determines if the puzzle is locked or not.
        /// If the puzzle is locked the user cannot move tiles.
        /// </summary>
       public static readonly DependencyProperty TileSizeProperty = DependencyProperty.RegisterAttached(nameof(TileSize), 
                                                                                                         typeof(short), 
                                                                                                         typeof(PuzzleSlider),
                                                                                                         new FrameworkPropertyMetadata(DefaultTileSize,
                                                                                                            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                                            new PropertyChangedCallback(OnTileSizeChanged)));
        /// <summary>
        /// Triggered when the locking property is changed.
        /// </summary>
        public static event DependencyPropertyChanged TileChangedEvent;


        private const byte Spacing = 5;
        private const byte BlankTileTag = 0;
        private const short AnimationDuration = 300; //in ms
        private const short DefaultTileSize = 100;

        public int _rowsCount;

        private readonly Dictionary<Tile, int> tagToPositionMap;
        private Tile blankTile;

        public short TileSize
        {
            get => (short)GetValue(TileSizeProperty);
            set => SetValue(TileSizeProperty, value);
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
                InitSlider();
            }
        }
        

        public PuzzleSlider()
        {
            tagToPositionMap = new Dictionary<Tile, int>();
            State = GetDefaultState();
            InitSlider();
            StateChangedEvent += OnNewState;
            TileChangedEvent += OnNewTileSize;
        }



        //returns the empty board
        private static ObservableBoard GetDefaultState()
        {
            return new ObservableBoard(new byte[,] { });

        }

        //creates the tiles for the puzzle
        private void InitSlider()
        {
            Children.Clear();
            tagToPositionMap.Clear();
            _rowsCount = State.Rows;
         
            for (int tileIndex = 0; tileIndex < _rowsCount * _rowsCount; tileIndex++)
            {
                var sliderTile = BuildTile(tileIndex);
                Children.Add(sliderTile);
                tagToPositionMap.Add(sliderTile, tileIndex);
            }
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
        /// Determines if the given tile can be exchanged with the blank tile.
        /// </summary>
        /// <param name="sliderTile"></param>
        /// <returns></returns>
        private bool IsMovePossible(Tile sliderTile)
        {
            var tilePosition = tagToPositionMap[sliderTile];
            var blankPosition = tagToPositionMap[blankTile];
            var tileRow = tilePosition / _rowsCount;
            var blankRow = blankPosition / _rowsCount;

            if(Math.Abs(tilePosition - blankPosition) == _rowsCount ||  //the tiles have to be on the same colomn 
              (Math.Abs(tilePosition - blankPosition) == 1 && tileRow == blankRow)) //the tiles have to be adjacent
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Exchanges thickness between two tiles. Used on animating.
        /// </summary>
        /// <param name="blankTile"></param>
        /// <param name="valueTile"></param>
        private void ExchangeThickness(Tile blankTile, Tile valueTile)
        {
            var tempThickness = blankTile.DestinationMargin;
            blankTile.DestinationMargin = valueTile.DestinationMargin;
            valueTile.DestinationMargin = tempThickness;
        }

        private void ExchangePositions(Tile blankTile, Tile valueTile)
        {
            var blankPosition = tagToPositionMap[blankTile];
            var valuePosition = tagToPositionMap[valueTile];
            
            //exchange position in the mapping
            tagToPositionMap[blankTile] = valuePosition;
            tagToPositionMap[valueTile] = blankPosition;
            
            //move the blank tile to the new position
            State.MoveBlankTile(valuePosition, blankPosition);
        }

        private void SetupStoryBoard(ThicknessAnimation blankAnimation, ThicknessAnimation valueTileAnimation)
        {
            // Create a Storyboard to contain and apply the animation.
            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.AutoReverse = false;
            pathAnimationStoryboard.Children.Add(valueTileAnimation);
            pathAnimationStoryboard.Children.Add(blankAnimation);
            pathAnimationStoryboard.Begin(this);
        }

        private void AnimateInDirection(int animationDirection)
        {
            var blankPosition = tagToPositionMap[blankTile];
            Tile otherTile = null;

            foreach(var keyPair in tagToPositionMap)
            {
                if (keyPair.Value == blankPosition  + animationDirection)
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

            sliderTile.PreviewMouseLeftButtonDown += OnTileClicked;

            return sliderTile;
        }

        /// <summary>
        /// Handles the state changing event.
        /// </summary>
        private void OnNewState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((PuzzleSlider) sender != this)
            {
                return;
            }
            State = (ObservableBoard)e.NewValue ?? GetDefaultState();
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
        /// Handles the tile changing event.
        /// </summary>
        private void OnNewTileSize(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if ((PuzzleSlider) sender != this)
            {
                return;
            }

            TileSize = (short?)args.NewValue ?? DefaultTileSize;
        }
    }
}