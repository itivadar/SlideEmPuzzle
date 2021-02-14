using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.Events
{

    /// <summary>
    /// Event triggered when the user finished the game.
    /// </summary>
    public class GameFinishedEvent : PubSubEvent<GameFinishedEvent>
    {
        /// <summary>
        /// Duration of the game.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        /// <summary>
        /// Total moves made by the player.
        /// </summary>
        public int MovesCount{ get; set; }
        
        /// <summary>
        /// Minimum number of moves required to solve the puzzle.
        /// Used to calculate the score.
        /// </summary>
        public int MinMoves { get; set; }

        /// <summary>
        /// The rows of the puzzle.
        /// </summary>
        public byte PuzzleRows { get; set;}
    }
}
