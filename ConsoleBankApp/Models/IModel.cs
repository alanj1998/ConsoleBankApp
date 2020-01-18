using System;
using SSD.Controllers;
using SSD.Lib;

namespace SSD.Models
{
    internal abstract class IModel
    {
        internal IModel() { }
        internal static void DeleteById<T>(string Id)
        {
            string type = typeof(T).Name;
            LogEntry l = new LogEntry("DELETE " + type + " by Id: " + Id, DateTime.Now);
            try
            {
                SQL.GetInstance().Delete<T>("Id = \"" + Id + "\";");
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
            }
            catch (System.Exception e)
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                throw e;
            }
        }

        internal static T InsertNewObject<T>(T objectToInsert)
        {
            string type = typeof(T).Name;
            LogEntry l = new LogEntry("INSERT new " + type, DateTime.Now);
            try
            {
                T t = SQL.GetInstance().Insert<T>(objectToInsert);
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                return t;
            }
            catch (System.Exception)
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                throw;
            }
        }

        internal static T SelectById<T>(string Id)
        {
            string type = typeof(T).Name;
            LogEntry l = new LogEntry("SELECT " + type + " by ID: " + Id, DateTime.Now);

            try
            {
                T[] returnObj = SQL.GetInstance().Select<T>("Id = \"" + Id + "\";");

                if (returnObj.Length > 0)
                {          
                    l.AddEndTime(DateTime.Now);
                    LoggerController.AddToLog(l.ToString());
                    return returnObj[0];
                }
                else
                {
                    l.AddEndTime(DateTime.Now);
                    LoggerController.AddToLog(l.ToString());
                    return default(T);
                }
            }
            catch (System.Exception)
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                throw;
            }
        }

        internal static void UpdateById<T>(T updatedModel, string Id)
        {
            string type = typeof(T).Name;
            LogEntry l = new LogEntry("Update " + type + " by ID: " + Id, DateTime.Now);

            try
            {
                SQL.GetInstance().Update<T>(updatedModel, "Id = \"" + Id + "\";");
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
            }
            catch (System.Exception)
            {
                l.AddEndTime(DateTime.Now);
                LoggerController.AddToLog(l.ToString());
                throw;
            }
        }
    }
}