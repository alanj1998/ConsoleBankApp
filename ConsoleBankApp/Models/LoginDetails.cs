using System;
using SSD.Lib;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using SSD.Controllers;

namespace SSD.Models
{
    internal sealed class LoginDetails : IModel
    {
        internal string Id { get; set; }
        internal string Email { get; set; }
        internal string Password { get; set; }
        internal string Salt { get; set; }
        internal string UserId { get; set; }
        internal Roles Role { get; set; }

        internal LoginDetails() { }

        internal static LoginDetails Login(string email)
        {
            LogEntry l = new LogEntry("Login", DateTime.Now);
            LoginDetails[] logins = SQL.GetInstance().Select<LoginDetails>($"Email = \"{email}\"");

            if (logins != null && logins.Length == 1)
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                
                return logins[0];
            }
            else
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                
                return null;
            }
        }
    }
}