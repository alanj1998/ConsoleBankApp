using System;
using SSD.Lib;

namespace SSD.Pages
{
    internal sealed class Menu
    {
        private string[] options { get; set; }

        internal Menu(string[] options)
        {
            this.options = options;
        }

        internal int RenderMenu(string header = "")
        {
            bool isDone = false;
            int response = 0;
            string menuChoice = "";

            do
            {
                if (header != "")
                {
                    Console.WriteLine(header);
                }
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine($"{i + 1}) {options[i]}");
                }

                Console.Write("> ");
                menuChoice = Console.ReadLine();

                if (menuChoice.Trim().ToLower() == ":q")
                {
                    return -1;
                }

                isDone = int.TryParse(menuChoice, out response);

                if (response != 0)
                {
                    if (response > options.Length)
                    {
                        Console.WriteLine("You have chosen a bad menu option. Try again!");
                        isDone = false;
                        Console.Read();
                        ConsoleExtensions.ClearLines(1 + options.Length);
                    }
                }
            }
            while (!isDone);

            return response;
        }
    }
}