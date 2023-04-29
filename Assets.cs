using System;
using System.Diagnostics;
using System.Text.Json;

namespace Hangman
{
    internal class Assets
    {
        private static bool askAgain = true;
        public static (string[], string[], string[]) ReadWords(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);

            var data = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);

            return (data["easy"], data["medium"], data["hard"]);
        }

        public static bool IsValid(string input, string type)
        {
            char[] alphabetAndSpecialChars = new char[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=',
                '{', '}', '[', ']', '\\', '|', ';', ':', '\'', '"', ',', '.', '<', '>', '/', '?'
            };


            bool isValid;
            if(input == null || input == "")
            {
                isValid = false;
                return isValid;
            }
            else if(type=="number" || type=="num")
            {
                for (int i = 0; i < input.Length; i++)
                {
                    for (int j = 0; j < alphabetAndSpecialChars.Length; j++)
                    {
                        if (input[i] == alphabetAndSpecialChars[j])
                        {
                            isValid = false;
                            Console.WriteLine($"input: {input[i]} is {alphabetAndSpecialChars[j]} ");
                            return isValid;

                        }
                    }
                }
                isValid = true;
                return isValid;
            }
            return true;
        }

        public int GuessSelection()
        {
            int defaultValue = 7;
            while (askAgain == true)
            {
                Console.WriteLine("\nDo you want to change default guess amount (7)?");
                Console.WriteLine("Y | N | DAA(NO, don't ask again)\n");
                var option = Console.ReadLine();
                if (option != null || option != "")
                {
                    option = option.ToLower();
                    if (option == "y")
                    {
                        Console.WriteLine("\nType the amount of guesses (integer)\n");
                        var guesses = Console.ReadLine();
                        bool isValid = IsValid(guesses, "number");
                        if (isValid == true)
                        {
                            return int.Parse(guesses);
                        }
                        else
                        {
                            Console.WriteLine("\nNumber is not valid, setting guesses to 7");
                            return defaultValue;
                        }
                    }
                    else if (option == "n")
                    {
                        return defaultValue;
                    }
                    else if (option == "daa")
                    {
                        Assets assets = new();
                        askAgain = false;
                    }
                }
                return defaultValue;
            }
            return defaultValue;
        }
        public static string OptionSelection()
        {
            while (true)
            {
                Console.WriteLine("Hangman game");
                Console.WriteLine("Select difficulty: easy/medium/hard\n");

                string[] difficulty = new string[] { "hard", "medium", "easy" };
                var option = Console.ReadLine() ?? throw new Exception("option value is null");
                option = option.ToLower();

                for (int i = 0; i < difficulty.Length; i++)
                {

                    if (option == difficulty[i])
                    {
                        return option;
                    }
                    else if (option != difficulty[i] && i >= difficulty.Length - 1)
                    {
                        Console.WriteLine("\nIncorrect difficulty\nYou have to write: easy|medium|hard");
                        Thread.Sleep(1000);
                        Console.Clear();
                    }
                }
            }
        }       

        public void Game(string option, int guesses=7)
        {
            string jsonFilePath = "words.json";
            var (easyWords, mediumWords, hardWords) = ReadWords(jsonFilePath);
            string alreadyGuessed = "";

            Random random = new();
            string correctWord = "";
            Console.WriteLine($"\nYou chose {option}, lets start");

            Thread.Sleep(1000);
            Console.Clear();
            if (option == "easy")
            {
                correctWord = easyWords[random.Next(easyWords.Length)];
            }else if(option == "medium")
            {
                correctWord = mediumWords[random.Next(mediumWords.Length)];
            }else if( option == "hard")
            {
                correctWord = hardWords[random.Next(hardWords.Length)];
            }
            else
            {
                Console.WriteLine("Invalid difficulty");
                OptionSelection();
            }
            string underscores = new string('_', correctWord.Length).Replace("_", "_ ");
            Console.WriteLine("\n"+underscores);

            char[] outputChar = new char[correctWord.Length];
            while (guesses > 0)
            {
                Console.Write("Your input: ");
                var userInput = Console.ReadLine();
                Console.Clear();

                if (userInput == null || userInput == "")
                {
                    Console.WriteLine("You didn't write anything");
                }
                else if (userInput.Length == 1)
                {
                    alreadyGuessed += AlreadyGuessed(userInput, alreadyGuessed);
                    if (correctWord.Contains(userInput))
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"You are correct the word contains: {userInput}");
                        Console.ResetColor();
                        Console.WriteLine($"Already guessed: {alreadyGuessed}");
                        outputChar = UpdateWords(correctWord, userInput, outputChar, guesses);
                        UpdateHangman(guesses, correctWord);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The word doesn't contain: {userInput}");
                        Console.ResetColor();
                        Console.WriteLine($"Already guessed: {alreadyGuessed}");
                        guesses--;
                        Console.WriteLine($"Left guesses: {guesses}");
                        outputChar = UpdateWords(correctWord, userInput, outputChar, guesses);
                        UpdateHangman(guesses, correctWord);
                    }
                }
                else
                {
                    alreadyGuessed += AlreadyGuessed(userInput, alreadyGuessed);
                    if (correctWord == userInput)
                    {
                        Won(guesses, correctWord);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong, this is not the correct word");
                        Console.ResetColor();
                        Console.WriteLine($"Already guessed: {alreadyGuessed}");
                        guesses--;
                        Console.WriteLine($"Left guesses: {guesses}");
                        UpdateHangman(guesses, correctWord);
                    }
                }
            }
        }

        public static char[] UpdateWords(string correctWord, string userInput, char[] outputChar, int guessesLeft)
        {
            char[] wordChars = correctWord.ToCharArray();
            string output = "";

            for (int i = 0; i < wordChars.Length; i++)
            {
                if (outputChar[i] == '\0')
                {
                    outputChar[i] = '_';
                }
                if (wordChars[i] == userInput[0])
                {
                    outputChar[i] = userInput[0];
                }         
            }

            for (int i = 0; i < outputChar.Length; i++)
            {
                output += outputChar[i];
            }
            if(output == correctWord)
            {
                Won(guessesLeft, correctWord);
            }
            if (guessesLeft > 0)
            {
                string outputSpaces = string.Join(" ", output.ToCharArray());
                Console.WriteLine(outputSpaces);
            }
            return outputChar;
        }

        public static void UpdateHangman(int guesses, string correctWord)
        {

            string[] hangmanAnimations = {
            "┌─────┐\n│" + "0\n".PadLeft(7) + "│" + "/|\\".PadLeft(7) + "\n│" + "/ \\".PadLeft(7) + "\n│",
            "┌─────┐\n│" + "0\n".PadLeft(7) + "│" + "/|\\".PadLeft(7) + "\n│" + "/".PadLeft(5) + "\n│",
            "┌─────┐\n│" + "0\n".PadLeft(7) + "│" + "/|\\".PadLeft(7) + "\n│\n│",
            "┌─────┐\n│" + "0\n".PadLeft(7) + "│" + "|\\".PadLeft(7) + "\n│\n│",
            "┌─────┐\n│" + "0\n".PadLeft(7) + "│" + " | ".PadLeft(7) + "\n│\n│",
            "┌─────┐\n│" + "0".PadLeft(6) + "\n│\n│\n│",
            "┌─────┐\n│\n│\n│\n│",
            };
            if (guesses < 7)
            {
                Console.WriteLine(hangmanAnimations[guesses]);
            }
            if(guesses == 0)
            {
                Console.WriteLine(hangmanAnimations[guesses]);
                Lost(correctWord);
            }

        }
        public static void Lost(string correctWord)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You Lost :(");
            string hangmanDeath = "┌─────┐\n│" + "0\n".PadLeft(7) + "│" + "/|\\".PadLeft(7) + "\n│" + "/ \\".PadLeft(7) + "\n│";
            Console.WriteLine(hangmanDeath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The correct word was: {correctWord}\n");
            Console.ResetColor();
            PlayAgain();
        }
        public static void Won(int guessesLeft, string word)
        {
            Assets assets = new();
            Console.Clear();

            float overall = ((word.Length * guessesLeft) / 10f) * 100f;
            string statistics = $"---------- STATISTICS ----------\nGuesses left: {guessesLeft}\nWord length: {word.Length}\nOverall Score: {overall}\n--------------------------------";
            Console.WriteLine("Congratulations! You won");
            Console.WriteLine(statistics+"\n");
            PlayAgain();
        }

        public static void PlayAgain()
        {
            Assets assets = new();
            Console.WriteLine("Do you want to play Again?\nY or N");
            var input = Console.ReadLine();
            if (input != null || input != "")
            {
                input = input.ToLower();

                if (input == "y")
                {
                    Console.Clear();
                    string option = OptionSelection();
                    int guesses = assets.GuessSelection();
                    assets.Game(option, guesses);
                }
                else if (input == "n")
                {
                    Environment.Exit(0);
                }
                else
                {
                    PlayAgain();
                }
            }
        }

        public static string AlreadyGuessed(string input, string alreadyGuessed)
        {
            List<string> guessedStrings = new()
            {
                input
            };
            string output = "";
            for (int i = 0; i < guessedStrings.Count; i++)
            {
                if (!alreadyGuessed.Contains(input))
                {
                    output += guessedStrings[i] + ", ";
                }
            }
            return output;
        }
    }
}
