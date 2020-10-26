using BinaryHeap;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SliderPuzzleSolver
{
  public sealed class PuzzleSolver : IPuzzleSolver
    {
        #region Helper Classes
        public class Node : IComparable
        {
            public Node ParentNode { get; set; }
            public IBoard Board { get; set; }
            public ushort Depth { get; set; }         
            public ushort ManhattanDistance { get; set; }

            ///used for comparing nodes to maintain the MinHeap property
            public int CompareTo(object obj)
            {
                return new ManhattanPriorityFunction().Compare(this, obj as Node);
            }
        }

        /// <summary>
        /// Compare used to compare nodes in the A* search algorithm
        /// </summary>
        public class ManhattanPriorityFunction: IComparer<Node>
        {
            public int Compare(Node currentNode, Node otherNode)
            {
                var currentNodeDistance = currentNode.ManhattanDistance  + currentNode.Depth;
                var otherNodeDistance = otherNode.ManhattanDistance   + otherNode.Depth;

                if (currentNodeDistance < otherNodeDistance) return -1;
                if (currentNodeDistance > otherNodeDistance) return 1;
           
                return 0;
            }
        }
        #endregion

        #region Constants
        const int NodeFound = -1;
        const int NodeNotFound = Int32.MaxValue;
        #endregion

        #region Public methods

        public IEnumerable<IBoard> SolutionSteps(IBoard boardToSolve)
        {
            if (!boardToSolve.IsSolvable())
            {
                Console.WriteLine("Unsolvable puzzle");
                return new List<IBoard>() as IEnumerable<IBoard>;
            }

            var lastNode = IDASolve(boardToSolve);

            return BuildSolutionSteps(lastNode) as IEnumerable<IBoard>;
        }

        #endregion

        #region Private Methods
        private IEnumerable<IBoard> BuildSolutionSteps(Node lastNode)
        {
            Stack<IBoard> sol = new Stack<IBoard>();
            while (lastNode != null)
            {
                sol.Push(lastNode.Board);
                lastNode = lastNode.ParentNode;
            }

            return sol as IEnumerable<IBoard>;
        }

        private Node Solve(IBoard board)
        {
            IHeap<Node> minPriorityQueue = new MinHeap<Node>(new ManhattanPriorityFunction());
            //no previous node, 0 moves to get to the root
            var root = CreateNode(board, null, board.Manhattan());

            minPriorityQueue.Add(root);
            Node dequeuedNode;
            do
            {
                dequeuedNode = minPriorityQueue.PopMin();
                foreach (var neighbor in dequeuedNode.Board.GetDistanceByChildBoards())
                {
                    if (dequeuedNode.ParentNode is null || !dequeuedNode.ParentNode.Board.Equals(neighbor))
                    {   
                        //calculate the priority function and updates the moves needed to get to the created node
                        var childNode = CreateNode(neighbor.Key, dequeuedNode,neighbor.Value);

                        minPriorityQueue.Add(childNode);
                    }
                }
            } while (!dequeuedNode.Board.IsSolved());

            return dequeuedNode;
        }

        //uses the IDA* algo to search for a optimal solution for the N-puzzle
        private Node IDASolve(IBoard startingBoard)
        {
            //startingNode has no parent node
            var startingNode = CreateNode(startingBoard, null, startingBoard.Manhattan());

            //used to remember each node in the path from the board that needs to be solved to the goal board
            var path = new Stack<Node>();
            path.Push(startingNode);

            int thresholdDepth = startingNode.ManhattanDistance;
            while(true)
            {
                var searchResult = BreadthFirstSearch(path, thresholdDepth);
             
                if (searchResult == NodeFound) return path.Peek();
                //there are no solution to the puzzle
                if (searchResult == NodeNotFound) { return null; }

                //the depth limit is increasead and the search is restarted
                thresholdDepth = searchResult;
            }
        }

        //makes a breadth first search into the search graph for the current puzzle
        //the search stops when the search cost exceeds a given threshold as parameter
        //it returns
        // - NodeFound (-1) if the goal node was reached.
        // - NodeNotFound (Int32.MaxValue) if no solution were found
        // - an integer representing the new threshold for the BFS search
        private int BreadthFirstSearch (Stack<Node> path, int threshold)
        {
            var node = path.Peek();
            var currentCost = node.Depth + node.ManhattanDistance;
            //we increase the threshold and start over.
            if (currentCost > threshold) return currentCost;

            //we found the goal board
            if (node.ManhattanDistance == 0) return NodeFound;

            var min = NodeNotFound;
            foreach (KeyValuePair<IBoard, int> childPair in node.Board.GetDistanceByChildBoards())
            {
                var neighborNode = CreateNode(childPair.Key, node, childPair.Value);
                if (node.ParentNode != null && childPair.Value.Equals(node.ParentNode.Board)) continue;
                path.Push(neighborNode);

                var result = BreadthFirstSearch(path, threshold);

                if (result == NodeFound) return NodeFound;
                min = Math.Min(result, min);
                path.Pop();
            }
            return min;
        }

        /// creates a node for the search tree.
        private Node CreateNode(IBoard board, Node previousNode, int manhattanDelta)
        {
            return new Node
            {
                Board = board,
                ManhattanDistance = (ushort)(previousNode is null ? manhattanDelta : previousNode.ManhattanDistance + manhattanDelta),
                ParentNode = previousNode,
                Depth = (ushort)(previousNode is null ? 0 : previousNode.Depth + 1)
            };
        }
        #endregion
    }
}
