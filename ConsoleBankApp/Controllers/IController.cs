using SSD.Lib;

namespace SSD.Controllers
{
    internal class IController
    {
        private SQL _sql;

        internal IController(SQL sql)
        {
            _sql = sql;
        }
    }
}