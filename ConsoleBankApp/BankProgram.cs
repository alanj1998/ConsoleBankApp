using SSD.Controllers;
using SSD.Lib;
using SSD.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSD
{
    class BankProgram
    {
        internal static void Start()
        {
            Router _router = new Router();
            SQL sql = new SQL();
            new AppController(sql);
            new LoggerController(sql);

            _router.Navigate(Routes.Splash);
        }
    }
}
