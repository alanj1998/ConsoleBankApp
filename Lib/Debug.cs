using System;

namespace SSD.Lib
{
    public class Debug
    {
        public static void StartDebug()
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" END DEBUG ".PadLeft(10, '#').PadRight(10, '#'));
            Console.ForegroundColor = c;
        }

        public static void EndDebug()
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" END DEBUG ".PadLeft(10, '#').PadRight(10, '#'));
            Console.ForegroundColor = c;
        }

        public static void Info(string filename, int lineNumber, string text)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($" {filename}:{lineNumber.ToString()} {text} ".PadLeft(10, '#').PadRight(10, '#'));
            Console.ForegroundColor = c;
        }
    }

    public class ConsoleExtensions
    {
        public static void ClearLines(int lineNumber)
        {
            for (int i = 0; i < lineNumber; i++)
            {
                Console.CursorTop = Console.CursorTop - 1;
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            }
        }
    }
}