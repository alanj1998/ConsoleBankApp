using SSD.Lib;
using SSD.Models;

namespace SSD.Controllers
{
    public class LoginController
    {
        private SQL _sql;

        public LoginController(SQL sqlLib)
        {
            this._sql = sqlLib;
        }
        public Person Login(string email, string password)
        {
            LoginDetails l = LoginDetails.Login(email, password);

            if (l == null)
            {
                return null;
            }
            else
            {
                try
                {
                    Person p = null;
                    if (l.Role == Roles.Admin)
                    {
                        BankAdmin temp = Person.SelectById<BankAdmin>(l.UserId);
                        p = temp;
                    }
                    else
                    {
                        BankUser temp = Person.SelectById<BankUser>(l.UserId);
                        p = temp;
                    }

                    return p;
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }

        public bool Register(string username, string password, Person p)
        {
            return false;
        }
    }
}