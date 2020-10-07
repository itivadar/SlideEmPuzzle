using Prism.Commands;
using Prism.Mvvm;
using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UserInterface.Pages.SliderPage
{
    class SliderPageViewModel : BindableBase
    {
        private byte[] _sliderState;
        private readonly IPuzzleSolver puzzleSolver;

        public SliderPageViewModel()
        {
            RandomizeCommand = new DelegateCommand(OnRandomize);
            puzzleSolver = new PuzzleSolver();
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
            var board = puzzleSolver.GenerateRandomBoard(4);
            SliderState = board.GetTiles();
        }

    }
}

