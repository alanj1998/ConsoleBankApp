using SSD.Lib;
using SSD.Models;

namespace SSD.Controllers
{
    public class UserController : IController
    {
        public UserController(SQL sql) : base(sql)
        {
        }

        public BankUser[] GetBankUsersByName(string Name)
        {
            BankUser[] users = BankUser.GetBankUsersByName(Name);

            return users;
        }
    }
}