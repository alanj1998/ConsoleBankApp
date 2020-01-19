using System;
using System.Security;
using SSD.Controllers;
using SSD.Models;

namespace SSD.Pages
{
    internal sealed class Login : IPage
    {
        internal Login(Router r) : base(r) { }

        private bool DoLogin(string username, SecureString password, out Person p)
        {
            try
            {
                Person answer = AppController.GetInstance().LoginController.Login(username, password);
                password.Dispose();
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                p = null;
                return false;
            }
        }

        internal override void Render()
        {
            Person p;
            bool isLoggedIn;

            do
            {
                Console.Clear();
                Console.WriteLine("Enter your username and password to login into the system: ");
                Console.WriteLine("Hint: Use :q to exit this screen...");
                Console.Write("Username: ");
                string username = Console.ReadLine();

                if (username.Trim().ToLower() == ":q")
                {
                    isLoggedIn = true;
                    this._router.Navigate(Routes.Splash);
                }
                else
                {
                    Console.Write("Password: ");
                    SecureString password = new SecureString();

                    bool firstOne = true;
                    while (true)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.Spacebar && firstOne)
                        {
                            isLoggedIn = true;
                            this._router.Navigate(Routes.Splash);
                            break;
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (password.Length > 0)
                            {
                                password.RemoveAt(password.Length - 1);
                                Console.Write("\b");
                                Console.Write(' ');
                                Console.Write("\b");
                            }
                        }
                        else
                        {
                            password.AppendChar(key.KeyChar);
                            Console.Write('*');
                        }

                        if (firstOne) firstOne = false;
                    }
                    Console.WriteLine();

                    isLoggedIn = this.DoLogin(username, password, out p);
                    password.Dispose();

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
            while (!isLoggedIn);
        }
    }
}