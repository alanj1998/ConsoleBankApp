using SSD.Lib;

namespace SSD.Controllers
{
    internal sealed class AppController : IController
    {
        private static AppController _controller;
        private static SQL _sql;

        internal LoginController LoginController { get; set; }
        internal UserController UserController { get; set; }
        internal TransactionController TransactionController { get; set; }

        internal AppController(SQL sqlLib) : base(sqlLib)
        {
            this.LoginController = new LoginController(_sql);
            this.UserController = new UserController(_sql);
            this.TransactionController = new TransactionController(_sql);
        }
        internal static AppController GetInstance()
        {
            if (_controller == null)
            {
                if (_sql == null)
                {
                    _sql = new SQL();
                }
                _controller = new AppController(_sql);
            }

            return _controller;
        }
    }
}