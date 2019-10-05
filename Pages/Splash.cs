using System;

namespace SSD.Pages
{
    public class Splash : IPage
    {
        public Splash(Router r) : base(r) { }

        public override void Render()
        {
            string[] options = new string[] { "Login", "Register" };
            Menu m = new Menu(options);
            int menuResponse = 0;

            Console.WriteLine("Welcome to Secure Bank Service App!");
            Console.WriteLine("Choose one of the following options: ");
            Console.WriteLine("Hint: you can use :q to exit the app.");
            menuResponse = m.RenderMenu();

            switch (menuResponse)
            {
                case 1:
                    this._router.Navigate(Routes.Login);
                    break;
                case 2:
                    this._router.Navigate(Routes.Register);
                    break;
                case -1:
                    Console.WriteLine("\nThank you for using the app!");
                    Console.WriteLine("Press any key to continue...");
                    break;
                default:
                    throw new Exception("Should not be here! Check Menu code!");
            }
        }
    }
}