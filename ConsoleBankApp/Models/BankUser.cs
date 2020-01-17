using SSD.Lib;

namespace SSD.Models
{
    internal sealed class BankUser : Person
    {
        internal string AccountType { get; set; }

        internal BankUser() { }

        internal static BankUser[] GetBankUsersByName(string Name)
        {
            BankUser[] users = SQL.GetInstance().Select<BankUser>($"lower(FirstName || ' ' || LastName) = lower(\"{Name}\");");
            
            return users;
        }
    }
}