using System.Collections.Generic;
using System;
using SSD.Models;

namespace SSD.Pages
{
    public class Dashboard : IPage
    {
        private Person person;
        public Dashboard(Router r) : base(r) { }

        public void AddModel(Person model)
        {
            Console.WriteLine(model.FirstName);
            this.person = model;
        }

        public override void Render()
        {
            Roles chosenRole = (Roles)person.Role;

            RenderDashboardForChosenRole(chosenRole);
        }

        public void RenderDashboardForChosenRole(Roles role)
        {
            List<string> options = new List<string>();
            int response = 0;

            if (role == Roles.Admin)
            {
                Console.WriteLine($"Hello {this.person.FirstName} {this.person.LastName}. Your Branch Location is {(person as BankAdmin).BranchLocation}");

                options.Add("Make Transaction");
                options.Add("View Transactions");
                options.Add("Update Transactions");
                options.Add("Delete Transactions");
                options.Add("Logout");
            }
            else if (role == Roles.User)
            {
                Console.WriteLine($"Hello {person.FirstName} {person.LastName}. Your account type is {(person as BankUser).AccountType}");

                options.Add("Make Transaction");
                options.Add("View Transactions");
                options.Add("Logout");
            }

            Menu m = new Menu(options.ToArray());
            response = m.RenderMenu();

            switch (response)
            {
                case -1:
                case 5:
                    this._router.Navigate(Routes.Splash);
                    break;
                case 3:
                    if (role == Roles.User)
                    {
                        this._router.Navigate(Routes.Splash);
                    }
                    break;
            }
        }
    }
}