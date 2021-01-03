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

    public class PuzzleSlider : Canvas
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(nameof(State),
                                                                                              typeof(ObservableBoard),
                                                                                              typeof(PuzzleSlider),
                                                                                              new PropertyMetadata(null,
                                                                                                new PropertyChangedCallback(OnStateChanged)));
       
        public delegate void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
        public static event DependencyPropertyChanged StateChangedEvent;


        private const int Spacing = 5;
        private const byte BlankTileTag = 0;
        private const int AnimationDuration = 300; //in ms

        private readonly double _tilesHeight = 100;
        private readonly double _tilesWidth = 100;
        public int _rowsCount;

        private readonly Dictionary<Tile, int> tagToPositionMap;
        private Tile blankTile;

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
        }

        private void OnNewState(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            State = (ObservableBoard) e.NewValue ?? GetDefaultState();
        }


        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateChangedEvent?.Invoke(d, e);
        }


        //generates the goal state
        private static ObservableBoard GetDefaultState()
        {
            return new ObservableBoard("1 2 3 0");
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

        private void OnTileClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedTile = sender as Tile;
            if (IsMovePossible(clickedTile))
            {
                AnimateSwitching(clickedTile);
            }
        }

       
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

            if (tileTag == BlankTileTag)
            {
                blankTile = sliderTile;
            }

            sliderTile.PreviewMouseLeftButtonDown += OnTileClicked;

            return sliderTile;
        }

    }
}