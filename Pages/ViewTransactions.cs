using SSD.Models;
using SSD.Controllers;
using System;
using SSD.Lib;

namespace SSD.Pages
{
    public class ViewTransactions : IPage
    {
        public Person user;
        public ViewTransactions(Router r) : base(r) { }

        public void AddModel(Person p)
        {
            this.user = p;
        }
        public override void Render()
        {
            if (user.Role == Roles.User)
            {
                Transactions[] t = AppController.GetInstance().TransactionController.GetTransactionsForUser(this.user.Id);
                Array.Sort(t, delegate (Transactions x, Transactions y) { return y.Timestamp.CompareTo(x.Timestamp); });

                Console.WriteLine($"Transactions for {user.FirstName} {user.LastName}");
                if (t.Length > 0)
                {
                    RenderTable(t);
                }
                else
                {
                    Console.WriteLine("No transactions for this account.");
                }
            }
            else if (user.Role == Roles.Admin)
            {
                string name = "";
                BankUser chosenUser = null;
                BankUser[] users = null;
                bool isDone = false;
                bool rightAnswer = false;
                int chosenIndex = -1;

                do
                {
                    Console.Write("Enter the name of a user to search for: ");
                    name = Console.ReadLine();

                    users = BankUser.GetBankUsersByName(name);

                    if (users.Length < 1)
                    {
                        do
                        {
                            Console.Write("No users found! Try Again? (y/n) ");
                            name = Console.ReadLine();

                            if (name.ToLower() == "y")
                            {
                                ConsoleExtensions.ClearLines(2);
                                rightAnswer = true;
                            }
                            else if (name.ToLower() == "n")
                            {
                                rightAnswer = true;
                                _router.Navigate(Routes.Dashboard, this.user);
                            }
                            else
                            {
                                Console.WriteLine("Wrong choice!");
                                Console.ReadKey();
                                ConsoleExtensions.ClearLines(2);
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
                            if (int.TryParse(Console.ReadLine(), out chosenIndex))
                            {
                                if (chosenIndex - 1 < 0 || chosenIndex - 1 >= users.Length)
                                {
                                    Console.WriteLine("Bad answer! Try again! ");
                                    Console.ReadKey();
                                    ConsoleExtensions.ClearLines(users.Length + 3);
                                }
                                else
                                {
                                    chosenUser = users[chosenIndex - 1];
                                    isDone = true;
                                    rightAnswer = true;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Bad answer! Try again! ");
                                Console.ReadKey();
                                ConsoleExtensions.ClearLines(users.Length + 3);
                            }
                        } while (!rightAnswer);
                    }
                } while (!isDone);

                Transactions[] t = AppController.GetInstance().TransactionController.GetTransactionsForUser(chosenUser.Id);
                Array.Sort(t, delegate (Transactions x, Transactions y) { return y.Timestamp.CompareTo(x.Timestamp); });

                Console.WriteLine($"Transactions for {user.FirstName} {user.LastName}");
                if (t.Length > 0)
                {
                    RenderTable(t);
                }
                else
                {
                    Console.WriteLine("No transactions for this account.");
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            this._router.Navigate(Routes.Dashboard, this.user);
        }

        public void RenderTable(Transactions[] transactions)
        {
            AddLine();
            AddRow("", "Date", "Sender", "Receiver", "Currency", "Amount");
            AddLine();
            for (int i = 0; i < transactions.Length; i++)
            {
                Transactions t = transactions[i];
                AddRow($"{i + 1}.", new DateTime(t.Timestamp).ToString("dd/MM/yyyy hh:mm tt"), $"{t.SenderAccount.FirstName} {t.SenderAccount.LastName}", $"{t.ReceiverAccount.FirstName} {t.ReceiverAccount.LastName}", $"{t.SmallCurrencyString}", $"{t.Amount}");
                AddLine();
            }
        }

        private void AddLine()
        {
            Console.WriteLine("".PadLeft(98, '-'));
        }

        private void AddRow(string val1, string val2, string val3, string val4, string val5, string val6)
        {
            Console.WriteLine("|{0,-3}|{1,-20}|{2,-25}|{3,-25}|{4,-8}|{5,-10}|", val1, val2, val3, val4, val5, val6);
        }
    }
}