using System;
using SSD.Lib;
using System.Collections.Generic;
using SSD.Controllers;

namespace SSD.Models
{
    internal sealed class Transactions : IModel
    {
        internal string Id { get; set; }
        internal string SenderAccountId { get; set; }
        internal BankUser SenderAccount { get; set; }
        internal string ReceiverAccountId { get; set; }
        internal BankUser ReceiverAccount { get; set; }
        internal decimal Amount { get; set; }
        internal string SmallCurrencyString { get; set; }
        internal string LargeCurrencyString { get; set; }
        internal long Timestamp { get; set; }

        internal Transactions() { }

        internal static Transactions[] GetTransactionsForUser(string Id)
        {
            LogEntry l = new LogEntry("GET Transactions For User: " + Id, DateTime.Now);   
            Transactions[] t = SQL.GetInstance().Select<Transactions>($"SenderAccountId = \"{Id}\" or ReceiverAccountId = \"{Id}\";");

            for(int i = 0; i < t.Length; i++)
            {
                if((t[i].SenderAccountId != Id) && (t[i].ReceiverAccountId != Id))
                {
                    List<Transactions> listOfTransactions = new List<Transactions>(t);
                    listOfTransactions.RemoveAt(i);
                    t = listOfTransactions.ToArray();
                }
            }
            
            l.AddEndTime(DateTime.Now);
            LoggerController.AddToLog(l.ToString());
            return t;
        }
    }
}