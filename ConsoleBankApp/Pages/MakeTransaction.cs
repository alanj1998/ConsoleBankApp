using SSD.Models;
using SSD.Controllers;
using SSD.Lib;
using System;

namespace SSD.Pages
{
    internal class MakeTransaction : IPage
    {
        private Person _user;
        private Currency[] currencies;
        internal MakeTransaction(Router r) : base(r)
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

        public void AddModel(Person p)
        {
            if ((p is BankAdmin) || (p is BankUser))
            {
                this._user = p;
            }
            else
            {
                throw new Exception("Error in AddModel. Attempt to inject a user which is not of the required class type!");
            }
        }

        internal override void Render()
        {
            Transactions transaction = new Transactions();
            BankUser sender = null;
            BankUser receiver;

            bool adminUserCheck = false;

            if (_user.Role == Roles.Admin)
            {
                Console.WriteLine("You are about to make a transation as an Admin.");
                Console.WriteLine("Search for a person to transfer funds from: ");
                sender = SearchForUsers();

                if(sender == null)
                {
                    return;
                }
                else if (sender.GetType() == typeof(BankUser))
                {
                    transaction.SenderAccountId = sender.Id;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Console.WriteLine("You are about to make a transaction from your account.");
                transaction.SenderAccountId = _user.Id;
            }

            do
            {
                Console.WriteLine("Search for a person to transfer funds to:");

                receiver = SearchForUsers();
                if (receiver == null)
                {
                    return;
                }
                if (receiver.GetType() == typeof(BankUser))
                {
                    transaction.ReceiverAccountId = receiver.Id;
                }
                else
                {
                    return;
                }


                if (sender.Id == receiver.Id)
                {
                    Console.WriteLine("You cannot make a transaction to yourself!");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    ConsoleExtensions.ClearLines(3);
                }
                else adminUserCheck = true;
            } while (!adminUserCheck);
            Helpers.FreeAndNil(ref receiver);
            Helpers.FreeAndNil(ref sender);

            Currency c = GetCurrency();
            transaction.SmallCurrencyString = c.CurrencyShortName;
            transaction.LargeCurrencyString = c.CurrencyLongName;

            transaction.Amount = GetAmount(c.CurrencyShortName);
            transaction.Timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();

            Transactions.InsertNewObject(transaction);

            Console.WriteLine("Your new transaction has been inserted!");
            Console.WriteLine("You can view it on the dashboard.");
            Console.ReadKey();

            Helpers.FreeAndNil(ref transaction);
            _router.Navigate(Routes.Dashboard, this._user);
        }

        private Currency GetCurrency()
        {
            bool rightAnswer = false;
            int selectedCurrency = -1;
            Currency c = null;

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

            return c;
        }

        private BankUser SearchForUsers()
        {
            bool rightAnswer = false;
            int chosenUser;
            BankUser chosenBankUser = null;
            bool foundPerson = false;
            string nameInput;
            BankUser[] users;
            do
            {
                Console.Write("Enter name: ");
                nameInput = Console.ReadLine();
                users = BankUser.GetBankUsersByName(nameInput);

                if (users.Length == 0)
                {
                    do
                    {
                        Console.Write("No users found! Try Again? (y/n) ");
                        nameInput = Console.ReadLine();

                        if (nameInput.ToLower() == "y")
                        {
                            ConsoleExtensions.ClearLines(2);
                            rightAnswer = true;
                        }
                        else if (nameInput.ToLower() == "n")
                        {
                            rightAnswer = true;
                            _router.Navigate(Routes.Dashboard, this._user);
                            return null;
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
                        if (int.TryParse(Console.ReadLine(), out chosenUser))
                        {
                            if (chosenUser - 1 < 0 || chosenUser - 1 >= users.Length)
                            {
                                Console.WriteLine("Bad answer! Try again! ");
                                Console.ReadKey();
                                ConsoleExtensions.ClearLines(users.Length + 3);
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
                            ConsoleExtensions.ClearLines(users.Length + 3);
                        }
                    } while (!rightAnswer);
                }
            } while (!foundPerson);

            return chosenBankUser;
        }

        private decimal GetAmount(string currency)
        {
            decimal amount;
            bool rightAnswer = false;
            do
            {
                Console.Write($"\nEnter Amount: {currency}");
                if (decimal.TryParse(Console.ReadLine(), out amount))
                {
                    rightAnswer = true;
                }
                else
                {
                    Console.WriteLine("Bad answer! Try again! ");
                    Console.ReadKey();
                    ConsoleExtensions.ClearLines(2);
                }
            } while (!rightAnswer);

            return amount;
        }
    }
}