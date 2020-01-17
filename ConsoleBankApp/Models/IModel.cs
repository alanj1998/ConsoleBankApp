using SSD.Lib;

namespace SSD.Models
{
    internal abstract class IModel
    {
        internal IModel() { }
        internal static void DeleteById<T>(string Id)
        {
            try
            {
                SQL.GetInstance().Delete<T>("Id = \"" + Id + "\";");
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        internal static T InsertNewObject<T>(T objectToInsert)
        {
            try
            {
                T t = SQL.GetInstance().Insert<T>(objectToInsert);
                return t;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        internal static T SelectById<T>(string Id)
        {
            try
            {
                T[] returnObj = SQL.GetInstance().Select<T>("Id = \"" + Id + "\";");

                if (returnObj.Length > 0)
                {
                    return returnObj[0];
                }
                else return default(T);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        internal static void UpdateById<T>(T updatedModel, string Id)
        {
            try
            {
                SQL.GetInstance().Update<T>(updatedModel, "Id = \"" + Id + "\";");
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}