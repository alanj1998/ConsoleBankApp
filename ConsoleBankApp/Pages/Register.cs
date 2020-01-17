using System;
using System.Security;
using SSD.Controllers;
using SSD.Lib;
using SSD.Models;

namespace SSD.Pages
{
    internal sealed class Register : IPage
    {
        internal Register(Router r) : base(r) { }
        internal override void Render()
        {
            string role = "", firstName, secondName, address1, address2, address3, phoneNumber, accountType;
            Person p = null;

            Console.WriteLine("Fill out this form to register:\n");

            string[] options = new string[] { "Admin", "User" };
            Menu m = new Menu(options);
            int resposne = m.RenderMenu();

            switch (resposne)
            {
                case 1:
                    role = "Admin";
                    break;
                case 2:
                default:
                    role = "User";
                    break;
            }

            Console.Write("First Name: ");
            firstName = Console.ReadLine();

            Console.Write("Second Name: ");
            secondName = Console.ReadLine();

            Console.Write("Address 1: ");
            address1 = Console.ReadLine();

            Console.Write("Address 2: ");
            address2 = Console.ReadLine();

            Console.Write("Address 3: ");
            address3 = Console.ReadLine();

            Console.Write("Phone Number: ");
            phoneNumber = Console.ReadLine();

            if (role == "Admin")
            {
                BankAdmin admin = new BankAdmin()
                {
                    FirstName = firstName,
                    LastName = secondName,
                    Address1 = address1,
                    Address2 = address2,
                    Address3 = address3,
                    Role = (Roles)Enum.Parse(typeof(Roles), role),
                    PhoneNumber = phoneNumber
                };

                Console.Write("Enter your branch location: ");
                admin.BranchLocation = Console.ReadLine();
                 
                try
                {
                    p = BankAdmin.InsertNewObject<BankAdmin>(admin);
                }
                catch (System.Exception err)
                {
                    Console.WriteLine(err.Message);
                    throw err;
                }
            }
            else
            {
                Console.Write("Enter account type: ");
                accountType = Console.ReadLine();

                BankUser user = new BankUser()
                {
                    AccountType = accountType,
                    Address1 = address1,
                    Address2 = address2,
                    Address3 = address3,
                    FirstName = firstName,
                    LastName = secondName,
                    PhoneNumber = phoneNumber,
                    Role = (Roles)Enum.Parse(typeof(Roles), role)
                };

                try
                {
                    p = BankUser.InsertNewObject<BankUser>(user);
                }
                catch (System.Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }

            bool result = this.SetupLogin(p);

            if (result) this._router.Navigate(Routes.Dashboard, p);
            else
            {
                Console.WriteLine("There has been an error setting up your credentials...");
                this._router.Navigate(Routes.Splash);
            }
        }

        private bool SetupLogin(Person p)
        {
            string email;
            SecureString password = new SecureString();
            Console.Clear();

            Console.WriteLine("Now setup your credentials: \n");
            Console.Write("Email: ");
            email = Console.ReadLine();
            Console.Write("Password: ");

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
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

            }
            Console.WriteLine();

            return AppController.GetInstance().LoginController.Register(email, password, p);
        }
    }
}