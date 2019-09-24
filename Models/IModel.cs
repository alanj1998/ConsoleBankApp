using SSD.Lib;

namespace SSD.Models
{
    public abstract class IModel
    {
        public IModel() { }
        public static void DeleteById<T>(string Id)
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

        public static T InsertNewObject<T>(T objectToInsert)
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

        public static T SelectById<T>(string Id)
        {
            try
            {
                Debug.Info("IModel.cs", 37, "Pre in model");
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

        public static void UpdateById<T>(T updatedModel, string Id)
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