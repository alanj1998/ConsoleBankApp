using SSD.Lib;

namespace SSD.Models
{
    public class BankUser : Person
    {
        public string AccountType { get; set; }

        public BankUser() { }

        public static BankUser[] GetBankUsersByName(string Name)
        {
            BankUser[] users = SQL.GetInstance().Select<BankUser>($"lower(FirstName || ' ' || LastName) = lower(\"{Name}\");");

            return users;
        }
    }
}