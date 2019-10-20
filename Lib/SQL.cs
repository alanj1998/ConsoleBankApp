using System.Data.SQLite;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using System.Text.RegularExpressions;
using SSD.Models;

namespace SSD.Lib
{
    public class SQL
    {
        private SQLiteConnection _connection;
        private static SQL sql;

        public SQL()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string dbName = @"../../../db/bank.db3";
            string connectionString = Path.Combine(basePath, dbName);

            this._connection = new SQLiteConnection(@"DataSource=" + connectionString + ";FailIfMissing=true;");
            this._connection.Open();

            sql = this;
        }

        ~SQL()
        {
            this._connection.Close();
        }

        public static SQL GetInstance()
        {
            if (sql == null)
            {
                sql = new SQL();
            }
            return sql;
        }

        public T[] Select<T>(string whereQuery)
        {
            List<T> returnValue = new List<T>();
            whereQuery = $"SELECT * FROM {this._GetTableStringFromModel(typeof(T).Name)} WHERE {whereQuery}";
            using (SQLiteCommand command = new SQLiteCommand(whereQuery, this._connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();

                PropertyInfo[] p = typeof(T).GetProperties();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        T t = (T)Activator.CreateInstance(typeof(T));

                        foreach (PropertyInfo prop in p)
                        {
                            if (prop.PropertyType.IsSubclassOf(typeof(IModel)))
                            {
                                string propName = prop.Name + "Id";
                                object value = reader[propName];

                                if (value == null || value.ToString() == "")
                                {
                                    continue;
                                }

                                Type type = this.GetType();

                                string sql = "SELECT * FROM " + this._GetTableStringFromModel(prop.PropertyType.Name) + " WHERE Id = \"" + value + "\";";
                                object res = type.GetMethod("Select").MakeGenericMethod(prop.PropertyType).Invoke(this, new object[] { "Id = \"" + value + "\";" });

                                if (prop.PropertyType.IsArray)
                                {
                                    prop.SetValue(t, res);
                                }
                                else
                                {
                                    if ((res as IModel[]) != null && (res as IModel[]).Length > 0)
                                    {
                                        prop.SetValue(t, (res as IModel[])[0]);
                                    }
                                }
                            }
                            else if (prop.PropertyType.IsEnum)
                            {
                                Type enumType = prop.PropertyType;
                                string enumNum = reader[prop.Name].ToString();
                                prop.SetValue(t, Enum.Parse(enumType, enumNum));
                            }
                            else
                            {
                                object value = null;
                                try
                                {
                                    value = reader[prop.Name];

                                }
                                catch (System.Exception)
                                {
                                    continue;
                                    throw;
                                }

                                if (value == null)
                                {
                                    continue;
                                }
                                else
                                {
                                    prop.SetValue(t, reader[prop.Name]);
                                }
                            }
                        }
                        if( t != null) returnValue.Add(t);
                    }
                }
            }
            return returnValue.ToArray();
        }

        public T Insert<T>(T model)
        {
            string id = Guid.NewGuid().ToString();
            string sql = "";
            PropertyInfo idProp = typeof(T).GetProperty("Id");
            PropertyInfo[] props = typeof(T).GetProperties();

            if (idProp == null)
            {
                throw new Exception("Id not in model");
            }
            idProp.SetValue(model, id);

            string upperCase = this._GetTableStringFromModel(model.GetType().Name);

            sql += $"INSERT INTO {upperCase} (";

            foreach (PropertyInfo p in props)
            {
                if (p.PropertyType.IsSubclassOf(typeof(IModel)))
                {
                    continue;
                }
                else
                {
                    sql += p.Name + ",";
                }
            }

            sql = sql.Remove(sql.Length - 1);
            sql += $") VALUES (";
            foreach (PropertyInfo p in props)
            {
                object val = p.GetValue(model);

                if (p.PropertyType.IsSubclassOf(typeof(IModel)))
                {
                    continue;
                }
                else if (p.PropertyType.IsEnum)
                {
                    Type t = p.PropertyType;
                    string number = p.GetValue(model).ToString();
                    int enumValue = (int)Enum.Parse(t, number.ToString());
                    sql += enumValue + ",";
                }
                else if (val.GetType().ToString().ToUpper() == "SYSTEM.STRING")
                {
                    sql += $"\"{val.ToString()}\",";
                }
                else
                {
                    sql += val.ToString() + ',';
                }
            }
            sql = sql.Remove(sql.Length - 1);
            sql += ");";
            using (SQLiteCommand command = new SQLiteCommand(sql, this._connection))
            {
                command.ExecuteNonQuery();
            }

            return model;
        }

        public void Update<T>(T updatedModel, string whereClause)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            PropertyInfo idProp = typeof(T).GetProperty("Id");

            if (idProp == null)
            {
                throw new Exception("No Id in object!");
            }

            string sql = $"UPDATE {this._GetTableStringFromModel(updatedModel.GetType().Name)} SET ";

            foreach (PropertyInfo p in props)
            {
                if (p.Name != "Id" && !p.PropertyType.IsSubclassOf(typeof(IModel)))
                {
                    object val = p.GetValue(updatedModel);

                    if (val.GetType().ToString().ToUpper() == "SYSTEM.STRING")
                    {
                        sql += $"\"{p.Name}\" = \"{p.GetValue(updatedModel)}\", ";
                    }
                    else
                    {
                        sql += $" \"{p.Name}\" = {p.GetValue(updatedModel)}, ";
                    }
                }
            }
            sql = sql.Remove(sql.Length - 2);
            sql += " WHERE " + whereClause;

            using (SQLiteCommand command = new SQLiteCommand(sql, this._connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete<T>(string whereClause)
        {
            string sql = $"DELETE FROM {this._GetTableStringFromModel(Activator.CreateInstance<T>().GetType().Name)} WHERE " + whereClause;
            using (SQLiteCommand command = new SQLiteCommand(sql, this._connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private string _GetTableStringFromModel(string modelName)
        {
            string[] splitType = Regex.Split(modelName, @"(?<!^)(?=[A-Z])"); // if it's not the first letter AND its a capital letter from A to Z
            string upperCase = string.Join('_', splitType).ToUpper();

            return upperCase;
        }
    }
}