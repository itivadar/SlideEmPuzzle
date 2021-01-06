using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.Events
{
    /// <summary>
    /// Event triggered when the user finished the game.
    /// </summary>
    class PuzzleTypeSelectedEvent : PubSubEvent<string>
    {
    }
}
