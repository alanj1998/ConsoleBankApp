using SSD.Models;
using System;
using SSD.Lib;
using SSD.Controllers;
using System.Text.RegularExpressions;

namespace SSD.Pages
{
    internal sealed class DeleteTransaction : IPage
    {

        private Person user;

        internal void AddModel(Person p)
        {
            this.user = p;
        }
        internal DeleteTransaction(Router r) : base(r)
        {
        }

        internal override void Render()
        {
            bool isRightAnswer = false;
            int chosenIndex = -1;
            Transactions chosenTransaction = null;

            Console.WriteLine("Deleting a transaction");
            BankUser u = SearchForUsers();
            Transactions[] transactions = GetTransactions(u.Id);

            Console.WriteLine($"Transactions for {u.FirstName} {u.LastName}");
            Helpers.FreeAndNil(ref u);

            TableHelpers.RenderTable(transactions);

            do
            {
                Console.Write("Select the transaction to edit by writing it's number: ");
                if (int.TryParse(Console.ReadLine(), out chosenIndex))
                {
                    if (chosenIndex - 1 < 0 || chosenIndex - 1 >= transactions.Length)
                    {
                        Console.WriteLine("Bad answer! Try again! ");
                        Console.ReadKey();
                        ConsoleExtensions.ClearLines(transactions.Length + 1);
                    }
                    else
                    {
                        chosenTransaction = transactions[chosenIndex - 1];
                        isRightAnswer = true;
                    }
                }
                else
                {
                    Console.WriteLine("Bad answer! Try again! ");
                    Console.ReadKey();
                    ConsoleExtensions.ClearLines(transactions.Length + 1);
                }
            } while (!isRightAnswer);
            Helpers.FreeAndNil(ref transactions);

            if (chosenTransaction.ReceiverAccountId != this.user.Id && chosenTransaction.SenderAccountId != this.user.Id)
            {
                Helpers.FreeAndNil(ref chosenTransaction);
                Console.WriteLine("You attempted to delete a transaction that does not belong to you! Quitting the app...");
                Environment.Exit(1);
            }
            else
            {
                AppController.GetInstance().TransactionController.DeleteTransaction(chosenTransaction.Id);
                Helpers.FreeAndNil(ref chosenTransaction);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                this._router.Navigate(Routes.Dashboard, this.user);
            }
        }

        private Transactions[] GetTransactions(string userId)
        {
            Transactions[] t = AppController.GetInstance().TransactionController.GetTransactionsForUser(userId);

            return t;
        }

        private BankUser SearchForUsers(string name = "")
        {
            bool rightAnswer = false;
            int chosenUser;
            BankUser chosenBankUser = null;
            bool foundPerson = false;
            string nameInput;
            BankUser[] users;
            do
            {
                if (name != "")
                {
                    nameInput = name;
                }
                else
                {
                    Console.Write("Enter name: ");
                    nameInput = Console.ReadLine();
                }

                if(!Regex.IsMatch(nameInput, "[A-Za-z]{1,} {0,1}[A-Za-z]"))
                {
                    Console.WriteLine("Badly formatted name! Try Again!");
                    ConsoleExtensions.ClearLines(3);
                    continue;
                } 
                users = BankUser.GetBankUsersByName(nameInput);

                if (users.Length == 0)
                {
                    do
                    {
                        Console.Write("No users found! Try Again? (y/n) ");
                        nameInput = Console.ReadLine();

                        if (nameInput.ToLower() == "y")
                        {
                            ConsoleExtensions.ClearLines(3);
                            rightAnswer = true;
                        }
                        else if (nameInput.ToLower() == "n")
                        {
                            rightAnswer = true;
                            _router.Navigate(Routes.Dashboard, this.user);
                        }
                        else
                        {
                            Console.WriteLine("Wrong choice!");
                            Console.ReadKey();
                            ConsoleExtensions.ClearLines(3);
                        }
                    } while (!rightAnswer);
                }
                else
                {
                    do
                    {
                        Console.WriteLine("The System Found these users:");
                        for (int i = 0; i < users.Length; i++)
                        {
                            BankUser user = users[i];
                            Console.WriteLine($"{i + 1}) {user.FirstName} {user.LastName}, {user.Address1}, {user.Address2}, {user.Address3}");
                        }

                        Console.Write("\nSelect your user: ");
                        if (int.TryParse(Console.ReadLine(), out chosenUser))
                        {
                            if (chosenUser - 1 < 0 || chosenUser - 1 >= users.Length)
                            {
                                Console.WriteLine("Bad answer! Try again! ");
                                Console.ReadKey();
                                ConsoleExtensions.ClearLines(users.Length + 4);
                            }
                            else
                            {
                                chosenBankUser = users[chosenUser - 1];
                                rightAnswer = true;
                                foundPerson = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bad answer! Try again! ");
                            Console.ReadKey();
                            ConsoleExtensions.ClearLines(users.Length + 4);
                        }
                    } while (!rightAnswer);

                    Helpers.FreeAndNil(ref user);
                    Helpers.FreeAndNil(ref users);
                }
            } while (!foundPerson);

            return chosenBankUser;
        }
    }
}