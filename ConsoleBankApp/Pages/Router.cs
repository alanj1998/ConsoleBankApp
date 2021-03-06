using System;
using SSD.Lib;
using SSD.Models;

namespace SSD.Pages
{
    internal sealed class Router
    {
        Splash splash;
        Login login;
        Register register;
        Error error;
        Dashboard dashboard;
        MakeTransaction makeTransaction;
        ViewTransactions viewTransactions;
        UpdateTransactions updateTransactions;
        DeleteTransaction deleteTransaction;
        ViewLogs viewLogs;

        internal Router()
        {
            splash = new Splash(this);
            login = new Login(this);
            register = new Register(this);
            error = new Error(this);
            dashboard = new Dashboard(this);
            makeTransaction = new MakeTransaction(this);
            viewTransactions = new ViewTransactions(this);
            updateTransactions = new UpdateTransactions(this);
            deleteTransaction = new DeleteTransaction(this);
            viewLogs = new ViewLogs(this);
        }

        internal void Navigate(Routes chosenRoute)
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
                case Routes.MakeTransaction:
                    throw new Exception("This route requires a model to be passed in...");
                case Routes.ViewTransaction:
                    throw new Exception("This route requires a model to be passed in...");
                case Routes.UpdateTransaction:
                    throw new Exception("This route requires a model to be passed in...");
                case Routes.DeleteTransaction:
                    throw new Exception("This route requires a model to be passed in...");
                case Routes.ViewLogs:
                    throw new Exception("This route requires a model to be passed in...");
                default:
                    route = error;
                    break;
            }
            Console.Clear();
            route.Render();
        }

        internal void Navigate(Routes chosenRoute, IModel model)
        {
            IPage route;

            switch (chosenRoute)
            {
                case Routes.Dashboard:
                    dashboard.AddModel(model as Person);
                    route = dashboard;
                    break;
                case Routes.MakeTransaction:
                    makeTransaction.AddModel(model as Person);
                    route = makeTransaction;
                    break;
                case Routes.ViewTransaction:
                    viewTransactions.AddModel(model as Person);
                    route = viewTransactions;
                    break;
                case Routes.UpdateTransaction:
                    updateTransactions.AddModel(model as Person);
                    route = updateTransactions;
                    break;
                case Routes.DeleteTransaction:
                    deleteTransaction.AddModel(model as Person);
                    route = deleteTransaction;
                    break;
                case Routes.ViewLogs:
                    viewLogs.AddModel(model as Person);
                    route = viewLogs;
                    break;
                default:
                    route = error;
                    break;
            }
            Helpers.FreeAndNil(ref model);

            Console.Clear();
            route.Render();
        }
    }

    internal enum Routes    {
        Splash, Login, Register, Error, Dashboard, MakeTransaction, ViewTransaction, UpdateTransaction, DeleteTransaction, ViewLogs
    }
}