using System.Collections.Generic;
using System;
using SSD.Models;
using SSD.Lib;

namespace SSD.Pages
{
    internal sealed class Dashboard : IPage
    {
        private Person person;
        internal Dashboard(Router r) : base(r) { }

        internal void AddModel(Person model)
        {
            Console.WriteLine(model.FirstName);
            this.person = model;
        }

        internal override void Render()
        {
            Roles chosenRole = person.Role;

            RenderDashboardForChosenRole(chosenRole);
        }

        private void RenderDashboardForChosenRole(Roles role)
        {
            List<string> options = new List<string>();
            int response;
            string header = "";

            if (role == Roles.Admin)
            {
                header = $"Hello {this.person.FirstName} {this.person.LastName}. Your Branch Location is {(person as BankAdmin).BranchLocation}";

                options.Add("Make Transaction");
                options.Add("View Transactions");
                options.Add("Update Transactions");
                options.Add("Delete Transactions");
                options.Add("Logout");
            }
            else if (role == Roles.User)
            {
                header = $"Hello {person.FirstName} {person.LastName}. Your account type is {(person as BankUser).AccountType}";

                options.Add("Make Transaction");
                options.Add("View Transactions");
                options.Add("Logout");
            }

            Menu m = new Menu(options.ToArray());
            bool goodResponse = false;

            do
            {
                response = m.RenderMenu(header);
                switch (response)
                {
                    case -1:
                    case 5:
                        this._router.Navigate(Routes.Splash);
                        goodResponse = true;
                        break;
                    case 1:
                        this._router.Navigate(Routes.MakeTransaction, this.person);
                        goodResponse = true;
                        break;
                    case 2:
                        this._router.Navigate(Routes.ViewTransaction, this.person);
                        goodResponse = true;
                        break;
                    case 3:
                        if (role == Roles.User)
                        {
                            this._router.Navigate(Routes.Splash);
                            goodResponse = true;
                        }
                        else if (role == Roles.Admin)
                        {
                            this._router.Navigate(Routes.UpdateTransaction, this.person);
                            goodResponse = true;
                        }
                        break;
                    case 4:
                        if (role == Roles.User)
                        {
                            Console.WriteLine("Bad choice! Try again...");
                            Console.ReadKey();
                            Console.Clear();
                            goodResponse = false;
                        }
                        else if (role == Roles.Admin)
                        {
                            this._router.Navigate(Routes.DeleteTransaction, this.person);
                            goodResponse = true;
                        }
                        break;
                    default:
                        Console.WriteLine("Bad choice! Try again...");
                        Console.ReadKey();
                        Console.Clear();
                        goodResponse = false;
                        break;
                }
            } while (!goodResponse);
        }
    }
}