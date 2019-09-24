using System;
using SSD.Models;

namespace SSD.Pages
{
    public class Router
    {
        Splash splash;
        Login login;
        Register register;
        Error error;
        Dashboard dashboard;

        public Router()
        {
            splash = new Splash(this);
            login = new Login(this);
            register = new Register(this);
            error = new Error(this);
            dashboard = new Dashboard(this);
        }

        public void Navigate(Routes chosenRoute)
        {
            IPage route;

            switch (chosenRoute)
            {
                case Routes.Splash:
                    route = splash;
                    break;
                case Routes.Login:
                    route = login;
                    break;
                case Routes.Register:
                    route = register;
                    break;
                case Routes.Dashboard:
                    throw new Exception("This route requires a model to be passed in...");
                default:
                    route = error;
                    break;
            }
            Console.Clear();
            route.Render();
        }

        public void Navigate(Routes chosenRoute, IModel model)
        {
            IPage route;

            switch (chosenRoute)
            {
                case Routes.Dashboard:
                    dashboard.AddModel(model as Person);
                    route = dashboard;
                    break;
                default:
                    route = error;
                    break;
            }
            Console.Clear();
            route.Render();
        }
    }

    public enum Routes
    {
        Splash, Login, Register, Error, Dashboard
    }
}