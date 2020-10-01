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
    public class Slider : Grid
    {
        private const int RowCount = 4;
        private const int Spacing = 5;
        private byte[] _state;
        private double _tilesHeight = 100;
        private double _tilesWidth = 100;
        int index = 0;

        private Dictionary<Tile, int> tagToPositionMap;
        private Tile blankCanvas;

        public Slider()
        {
            tagToPositionMap = new Dictionary<Tile, int>();
            InitState();
            InitSlider();
        }


        private void InitState()
        {
            _state = new byte[RowCount * RowCount];
            for (byte tileIndex = 0; tileIndex < RowCount * RowCount; tileIndex++)
            {
                _state[tileIndex] = tileIndex;
            }
        }

        private void InitSlider()
        {
            for (int tileIndex = 0; tileIndex < RowCount * RowCount; tileIndex++)
            {
                var sliderTile = BuildTile(tileIndex);

                if (tileIndex + 1 == RowCount * RowCount)
                {
                    sliderTile = BuildTile(tileIndex, true);
                    blankCanvas = sliderTile;
                }
                
                Children.Add(sliderTile);
                tagToPositionMap.Add(sliderTile, _state[tileIndex]);
            }
        }

        private void C_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedTile = sender as Tile;

            AnimateYourSelf(clickedTile);

        }
       
        private void AnimateYourSelf(Tile c, int index = 0)
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
            tileAnimation.Completed += TileAnimation_Completed;
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
            pathAnimationStoryboard.Completed += PathAnimationStoryboard_Completed;
            pathAnimationStoryboard.Begin(this);
        }

        private void TileAnimation_Completed(object sender, EventArgs e)
        {
        }

        private void PathAnimationStoryboard_Completed(object sender, EventArgs e)
        {
            
        }

        private bool IsMovePossible(Tile sliderTile)
        {
            var tilePosition = tagToPositionMap[sliderTile];
            var blankPosition = tagToPositionMap[blankCanvas];

            if(Math.Abs(tilePosition - blankPosition) == 4 || Math.Abs(tilePosition - blankPosition) == 1) 
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
                AnimateYourSelf(otherTile);
            }
        }

        private Tile BuildTile(int tileIndex, bool IsBlankTile = false)
        {
            var tileColumn = _state[tileIndex] % RowCount;
            var tileRow = _state[tileIndex] / RowCount;

            var sliderTile = new Tile
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = _tilesWidth,
                Height = _tilesHeight,
                TileTag = tileIndex,
                Background = IsBlankTile ? Brushes.Transparent : Brushes.Wheat,
                Margin = new Thickness(tileColumn * (_tilesWidth + Spacing), tileRow * (_tilesHeight + Spacing), 0, 0),
                FinalMargin = new Thickness(tileColumn * (_tilesWidth + Spacing), tileRow * (_tilesHeight + Spacing), 0, 0),
            };

            var textBlock = new TextBlock
            {
                Text = (tileIndex + 1).ToString(),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = 48
            };

            if(!IsBlankTile)
            {
                sliderTile.Children.Add(textBlock);
            }

            sliderTile.MouseDown += C_MouseDown;

            return sliderTile;
        }

    }
}