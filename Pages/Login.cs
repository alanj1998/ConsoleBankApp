using System;
using SSD.Controllers;
using SSD.Lib;
using SSD.Models;

namespace SSD.Pages
{
    public class Login : IPage
    {
        public Login(Router r) : base(r) { }

        public bool DoLogin(string username, string password, out Person p)
        {
            try
            {
                Person answer = AppController.GetInstance().LoginController.Login(username, password);
                if (answer == null)
                {
                    Console.WriteLine("Wrong Email or Password");

                    p = null;
                    return false;
                }
                else
                {
                    p = answer;
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                p = null;
                return false;
            }
        }

        public override void Render()
        {
            string username = "";
            string password = "";
            bool isLoggedIn = false;
            Person p;

            do
            {
                Console.Clear();
                Console.WriteLine("Enter your username and password to login into the system: ");
                Console.WriteLine("Hint: Use :q to exit this screen...");
                Console.Write("Username: ");
                username = Console.ReadLine();

                if (username.Trim().ToLower() == ":q")
                {
                    isLoggedIn = true;
                    this._router.Navigate(Routes.Splash);
                }
                else
                {
                    Console.Write("Password: ");
                    password = Console.ReadLine();

                    if (password.Trim().ToLower() == ":q")
                    {
                        isLoggedIn = true;
                        this._router.Navigate(Routes.Splash);
                    }
                    else
                    {
                        isLoggedIn = this.DoLogin(username, password, out p);

                        if (!isLoggedIn)
                        {
                            Console.WriteLine("\nFailed to Login! Username or Password is incorrect!");
                        }
                        else
                        {
                            Console.WriteLine("\nSuccessfully logged in!");
                            this._router.Navigate(Routes.Dashboard, p);
                        }
                        Console.ReadLine();
                    }
                }
            }
            while (!isLoggedIn);
        }
    }
}