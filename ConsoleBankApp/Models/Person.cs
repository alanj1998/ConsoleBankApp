using System;
using SSD.Controllers;
using SSD.Lib;

namespace SSD.Models
{
    internal class Person : IModel
    {
        internal string Id { get; set; }
        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal string Address1 { get; set; }
        internal string Address2 { get; set; }
        internal string Address3 { get; set; }
        internal string PhoneNumber { get; set; }
        internal Roles Role { get; set; }

        internal static Person GetPersonByName(string nameString)
        {
            LogEntry l = new LogEntry("Getting " + nameString + " user by name", DateTime.Now);   
            string[] splitName = nameString.Split(' ');
            Person[] p = SQL.GetInstance().Select<Person>($"FirstName = \"{splitName[0]}\" AND LastName = \"{splitName[1]}\";");

            if (p.Length > 0)
            {
                if(p[0].FirstName == splitName[0] && p[0].LastName == splitName[1])
                {
                    l.AddEndTime(DateTime.Now);
                    LoggerController.AddToLog(l.ToString());
                    return p[0];
                } else
                {
                    l.AddEndTime(DateTime.Now);
                    LoggerController.AddToLog(l.ToString());
                    return null;
                }
            }
            else
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                return null;
            }
        }
    }

    internal enum Roles
    {
        Admin = 1, User = 2, Unauthenticated
    }
}