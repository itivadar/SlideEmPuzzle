using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;

namespace UserInterface.Pages.SliderPage
{
    public delegate void PropertyChangedEvent();

    public class ObservableBoard
    {
        #region Private Fields

        private IBoard _board;

        #endregion Private Fields

        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Events
        /// <summary>
        /// Event raised when the board state is changed. 
        /// </summary>
        public event PropertyChangedEvent StateChanged;

        #endregion Public Events

        #region Public Properties
        /// <summary>
        /// Determines if the board is solved or not.
        /// </summary>
        public bool IsSolved => _board.IsSolved();

        /// <summary>
        /// Gets the board.
        /// </summary>
        public IBoard Board => _board.Clone();

        /// <summary>
        /// Gets the board rows.
        /// </summary>
        public byte Rows => _board.Rows;

        #endregion Public Properties

        #region Public Indexers
        
        /// <summary>
        /// Gets the tile at a specific index.
        /// </summary>
        /// <param name="i">the index of the requested tile</param>
        /// <returns>the tile tag at the given index.</returns>
        public byte this[int i]
        {
            get => _board.GetTiles()[i];
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Gets the goal board consisting of the given rows.
        /// </summary>
        /// <param name="rows">Number of the rows in the board</param>
        /// <returns></returns>
        public static ObservableBoard GetGoalState(int rows)
        {
            byte[,] state = new byte[rows, rows];
            byte tileTag = 1;
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < rows; colIndex++)
                {
                    state[rowIndex, colIndex] = tileTag++;
                }
            }
            state[rows - 1, rows - 1] = 0;
            return new ObservableBoard(state);
        }

        public void MoveBlankTile(int tilePosition, int blankPosition)
        {
            var direction = Direction.Up;
            if (blankPosition - tilePosition == 1) direction = Direction.Left;
            if (blankPosition - tilePosition == -1) direction = Direction.Right;
            if (blankPosition - tilePosition == -Rows) direction = Direction.Down;

            MoveBlankTile(direction);
        }

        public void MoveBlankTile(Direction direction)
        {
            _board.MoveBlankTile(direction);
            RaiseStateChangedEvent();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Raised when the board state has been changed.
        /// Triggred by any player moves.
        /// </summary>
        private void RaiseStateChangedEvent()
        {
            StateChanged?.Invoke();
        }

        #endregion Private Methods
    }
}