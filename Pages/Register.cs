using System;
using SSD.Controllers;
using SSD.Lib;
using SSD.Models;

namespace SSD.Pages
{
    public class Register : IPage
    {
        public Register(Router r) : base(r) { }
        public override void Render()
        {
            string role = "", firstName, secondName, address1, address2, address3, phoneNumber, accountType, managerName = "";
            bool hasManager = false;
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

                do
                {
                    Console.Write("Type in the first name and second name of your manager: ");
                    managerName = Console.ReadLine();

                    try
                    {
                        p = Person.GetPersonByName(managerName);
                    }
                    catch (System.Exception)
                    {
                        p = null;
                    }

                    if (p == null)
                    {
                        Console.Write("Manager not found. Try again? (yes/no): ");
                        string answer = Console.ReadLine();

                        switch (answer.ToLower())
                        {
                            case "n":
                            case "no":
                                hasManager = true;
                                break;
                            default:
                                hasManager = false;

                                ConsoleExtensions.ClearLines(2);
                                break;
                        }
                    }
                } while (!hasManager);

                Console.Write("Enter your branch location: ");
                admin.BranchLocation = Console.ReadLine();
                 
                try
                {
                    p = BankAdmin.InsertNewObject<BankAdmin>(admin);
                }
                catch (System.Exception err)
                {
                    Console.WriteLine(err.Message);
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

            this.SetupLogin(p.Id, p.Role);

            this._router.Navigate(Routes.Dashboard, p);
        }

        public void SetupLogin(string userId, Roles role)
        {
            string email = "",
                password = "";
            Console.Clear();

            Console.WriteLine("Now setup your credentials: \n");
            Console.Write("Email: ");
            email = Console.ReadLine();
            Console.Write("Password: ");
            password = Console.ReadLine();

            LoginDetails l = new LoginDetails()
            {
                UserId = userId,
                Role = role,
                Password = password,
                Email = email
            };

            try
            {
                LoginDetails.InsertNewObject<LoginDetails>(l);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public void SetupAdminProfile(ref Person admin)
        {

        }
    }
}