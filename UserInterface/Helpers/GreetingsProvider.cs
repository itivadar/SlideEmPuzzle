using System;
using System.Collections.Generic;

namespace UserInterface.Helpers
{
    internal class GreetingsProvider : Dictionary<int, string>, IGreetingsProvider
    {
        #region Private Fields

        private readonly Random _randomGenerator;

        #endregion Private Fields

        #region Public Constructors

        public GreetingsProvider()
        {
            _randomGenerator = new Random();
            Initialize();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets a random greetings to be displayed after the user choices a puzzle.
        /// </summary>
        /// <returns>a string with the greeting about the user choise</returns>
        public string GetRandomGreeting()
        {
            return this[_randomGenerator.Next(Count)];
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Maps a greetins to a index.
        /// </summary>
        /// <param name="greeting">the greeting text</param>
        private void AddGreetings(string greeting)
        {
            this[Count] = greeting;
        }

        /// <summary>
        /// Adds all the greetins text.
        /// </summary>
        private void Initialize()
        {
            AddGreetings("Good choice!");
            AddGreetings("A wise choice!");
            AddGreetings("Well chosen!");
            AddGreetings("Very well then!");
            AddGreetings("Excellent!");
            AddGreetings("Aye, capitan");
            AddGreetings("Nice one!");
            AddGreetings("Bingo!");
            AddGreetings("Let's slide'em!");
            AddGreetings("You gotta slide'em!");
            AddGreetings("Go  Slide'em!");
            AddGreetings("You're on the right path!");
            AddGreetings("Oh..again?");
            AddGreetings("Same puzzle? Again? ...");
            AddGreetings("Evidence of a fine taste");
        }

        #endregion Private Methods
    }
}