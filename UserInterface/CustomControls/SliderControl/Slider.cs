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


    public class Slider : Canvas
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(nameof(State),
                                                                                              typeof(byte[]),
                                                                                              typeof(Slider),
                                                                                              new PropertyMetadata(GetDefaultState(),
                                                                                                new PropertyChangedCallback(OnStateChanged)));

        public delegate void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
        public static event DependencyPropertyChanged StateChangedEvent;
            

        private const int RowCount = 4;
        private const int Spacing = 5;
        private const byte BlankTileTag = 0;

        private double _tilesHeight = 100;
        private double _tilesWidth = 100;
        private byte[] _state;

        private Dictionary<Tile, int> tagToPositionMap;
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
        

        public Slider()
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


        private static byte[] GetDefaultState()
        {
           var stateBytes = new byte[RowCount * RowCount];
            for (byte tileIndex = 0; tileIndex < RowCount * RowCount; tileIndex++)
            {
                stateBytes[tileIndex] = (byte)(tileIndex + 1);
            }
            stateBytes[RowCount * RowCount - 1] = BlankTileTag;
            return stateBytes;
        }
        private void InitSlider()
        {
            Children.Clear();
            tagToPositionMap.Clear();

            for (int tileIndex = 0; tileIndex < RowCount * RowCount; tileIndex++)
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
                From = c.FinalMargin,
                To = blankCanvas.FinalMargin,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            };

            var blankAnimation = new ThicknessAnimation
            {
                From = blankCanvas.FinalMargin,
                To = c.FinalMargin,
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
            var tileRow = tilePosition / RowCount;
            var blankRow = blankPosition / RowCount;

            if(Math.Abs(tilePosition - blankPosition) == 4 ||  //the tiles have to be on the same colomn 
              (Math.Abs(tilePosition - blankPosition) == 1 && tileRow == blankRow)) //the tiles have to be adjacent
            {
                return true;
            }
            return false;
        }

        private void ExchangeThickness(Tile firstValue, Tile secondValue)
        {
            var tempThickness = firstValue.FinalMargin;
            firstValue.FinalMargin = secondValue.FinalMargin;
            secondValue.FinalMargin = tempThickness;

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
            var tileColumn = tileIndex % RowCount;
            var tileRow = tileIndex / RowCount;
            var tileTag = State[tileIndex]; 

            var sliderTile = new Tile
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = _tilesWidth,
                Height = _tilesHeight,
                TileTag = tileTag,
                Background = tileTag == BlankTileTag ? Brushes.Transparent : Brushes.Wheat,
                Margin = new Thickness(tileColumn * (_tilesWidth + Spacing), tileRow * (_tilesHeight + Spacing), 0, 0),
                FinalMargin = new Thickness(tileColumn * (_tilesWidth + Spacing), tileRow * (_tilesHeight + Spacing), 0, 0),
            };
            var textBlock = new TextBlock
            {
                Text = tileTag.ToString(),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = 48
            };

            if(tileTag != BlankTileTag)
            {
                sliderTile.Children.Add(textBlock);
            }
            else
            {
                blankCanvas = sliderTile;
            }

            sliderTile.MouseDown += OnTileClicked;

            return sliderTile;
        }

    }
}