using SSD.Lib;

namespace SSD.Controllers
{
    public class AppController
    {
        private static AppController _controller;
        private static SQL _sql;

        public LoginController LoginController { get; set; }

        public AppController(SQL sqlLib)
        {
            _sql = sqlLib;
            this.LoginController = new LoginController(_sql);
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