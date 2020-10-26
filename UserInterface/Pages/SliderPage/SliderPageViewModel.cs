using Prism.Commands;
using Prism.Mvvm;
using SliderPuzzleGenerator;
using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UserInterface.Pages.SliderPage
{
    public class SliderPageViewModel : BindableBase
    {
        private byte[] _sliderState;
        private readonly IPuzzleSolver _puzzleSolver;
        private readonly IPuzzleGenerator _puzzleGenerator;

        public SliderPageViewModel(IPuzzleSolver puzzleSolver, IPuzzleGenerator puzzleGenerator)
        {
            _puzzleSolver = puzzleSolver;
            _puzzleGenerator = puzzleGenerator;
            RandomizeCommand = new DelegateCommand(OnRandomize);
            
            
        }
        public ICommand RandomizeCommand { get; set; }

        public byte[] SliderState
        {
            get => _sliderState;
            set 
            {
                _sliderState = value;
                RaisePropertyChanged(nameof(SliderState));
            }
        }

        private void OnRandomize()
        {
            var board = _puzzleGenerator.GenerateRandomPuzzle(3);
            SliderState = board.GetTiles();
        }

    }
}

