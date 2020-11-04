using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.Pages.SliderPage
{
    public delegate void PropertyChangedEvent();
    public class ObservableBoard
    {
        private byte[] _board;
        public event PropertyChangedEvent StateChanged;

        public ObservableBoard(IBoard board)
        {
            _board = board.GetTiles();
        }

        public ObservableBoard(byte[] board)
        {
            _board = board;
        }

        public int Length => _board.Length;

        public byte this[int i]
        {
            get => _board[i];
            set
            {
                _board[i] = value;
                RaiseStateChangedEvent();
            }
        }

        public bool IsSolved()
        {
            return new Board(string.Join(" ", _board)).IsSolved();
         }

        private void RaiseStateChangedEvent()
        {
            StateChanged?.Invoke();
        }
    }
}
