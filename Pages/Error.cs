using System;

namespace SSD.Pages
{
    public class Error : IPage
    {
        public Error(Router r) : base(r) { }

        public override void Render()
        {
            Console.WriteLine("You tried to enter a route that does not exist!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
            this._router.Navigate(Routes.Splash);
        }
    }
}