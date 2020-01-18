using System;
using SSD.Controllers;
using SSD.Lib;

namespace SSD.Models
{
    internal sealed class BankUser : Person
    {
        internal string AccountType { get; set; }

        internal BankUser() { }

        internal static BankUser[] GetBankUsersByName(string Name)
        {
            LogEntry l = new LogEntry("GET Bank Users by Name: " + Name, DateTime.Now);
            BankUser[] users = SQL.GetInstance().Select<BankUser>($"lower(FirstName || ' ' || LastName) = lower(\"{Name}\");");
            
            l.AddEndTime(DateTime.Now);
            LoggerController.AddToLog(l.ToString());
            
            return users;
        }
    }
}