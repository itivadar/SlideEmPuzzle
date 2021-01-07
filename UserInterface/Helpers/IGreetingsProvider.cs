namespace UserInterface.Helpers
{
    /// <summary>
    /// Defines the interface for the provider.
    /// </summary>
    public interface IGreetingsProvider
    {
        /// <summary>
        /// Gets a random greetings to be displayed after the user choices a puzzle.
        /// </summary>
        /// <returns>a string with the greeting about the user choise</returns>
        string GetRandomGreeting();
    }
}