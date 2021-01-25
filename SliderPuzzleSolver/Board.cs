using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver
{
    public enum Direction : byte
    {
        Right,
        Left,
        Up,
        Down
    }

    public sealed class Board : IBoard, IEquatable<IBoard>
    {
        #region Fields

        private int? _hashCode;
        private byte[,] _tiles;
        #endregion Fields

        #region Proprieties

        public (byte Row, byte Column) BlankTilePosition { get; private set; }
        public byte Rows { get; private set; }
        #endregion Proprieties

        #region Public Methods

        public Board(byte[,] tiles)
        {
            if (tiles is null)
            {
                throw new ArgumentNullException();
            }

            SetupBoad(tiles);
        }

        public Board(string tilesRepresentation)
        {
            var tiles = GetTiles(tilesRepresentation);
            SetupBoad(tiles);
        }

        //generate the goal tiles
        public static IBoard GetGoalBoard(int dimension)
        {
            var tiles = new byte[dimension, dimension];
            for (int rowId = 0; rowId < dimension; rowId++)
                for (int columnId = 0; columnId < dimension; columnId++)
                {
                    tiles[rowId, columnId] = (byte)(rowId * dimension + columnId + 1);
                }
            tiles[dimension - 1, dimension - 1] = 0;
            return new Board(tiles);
        }

        //Determines if two boards have the arrangement of the numbers
        public bool Equals(IBoard obj)
        {
            var otherBoard = obj as Board;
            if (Rows != otherBoard.Rows) return false;
            for (int rowId = 0; rowId < Rows; rowId++)
                for (int columnId = 0; columnId < Rows; columnId++)
                {
                    if (_tiles[rowId, columnId] != otherBoard._tiles[rowId, columnId])
                    {
                        return false;
                    }
                }

            return true;
        }

        //returns a list with all the child boards
        //a child board is a board where the blank makes an move from the parent board.
        public List<IBoard> GetChildBoards()
        {
            var childBoards = new List<IBoard>();
            foreach (var direction in ConstantHelper.DirectionsTransfom.Keys)
            {
                var neighborTile = GetNeighborTiles(direction);

                if (neighborTile != null)
                {
                    childBoards.Add(new Board(neighborTile));
                }
            }
            return childBoards;
        }

        //returns a dictionary mapping the neighbor boards to their Manhattan difference against the current board.
        //between two neighbor boards the Manhattan distance will change by -1 or +1
        public Dictionary<IBoard, int> GetDistanceByChildBoards()
        {
            Dictionary<IBoard, int> allNeighbors = new Dictionary<IBoard, int>();
            foreach (var direction in ConstantHelper.DirectionsTransfom.Keys)
            {
                var neighborTile = GetNeighborTiles(direction);

                if (neighborTile != null)
                {
                    var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
                    var currentDistance = GetManhattanAt(_tiles, BlankTilePosition.Row + Row, BlankTilePosition.Column + Column);
                    var futureDistance = GetManhattanAt(neighborTile, BlankTilePosition.Row, BlankTilePosition.Column);
                    allNeighbors.Add(new Board(neighborTile), futureDistance - currentDistance);
                }
            }
            return allNeighbors;
        }

        /// <summary>
        /// Calculates the hash code of the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _hashCode ?? ComputeHash();
        }

        public byte[] GetTiles()
        {
            var tiles = new byte[Rows * Rows];
            int index = 0;
            foreach (var tile in _tiles)
            {
                tiles[index++] = tile;
            }
            return tiles;
        }

        //Hamming distance: number of tiles which are in wrong position
        public ushort Hamming()
        {
            ushort hammingCount = 0;
            for (int rowId = 0; rowId < Rows; rowId++)
                for (int colomnId = 0; colomnId < Rows; colomnId++)
                {
                    if (_tiles[rowId, colomnId] == ConstantHelper.BlankTileValue) continue;
                    var (Row, Column) = GoalPositionForValue(_tiles[rowId, colomnId]);
                    if (Row != rowId || Column != colomnId)
                        hammingCount++;
                }
            return hammingCount;
        }

        //determines if two boards are the same
        public bool Indentic(object that)
        {
            var otherBoard = (IBoard)that;
            if (this.Rows != otherBoard.Rows) return false;

            for (byte rowId = 0; rowId < Rows; rowId++)
                for (byte columnId = 0; columnId < Rows; columnId++)
                {
                    if (_tiles[rowId, columnId] != otherBoard.Tile(rowId, columnId))
                        return false;
                }

            return true;
        }

        /// <summary>
        /// If Dimension is odd, then puzzle instance is solvable if number of inversions is even in the input state.
        /// If Dimension is even, puzzle instance is solvable if:
        ///     the blank is on an even row counting from the bottom and number of inversions is odd.
        ///     the blank is on an odd row counting from the bottom and number of inversions is even.
        /// </summary>
        /// <returns></returns>
        public bool IsSolvable()
        {
            var invCount = InversionCount();
            if (Rows % 2 == 1)
            {
                return invCount % 2 == 0;
            }

            return (Rows - BlankTilePosition.Row) % 2 != invCount % 2;
        }

        //Is the board solved?
        //The board is solved when no tiles are in the wrong position
        public bool IsSolved()
        {
            return Hamming() == 0;
        }

        //Manhattan distance: total Manhattan distance to each tile to its goal position
        public int Manhattan()
        {
            int distance = 0;
            for (int rowId = 0; rowId < Rows; rowId++)
                for (int colomnId = 0; colomnId < Rows; colomnId++)
                {
                    if (_tiles[rowId, colomnId] == ConstantHelper.BlankTileValue)
                    {
                        continue;
                    }
                    distance += GetManhattanAt(_tiles, rowId, colomnId);
                }
            return distance;
        }

        //moves the blank tile in a given direction
        public void MoveBlankTile(Direction direction)
        {
            var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
            var newPosition = (Row: (byte)(BlankTilePosition.Row + Row), Column: (byte)(BlankTilePosition.Column + Column));

            if (newPosition.Row < 0 || newPosition.Row >= Rows) return;
            if (newPosition.Column < 0 || newPosition.Column >= Rows) return;

            Swap(_tiles, newPosition, BlankTilePosition);
            BlankTilePosition = newPosition;
        }

        //gets the value in a specific tile
        public byte Tile(byte row, byte column)
        {
            return _tiles[row, column];
        }
        //the string representation of the board
        public override string ToString()
        {
            var representation = new StringBuilder();

            for (int rowId = 0; rowId < Rows; rowId++)
            {
                representation.AppendLine();
                for (int columnId = 0; columnId < Rows; columnId++)
                {
                    representation.Append($"{_tiles[rowId, columnId]} ");
                }
            }

            return representation.ToString();
        }
        //Returns a board twin the current one
        //A twin is obtained by swapping any two tiles which are not blank.
        public IBoard Twin()
        {
            var newTiles = _tiles.Clone() as byte[,];
            var rand = new Random();
            //determines random the value of first tile
            var firstValuePos = (Row: rand.Next() % Rows, Colomn: rand.Next() % Rows);
            //move one row if gets to the blank position
            if (firstValuePos == BlankTilePosition)
            {
                firstValuePos.Row = (firstValuePos.Row + 1) % Rows;
            }
            //the second tile is on another colomn to avoid swapping same tile
            var secondValuePos = (Row: firstValuePos.Row, Colomn: (firstValuePos.Colomn + 1) % Rows);

            if (secondValuePos == BlankTilePosition)
            {
                secondValuePos.Row = (secondValuePos.Row + 1) % Rows;
            }

            Swap(newTiles, firstValuePos, secondValuePos);

            return new Board(newTiles);
        }

        public IBoard Clone()
        {
            var newTiles = _tiles.Clone() as byte[,];
            return new Board(newTiles);
        }

        #endregion Public Methods

        #region Private Methods

        private int ComputeHash()
        {
            int hash = 0;
            foreach (int tile in _tiles)
            {
                hash = HashCode.Combine(tile, hash);
            }
            _hashCode = hash;
            return _hashCode.Value;
        }

        //get the manhattan distance for single tile
        private int GetManhattanAt(byte[,] board, int x, int y)
        {
            var value = board[x, y] - 1;
            var goalRow = (value) / Rows;
            var goalCol = (value) % Rows;
            return Math.Abs(goalRow - x) + Math.Abs(goalCol - y);
        }

        //if the move of the blank til is possible return tile array where the blank tile is moved according to the direction
        //if the move is not possible, return null
        private byte[,] GetNeighborTiles(Direction direction)
        {
            var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
            var newPosition = (Row: BlankTilePosition.Row + Row, Column: BlankTilePosition.Column + Column);

            if (newPosition.Row < 0 || newPosition.Row >= Rows) return null;
            if (newPosition.Column < 0 || newPosition.Column >= Rows) return null;

            byte[,] transformedArray = new byte[Rows, Rows];

            for (int rowId = 0; rowId < Rows; rowId++)
                for (int columnId = 0; columnId < Rows; columnId++)
                {
                    transformedArray[rowId, columnId] = _tiles[rowId, columnId];
                }

            Swap(transformedArray, newPosition, BlankTilePosition);

            return transformedArray;
        }

        //get a byte array from a string representation
        private byte[,] GetTiles(string stringRepresentation)
        {
            var tilesValues = stringRepresentation.Split(" ");
            int n = (int)Math.Sqrt(tilesValues.Length);
            byte[,] tiles = new byte[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    tiles[i, j] = byte.Parse(tilesValues[i * n + j]);
                }
            return tiles;
        }

        //gets the position when a specific tile should be
        private (byte Row, byte Colomn) GoalPositionForValue(int value)
        {
            var valueBasedZero = value - 1;
            var goalPosition = (Row: (byte)(valueBasedZero / Rows), Colomn: (byte)(valueBasedZero % Rows));
            return goalPosition;
        }

        private ushort InversionCount()
        {
            byte s = 0;
            int n = Rows * Rows;
            byte[] copyArr = new byte[n];
            for (byte i = 0; i < Rows; i++)
                for (byte j = 0; j < Rows; j++)
                {
                    copyArr[i * Rows + j] = Tile(i, j);
                }

            for (var i = 0; i < n; i++)
            {
                for (var j = i; j < n; j++)
                {
                    if (copyArr[i] == ConstantHelper.BlankTileValue || copyArr[j] == ConstantHelper.BlankTileValue) continue;
                    if (copyArr[i] > copyArr[j]) s++;
                }
            }
            return s;
        }
        //set the tiles array from a provided byte array
        private void SetupBoad(byte[,] tiles)
        {
            Rows = (byte)tiles.GetLength(0);
            _tiles = new byte[Rows, Rows];

            for (byte i = 0; i < Rows; i++)
            {
                for (byte j = 0; j < Rows; j++)
                {
                    _tiles[i, j] = tiles[i, j];
                    if (_tiles[i, j] == ConstantHelper.BlankTileValue)
                    {
                        BlankTilePosition = (i, j);
                    }
                }
            }
        }

        //swap two tiles
        private void Swap(byte[,] a, (int Row, int Colomn) firstValue, (int Row, int Colomn) secondValue)
        {
            byte tmp = a[firstValue.Row, firstValue.Colomn];
            a[firstValue.Row, firstValue.Colomn] = a[secondValue.Row, secondValue.Colomn];
            a[secondValue.Row, secondValue.Colomn] = tmp;
        }
        #endregion Private Methods
    }
}