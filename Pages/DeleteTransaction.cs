using SSD.Models;
using System;
using SSD.Lib;
using SSD.Controllers;

namespace SSD.Pages
{
    public class DeleteTransaction : IPage
    {

        public Person user;

        public void AddModel(Person p)
        {
            this.user = p;
        }
        public DeleteTransaction(Router r) : base(r)
        {
        }

        public override void Render()
        {
            bool isRightAnswer = false;
            int chosenIndex = -1;
            Transactions chosenTransaction = null;

            Console.WriteLine("Deleting a transaction");
            BankUser u = SearchForUsers();
            Transactions[] transactions = GetTransactions(u.Id);

            Console.WriteLine($"Transactions for {u.FirstName} {u.LastName}");
            RenderTable(transactions);

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

            AppController.GetInstance().TransactionController.DeleteTransaction(chosenTransaction.Id);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            this._router.Navigate(Routes.Dashboard, this.user);
        }

        public Transactions[] GetTransactions(string userId)
        {
            return AppController.GetInstance().TransactionController.GetTransactionsForUser(userId);
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

        public BankUser SearchForUsers(string name = "")
        {
            bool rightAnswer = false;
            int chosenUser = -1;
            BankUser chosenBankUser = null;
            bool foundPerson = false;
            string nameInput = "";
            BankUser[] users = null;
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
                }
            } while (!foundPerson);

            return chosenBankUser;
        }
    }
}