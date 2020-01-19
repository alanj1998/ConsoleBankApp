using SSD.Controllers;
using SSD.Lib;
using SSD.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSD.Pages
{
    internal sealed class ViewLogs: IPage
    {
        private Person p;
        internal ViewLogs(Router r) : base(r) { }

        internal void AddModel(Person p)
        {
            this.p = p;
        }
        internal override void Render()
        {
            int offset = 0;
            ConsoleKeyInfo consoleKeyInfo;

            do
            {
                Console.WriteLine("Use <- to go back and -> to go forward. Use escape to leave.");

                Console.WriteLine($"Logs [{offset * 5} - {GetFinalNumber(offset)}]");
                LoggerController.Get5LinesOfLog(offset).ForEach((logString) =>
                {
                    Console.WriteLine(Helpers.ConvertFromSecureToNormalString(logString).Replace("\n", ""));
                });
                Console.Write("> ");

                consoleKeyInfo = Console.ReadKey();

                if(consoleKeyInfo.Key == ConsoleKey.LeftArrow) 
                {
                    offset -= 1;

                    if(offset < 0)
                    {
                        offset = 0;
                    }
                }
                else if(consoleKeyInfo.Key == ConsoleKey.RightArrow)
                {
                    if((offset + 1) * 5 < LoggerController.GetCountOfLogs())
                    {
                        offset += 1;
                    }
                }
                Console.Clear();
            } while (consoleKeyInfo.Key != ConsoleKey.Escape);

            this._router.Navigate(Routes.Dashboard, p);
        }

        private int GetFinalNumber(int offset)
        {
            int lastNumber = LoggerController.GetCountOfLogs();

            if((offset + 1) * 5 < lastNumber)
            {
                return (offset + 1) * 5;
            }
            else
            {
                return lastNumber;
            }
        }
    }
}
