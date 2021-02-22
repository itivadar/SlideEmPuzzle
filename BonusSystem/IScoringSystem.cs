namespace BonusSystem
{
  public interface IScoringSystem
  {
    /// <summary>
    /// Gets the player score based on the perfomance.
    /// </summary>
    /// <param name="puzzleRows">the dificulty of the puzzle</param>
    /// <param name="playerMoves">the player moves</param>
    /// <param name="minMoves">the minimum required moves to solve the puzzle</param>
    /// <param name="playerTime">the time of the solving</param>
    /// <returns>the player score </returns>
    ushort GetPlayerScore(byte puzzleRows, int playerMoves, int minMoves, int playerTime);


    /// <summary>
    /// Gets the stars count for the player score.
    /// </summary>
    /// <param name="playerScore">the player score</param>
    /// <returns>integer between 0 and 3 represeting the stars gained by the player</returns>
    int GetStarCount(int playerScore);
  }
}