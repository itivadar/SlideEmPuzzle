using Prism.Events;

namespace UserInterface.Events
{
    /// <summary>
    /// Event triggered when the user finished the game.
    /// </summary>
    internal class PuzzleTypeSelectedEvent : PubSubEvent<string>
    {
    }
}