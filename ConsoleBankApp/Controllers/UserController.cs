using SSD.Lib;
using SSD.Models;

namespace SSD.Controllers
{
    internal sealed class UserController : IController
    {
        internal UserController(SQL sql) : base(sql)
        {
        }

        internal BankUser[] GetBankUsersByName(string Name)
        {
            BankUser[] users = BankUser.GetBankUsersByName(Name);
            
            return users;
        }
    }
}