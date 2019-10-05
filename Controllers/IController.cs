using SSD.Lib;

namespace SSD.Controllers
{
    public class IController
    {
        private SQL _sql;

        public IController(SQL sql)
        {
            _sql = sql;
        }
    }
}