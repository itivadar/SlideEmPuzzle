using BinaryHeap;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

namespace SliderPuzzleSolver
{
  public sealed class PuzzleSolver : IPuzzleSolver
    {
        #region Helper Classes
        public class Node : IComparable
        {
            public Node PreviousNode { get; set; }
            public IBoard Board { get; set; }
            public ushort MovesToGetHere { get; set; }         
            public ushort ManhattanDistance { get; set; }
            public ushort HammingDistance { get; set; }

            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }
        }

        public class ManhattanPriorityFunction: IComparer<Node>
        {
            public int Compare([AllowNull] Node currentNode, [AllowNull] Node otherNode)
            {
                var currentNodeDistance = currentNode.ManhattanDistance + currentNode.MovesToGetHere;
                var otherNodeDistance = otherNode.ManhattanDistance + otherNode.MovesToGetHere;

                if (currentNodeDistance < otherNodeDistance) return -1;
                if (currentNodeDistance > otherNodeDistance) return 1;
                if (currentNode.HammingDistance < otherNode.HammingDistance) return -1;
                if (currentNode.HammingDistance > otherNode.HammingDistance) return 1;
           
                return 0;
            }
        }

        #endregion

        #region Fields
        readonly IBoard _initialBoard;
        #endregion

        #region Public methods
        public PuzzleSolver(IBoard board)
        {
            if(board is null)
            {
                throw  new ArgumentNullException();
            }

            _initialBoard = board;
        }

        public IEnumerable<IBoard> SolutionSteps()
        {
            if (!_initialBoard.CanBeSolved())
            {
                Console.WriteLine("Unsolvable puzzle");
                return new List<IBoard>() as IEnumerable<IBoard>;
            }

            var lastNode = Solve();

            return BuildSolutionSteps(lastNode);
        }

        #endregion

        #region Private Methods
        private IEnumerable<IBoard> BuildSolutionSteps(Node lastNode)
        {
            Stack<IBoard> sol = new Stack<IBoard>();
            while (lastNode != null)
            {
                sol.Push(lastNode.Board);
                lastNode = lastNode.PreviousNode;
            }

            return sol as IEnumerable<IBoard>;
        }

        private Node Solve()
        {
            IHeap<Node> minPriorityQueue = new MinHeap<Node>(new ManhattanPriorityFunction());
            //no previous node, 0 moves to get to the root
            var root = CreateNode(_initialBoard, null);

            minPriorityQueue.Add(root);
            Node dequeuedNode;
            do
            {
                dequeuedNode = minPriorityQueue.PopMin();
                foreach (var neighbor in dequeuedNode.Board.GetNeighbors())
                {
                    if (dequeuedNode.PreviousNode is null || !dequeuedNode.PreviousNode.Board.Equals(neighbor))
                    {   
                        //calculate the priority function and updates the moves needed to get to the created node
                        var childNode = CreateNode(neighbor, dequeuedNode);

                        minPriorityQueue.Add(childNode);
                    }
                }
            } while (!dequeuedNode.Board.IsSolved());

            return dequeuedNode;
        }

        private Node CreateNode(IBoard board, Node previousNode)
        {
            return new Node
            {
                Board = board,
                HammingDistance = board.Hamming(),
                ManhattanDistance = board.Manhattan(),
                PreviousNode = previousNode,
                MovesToGetHere =( ushort) (previousNode is null ? 0 : previousNode.MovesToGetHere + 1)
            };
        }
        #endregion
    }
}
