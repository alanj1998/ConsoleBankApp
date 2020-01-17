using System;

namespace SSD.Pages
{
    internal sealed class Error : IPage
    {
        internal Error(Router r) : base(r) { }

        internal override void Render()
        {
            Console.WriteLine("You tried to enter a route that does not exist!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
            this._router.Navigate(Routes.Splash);
        }
    }
}