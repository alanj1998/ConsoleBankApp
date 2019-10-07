using SSD.Lib;

namespace SSD.Controllers
{
    public class AppController : IController
    {
        private static AppController _controller;
        private static SQL _sql;

        public LoginController LoginController { get; set; }
        public UserController UserController { get; set; }
        public TransactionController TransactionController { get; set; }

        public AppController(SQL sqlLib) : base(sqlLib)
        {
            this.LoginController = new LoginController(_sql);
            this.UserController = new UserController(_sql);
            this.TransactionController = new TransactionController(_sql);
        }
        public static AppController GetInstance()
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