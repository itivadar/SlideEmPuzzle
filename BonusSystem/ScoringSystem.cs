using System;
using System.Collections.Generic;

namespace BonusSystem
{
  public class ScoringSystem : IScoringSystem
  {
    /// <summary>
    /// The moves count for which the player gets the least base points
    /// </summary>
    private const int MaxRewardedMoves = 90;

    /// <summary>
    /// The maximum time in seconds for which the player receives bonus time points.
    /// </summary>
    private const int MaxRewaredTime = 120;

    /// <summary>
    /// The based bonus associated with the puzzle rows.
    /// The base points will be awarded to the player if he succeds to solve the puzzle
    /// </summary>
    private Dictionary<byte, short> _basePoints = new Dictionary<byte, short>()
    {
        {2, 1000},
        {3, 2000},
        {4, 2800}
    };

    /// <summary>
    /// Gets the player score based on the perfomance.
    /// </summary>
    /// <param name="puzzleRows">the dificulty of the puzzle</param>
    /// <param name="playerMoves">the player moves</param>
    /// <param name="minMoves">the minimum required moves to solve the puzzle</param>
    /// <param name="playerTime">the time of the solving</param>
    /// <returns>the player score </returns>
    public ushort GetPlayerScore(byte puzzleRows, int playerMoves, int minMoves, int playerTime)
    {
      return (ushort)(GetBasePoints(puzzleRows, playerMoves, minMoves) + GetBonusTimePoints(puzzleRows, playerTime));
    }

    /// <summary>
    /// Gets the base points for the user. 
    /// Based on the numbers of the player moves.
    /// </summary>
    /// <returns>the based points</returns>
    private ushort GetBasePoints(byte puzzleRows, int playerMoves, int minMoves)
    {
      playerMoves = Math.Min(playerMoves, MaxRewardedMoves);
      playerMoves = Math.Max(playerMoves, minMoves);

      var coefficientBonus = ((float)(MaxRewardedMoves - playerMoves) / (MaxRewardedMoves - minMoves));
      return (ushort)(Math.Pow(2, coefficientBonus) * _basePoints[puzzleRows]);
    }

    /// <summary>
    /// Gets the bonus time points for the user.
    /// Faster means more points.
    /// </summary>
    private ushort GetBonusTimePoints(byte puzzleRows, int playerTime)
    {
      playerTime = Math.Min(playerTime, MaxRewaredTime);

      var timeBonus = ((float)(MaxRewaredTime - playerTime) / MaxRewaredTime);
      return (ushort)(timeBonus * 1000);
    }

  }
}