using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UserInterface.CustomControls
{
    public static class AnimationDirection
    {
        public static int Up =>-4;
        public static int Left => -1;
        public static int Down => 4;
        public static int Right => 1;

    }


    public class PuzzleSlider : Canvas
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(nameof(State),
                                                                                              typeof(byte[]),
                                                                                              typeof(PuzzleSlider),
                                                                                              new PropertyMetadata(GetDefaultState(),
                                                                                                new PropertyChangedCallback(OnStateChanged)));
        public delegate void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
        public static event DependencyPropertyChanged StateChangedEvent;


        private const int DefaultSize = 2;
        private const int Spacing = 5;
        private const byte BlankTileTag = 0;

        private readonly double _tilesHeight = 100;
        private readonly double _tilesWidth = 100;
        public int _rowsCount;

        private byte[] _state;

        private readonly Dictionary<Tile, int> tagToPositionMap;
        private Tile blankCanvas;

        /// <summary>
        /// Gets or sets the slider state
        /// Every element of state[i] represents the tag of the tile at position i.
        /// Element 0 is for blank tile
        /// </summary>
        public byte[] State
        {
            get => _state;
            set 
            {
                _state = value;
               InitSlider();
            }
        }
        

        public PuzzleSlider()
        {
            tagToPositionMap = new Dictionary<Tile, int>();
            State = GetDefaultState();
            InitSlider();           
            StateChangedEvent += OnNewState;
        }

        private void OnNewState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            State = (byte[])e.NewValue ?? GetDefaultState();
        }


        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateChangedEvent?.Invoke(d, e);
        }


        //generates the goal state
        private static byte[] GetDefaultState()
        {
           var stateBytes = new byte[DefaultSize * DefaultSize];
            for (byte tileIndex = 0; tileIndex < DefaultSize * DefaultSize; tileIndex++)
            {
                stateBytes[tileIndex] = (byte)(tileIndex + 1);
            }
            stateBytes[DefaultSize * DefaultSize - 1] = BlankTileTag;
            return stateBytes;
        }

        //creates the tiles for the puzzle
        private void InitSlider()
        {
            Children.Clear();
            tagToPositionMap.Clear();
            _rowsCount = (int) Math.Sqrt(State.Length);
         
            for (int tileIndex = 0; tileIndex < _rowsCount * _rowsCount; tileIndex++)
            {
                var sliderTile = BuildTile(tileIndex);
                Children.Add(sliderTile);
                tagToPositionMap.Add(sliderTile, tileIndex);
            }
        }

        private void OnTileClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedTile = sender as Tile;
            if (IsMovePossible(clickedTile))
            {
                AnimateSwitching(clickedTile);
            }
        }

       
        private void AnimateSwitching(Tile c, int index = 0)
        {
            var tileAnimation = new ThicknessAnimation
            {
                From = c.DestinationMargin,
                To = blankCanvas.DestinationMargin,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            };

            var blankAnimation = new ThicknessAnimation
            {
                From = blankCanvas.DestinationMargin,
                To = c.DestinationMargin,
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };


            ExchangeThickness(blankCanvas, c);
            tileAnimation.BeginTime = new TimeSpan(0, 0, 0,0,600*index);
            blankAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 600 * index);
            Storyboard.SetTarget(blankAnimation, blankCanvas);
            Storyboard.SetTargetProperty(blankAnimation, new PropertyPath(Tile.MarginProperty));
            Storyboard.SetTarget(tileAnimation, c);
            Storyboard.SetTargetProperty(tileAnimation, new PropertyPath(Tile.MarginProperty));

            // Create a Storyboard to contain and apply the animation.
            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.AutoReverse = false;
            pathAnimationStoryboard.Children.Add(tileAnimation);
            pathAnimationStoryboard.Children.Add(blankAnimation);
            pathAnimationStoryboard.Begin(this);
        }

        private bool IsMovePossible(Tile sliderTile)
        {
            var tilePosition = tagToPositionMap[sliderTile];
            var blankPosition = tagToPositionMap[blankCanvas];
            var tileRow = tilePosition / _rowsCount;
            var blankRow = blankPosition / _rowsCount;

            if(Math.Abs(tilePosition - blankPosition) == _rowsCount ||  //the tiles have to be on the same colomn 
              (Math.Abs(tilePosition - blankPosition) == 1 && tileRow == blankRow)) //the tiles have to be adjacent
            {
                return true;
            }
            return false;
        }

        private void ExchangeThickness(Tile firstValue, Tile secondValue)
        {
            var tempThickness = firstValue.DestinationMargin;
            firstValue.DestinationMargin = secondValue.DestinationMargin;
            secondValue.DestinationMargin = tempThickness;

            var firstPosition = tagToPositionMap[firstValue];
            tagToPositionMap[firstValue] = tagToPositionMap[secondValue];
            tagToPositionMap[secondValue] = firstPosition;
        }

        private void AnimateInDirection(int animationDirection)
        {
            var blankPosition = tagToPositionMap[blankCanvas];
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

        private Tile BuildTile(int tileIndex)
        {
            var tileColumn = tileIndex % _rowsCount;
            var tileRow = tileIndex / _rowsCount;
            var tileTag = State[tileIndex]; 

            var sliderTile = new Tile
            {
                Width = _tilesWidth,
                Height = _tilesHeight,
                TileTag = tileTag,
                IsBlankTile = tileTag == BlankTileTag,
                Margin = new Thickness(tileColumn * (_tilesWidth + Spacing), tileRow * (_tilesHeight + Spacing), 0, 0),
                DestinationMargin = new Thickness(tileColumn * (_tilesWidth + Spacing), tileRow * (_tilesHeight + Spacing), 0, 0),
            };
           
            if(tileTag == BlankTileTag)
            {
                blankCanvas = sliderTile;
            }

            sliderTile.MouseDown += OnTileClicked;

            return sliderTile;
        }

    }
}