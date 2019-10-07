using SSD.Lib;

namespace SSD.Models
{
    public class Transactions : IModel
    {
        public string Id { get; set; }
        public string SenderAccountId { get; set; }
        public BankUser SenderAccount { get; set; }
        public string ReceiverAccountId { get; set; }
        public BankUser ReceiverAccount { get; set; }
        public decimal Amount { get; set; }
        public string SmallCurrencyString { get; set; }
        public string LargeCurrencyString { get; set; }
        public long Timestamp { get; set; }

        public Transactions() { }

        public static Transactions[] GetTransactionsForUser(string Id)
        {
            Transactions[] t = SQL.GetInstance().Select<Transactions>($"SenderAccountId = \"{Id}\" or ReceiverAccountId = \"{Id}\";");

            return t;
        }
    }
}