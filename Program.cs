using System.Security.Cryptography;

namespace Hangman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Assets asset = new();
            string option = Assets.OptionSelection();
            int guesses = asset.GuessSelection();
            asset.Game(option, guesses);
        }   
    }
}