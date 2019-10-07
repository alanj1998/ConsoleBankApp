using SSD.Lib;
using SSD.Models;

namespace SSD.Controllers
{
    public class TransactionController : IController
    {
        public TransactionController(SQL sqlLib) : base(sqlLib) { }

        public Transactions[] GetTransactionsForUser(string Id)
        {
            return Transactions.GetTransactionsForUser(Id);
        }

        public void UpdateTransaction(Transactions t)
        {
            Transactions.UpdateById(t, t.Id);
        }

        public void DeleteTransaction(string Id)
        {
            Transactions.DeleteById<Transactions>(Id);
        }
    }
}