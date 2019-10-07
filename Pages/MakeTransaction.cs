using SSD.Models;
using SSD.Controllers;
using SSD.Lib;
using System;

namespace SSD.Pages
{
    public class MakeTransaction : IPage
    {
        private Person _user;
        private Currency[] currencies;
        public MakeTransaction(Router r) : base(r)
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
            this._user = p;
        }

        private BankUser[] _getUsersPerName(string Name)
        {
            BankUser[] users = AppController.GetInstance().UserController.GetBankUsersByName(Name);

            return users;
        }

        public override void Render()
        {
            Transactions transaction = new Transactions()
            {
                SenderAccountId = _user.Id
            };

            Console.WriteLine("You are about to make a transaction from your account.");
            Console.WriteLine("First search for a person to transfer funds to:");

            BankUser b = SearchForUsers();
            transaction.ReceiverAccountId = b.Id;

            Currency c = GetCurrency();
            transaction.SmallCurrencyString = c.CurrencyShortName;
            transaction.LargeCurrencyString = c.CurrencyLongName;

            transaction.Amount = GetAmount(c.CurrencyShortName);
            transaction.Timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();

            Transactions.InsertNewObject(transaction);

            Console.WriteLine("Your new transaction has been inserted!");
            Console.WriteLine("You can view it on the dashboard.");
            Console.ReadKey();
            _router.Navigate(Routes.Dashboard, this._user);
        }

        public Currency GetCurrency()
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

        public BankUser SearchForUsers()
        {
            bool rightAnswer = false;
            int chosenUser = -1;
            BankUser chosenBankUser = null;
            bool foundPerson = false;
            string nameInput = "";
            BankUser[] users = null;
            do
            {
                Console.Write("Enter name:");
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

        public decimal GetAmount(string currency)
        {
            decimal amount = 0;
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