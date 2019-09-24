using System;
using SSD.Pages;
using SSD.Lib;
using SSD.Controllers;

namespace SSD
{
    class Program
    {
        static void Main(string[] args)
        {
            Router _router = new Router();
            SQL sql = new SQL();
            AppController app = new AppController(sql);

            _router.Navigate(Routes.Splash);
        }
    }
}