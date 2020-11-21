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
        private IBoard _board;
        public event PropertyChangedEvent StateChanged;

        public ObservableBoard(IBoard board)
        {
            _board = board;
        }

        public ObservableBoard(byte[,] board)
        {
            _board = new Board(board);
        }

        public ObservableBoard(string board)
        {
            _board = new Board(board);
        }

        public int Rows => _board.Rows;
        public bool IsSolved => _board.IsSolved();


        public byte this[int i]
        {
            get => _board.GetTiles()[i];
        }

        public void MoveBlankTile(int tilePosition, int blankPosition)
        {
            var direction = Direction.Up;
            if (blankPosition - tilePosition == 1)      direction = Direction.Left;
            if (blankPosition - tilePosition == -1)     direction = Direction.Right;
            if (blankPosition - tilePosition == -Rows)  direction = Direction.Down;

            MoveBlankTile(direction);
        }

        public void MoveBlankTile(Direction direction)
        {
            _board.MoveBlankTile(direction);
            RaiseStateChangedEvent();
        }

        private void RaiseStateChangedEvent()
        {
            StateChanged?.Invoke();
        }
    }
}
