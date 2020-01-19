using SSD.Models;
using SSD.Lib;
using SSD.Controllers;
using System;
using System.Reflection;

namespace SSD.Pages
{
    internal sealed class UpdateTransactions : IPage
    {
        private Person user;
        private Currency[] currencies;

        internal void AddModel(Person p)
        {
            this.user = p;
        }
        internal UpdateTransactions(Router r) : base(r)
        {
            currencies = new Currency[] {
                new Currency(){
                    CurrencyShortName = "EUR",
                    CurrencyLongName = "Euro"
                },
                new Currency() {
                    CurrencyLongName = "Swiss Frank",
                    CurrencyShortName = "CHF"
                },
                new Currency() {
                    CurrencyLongName = "American Dollars",
                    CurrencyShortName = "USD"
                },
                new Currency() {
                    CurrencyLongName = "Norwegian Krona",
                    CurrencyShortName = "NOK"
                },
                new Currency() {
                    CurrencyLongName = "Polish Zloty",
                    CurrencyShortName = "PLN"
                },
                new Currency() {
                    CurrencyLongName = "Chinese Yuan",
                    CurrencyShortName = "CNY"
                },
                new Currency() {
                    CurrencyLongName = "Russian Ruble",
                    CurrencyShortName = "RUB"
                }
            };
        }

        internal override void Render()
        {
            bool isRightAnswer = false;
            int chosenIndex;
            Transactions chosenTransaction = null;

            Console.WriteLine("Updating a transaction");
            BankUser u = SearchForUsers();
            Transactions[] transactions = GetTransactions(u.Id);

            Console.WriteLine($"Transactions for {u.FirstName} {u.LastName}");
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

            UpdateChosenTransaction(chosenTransaction);
            Helpers.FreeAndNil(ref chosenTransaction);
            Helpers.FreeAndNil(ref transactions);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        
            this._router.Navigate(Routes.Dashboard, this.user);
        }

        private void UpdateChosenTransaction(Transactions t)
        {
            string input;
            bool rightAnswer = false;
            decimal amount;

            Console.Write($"Sender ({((t.SenderAccount != null) ? t.SenderAccount.FirstName + " " + t.SenderAccount.LastName : "")}): ");
            input = Console.ReadLine();
            if (input != "")
            {
                BankUser u = SearchForUsers(input);
                t.SenderAccount = u;
                t.SenderAccountId = u.Id;
            }

            Console.Write($"Receiver ({((t.SenderAccount != null) ? t.SenderAccount.FirstName + " " + t.SenderAccount.LastName : "")}): ");
            input = Console.ReadLine();
            if (input != "")
            {
                BankUser u = SearchForUsers(input);
                t.ReceiverAccount = u;
                t.ReceiverAccountId = u.Id;
            }

            do
            {
                Console.Write($"Currency ({t.SmallCurrencyString}): ");
                input = Console.ReadLine();
                if (input != "")
                {
                    Currency c = GetCurrency(input);

                    if (c == null)
                    {
                        Console.WriteLine("Bad answer try again!");
                        Console.ReadKey();
                        ConsoleExtensions.ClearLines(2);
                    }
                    else
                    {
                        t.SmallCurrencyString = c.CurrencyShortName;
                        t.LargeCurrencyString = c.CurrencyLongName;
                        rightAnswer = true;
                    }
                }
                else
                {
                    rightAnswer = true;
                }
            } while (!rightAnswer);
            rightAnswer = false;

            do
            {
                Console.Write($"Amount ({t.SmallCurrencyString}{t.Amount}): {t.SmallCurrencyString}");
                input = Console.ReadLine();

                if (input == "")
                {
                    rightAnswer = true;
                }
                else if (decimal.TryParse(input, out amount))
                {
                    t.Amount = amount;
                    rightAnswer = true;
                }
                else
                {
                    Console.WriteLine("Bad answer. Try again!");
                    Console.ReadLine();
                    ConsoleExtensions.ClearLines(2);
                }
            } while (!rightAnswer);

            AppController.GetInstance().TransactionController.UpdateTransaction(t);

            Helpers.FreeAndNil(ref t);
        }

        private Transactions[] GetTransactions(string userId)
        {
            return AppController.GetInstance().TransactionController.GetTransactionsForUser(userId);
        }

        private Currency GetCurrency(string currency = "")
        {
            bool rightAnswer = false;
            int selectedCurrency = -1;
            Currency c = null;

            if (currency == "")
            {
                do
                {
                    Console.WriteLine("Select currency to send your payment in:");

                    for (int i = 0; i < currencies.Length; i++)
                    {
                        Console.WriteLine($"{i + 1}) {currencies[i].CurrencyLongName}");
                    }
                    Console.Write("Select your currency: ");
                    if (int.TryParse(Console.ReadLine(), out selectedCurrency))
                    {
                        if (selectedCurrency - 1 < 0 || selectedCurrency - 1 > currencies.Length)
                        {
                            Console.WriteLine("Bad answer! Try again! ");
                            Console.ReadKey();
                            ConsoleExtensions.ClearLines(currencies.Length + 3);
                        }
                        else
                        {
                            c = currencies[selectedCurrency - 1];
                            rightAnswer = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Bad answer! Try again! ");
                        Console.ReadKey();
                        ConsoleExtensions.ClearLines(currencies.Length + 3);
                    }
                } while (!rightAnswer);
            }
            else
            {
                Currency curr = null;
                for (int i = 0; i < currencies.Length; i++)
                {
                    if (currencies[i].CurrencyShortName.ToString().ToLower() == currency.ToLower())
                    {
                        curr = currencies[i];
                        break;
                    }
                }

                c = curr;
            }

            return c;
        }

        private BankUser SearchForUsers(string name = "")
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

                            Helpers.FreeAndNil(ref users);
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

            Helpers.FreeAndNil(ref users);
            Helpers.FreeAndNil(ref user);
            return chosenBankUser;
        }

    }
}