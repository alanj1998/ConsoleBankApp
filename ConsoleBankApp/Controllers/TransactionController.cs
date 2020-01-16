using SSD.Lib;
using SSD.Models;

namespace SSD.Controllers
{
    internal sealed class TransactionController : IController
    {
        internal TransactionController(SQL sqlLib) : base(sqlLib) { }

        internal Transactions[] GetTransactionsForUser(string Id)
        {
            return Transactions.GetTransactionsForUser(Id);
        }

        internal void UpdateTransaction(Transactions t)
        {
            Transactions.UpdateById(t, t.Id);
        }

        internal void DeleteTransaction(string Id)
        {
            Transactions.DeleteById<Transactions>(Id);
        }
    }
}