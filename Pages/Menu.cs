using System;

namespace SSD.Pages
{
    public class Menu
    {
        private string[] options { get; set; }

        public Menu(string[] options)
        {
            this.options = options;
        }

        public int RenderMenu()
        {
            bool isDone = false;
            int response = 0;
            string menuChoice = "";

            do
            {
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
                        Console.Clear();
                    }
                }
            }
            while (!isDone);

            return response;
        }
    }
}