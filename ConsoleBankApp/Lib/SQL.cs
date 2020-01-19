using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using System.Text.RegularExpressions;
using SSD.Models;
using System.Security;

namespace SSD.Lib
{
    internal class SQL
    {
        private MySqlConnection _connection;
        private static SQL sql;

        internal SQL()
        {
            this._connection = new MySqlConnection();
            this._connection.ConnectionString = new MySqlConnectionStringBuilder()
            {
                Server = "remotemysql.com",
                Database = "vJRBiWBOBC",
                Password = "zwXOcj1L1c",
                UserID = "vJRBiWBOBC"
            }.ToString();

            this._connection.Open();

            //var command = this._connection.CreateCommand();
//            command.CommandText = "SELECT quote($password);";
//            command.Parameters.AddWithValue("$password", @"&+hWa%sYNG!_AEXpPmA==G4pG=Z#FYJ53xg##nJx$#R-ad+h9-c9m+7&eS8L27j-7#sBhSLWACft+9Zwn=4BNngA!wGPJNb%+AV_6W@TE@=vg5a-n_fnvRsDu4BgQWD$^7azBWSNetmrau7Vpe%c#y#9XHtr7z?XP395qV$GTmSr$cxurVsvqt*5sP@#5?U&xUrt69pC65!z=qJ@?9b@vJvBA9yS^tbnuc6v_ejPu9JT-VV^Y6HtF7PS=PT$+XV^!@8ZZtN!ybAPtRG4RaD=_6V56W=VT^gvKX@dAtZ#3q3?x_ZcFP9vn65u^GMaFdc2#@JGGT=J4PHKVsgmeu5F=!RX%gy4bq+P@^JUFMGp6vfz3pt6TLTCvb!y$+!Grxe_5TZv&_#g2zuGhU4W^yyxy3N=js#Lu8yw%^uUTxbY4=t*=FGL^gQLx+*rc%X6mn*sKc9C?kkCpK+EYDF*pv8QeVv3Av9U@MB^B%V-7Us#HWTN@&7nY&qV7YbV-b7@=@%-"
//);
//            var quotedPassword = (string)command.ExecuteScalar();

            //command.CommandText = "PRAGMA KEY='&+hWa % sYNG!_AEXpPmA == G4pG = Z#FYJ53xg##nJx$#R-ad+h9-c9m+7&eS8L27j-7#sBhSLWACft+9Zwn=4BNngA!wGPJNb%+AV_6W@TE@=vg5a-n_fnvRsDu4BgQWD$^7azBWSNetmrau7Vpe%c#y#9XHtr7z?XP395qV$GTmSr$cxurVsvqt*5sP@#5?U&xUrt69pC65!z=qJ@?9b@vJvBA9yS^tbnuc6v_ejPu9JT-VV^Y6HtF7PS=PT$+XV^!@8ZZtN!ybAPtRG4RaD=_6V56W=VT^gvKX@dAtZ#3q3?x_ZcFP9vn65u^GMaFdc2#@JGGT=J4PHKVsgmeu5F=!RX%gy4bq+P@^JUFMGp6vfz3pt6TLTCvb!y$+!Grxe_5TZv&_#g2zuGhU4W^yyxy3N=js#Lu8yw%^uUTxbY4=t*=FGL^gQLx+*rc%X6mn*sKc9C?kkCpK+EYDF*pv8QeVv3Av9U@MB^B%V-7Us#HWTN@&7nY&qV7YbV-b7@=@%-'";
            ////command.Parameters.Clear();
            //var l = command.ExecuteNonQuery();

            sql = this;
        }

        ~SQL()
        {
            this._connection.Close();
        }

        internal static SQL GetInstance()
        {
            if (sql == null)
            {
                sql = new SQL();
            }
            return sql;
        }

        internal T[] Select<T>(string whereQuery)
        {
            List<T> returnValue = new List<T>();
            whereQuery = $"SELECT * FROM {this._GetTableStringFromModel(typeof(T).Name)} WHERE {whereQuery}";
            using (MySqlCommand command = new MySqlCommand(whereQuery, this._connection))
            {
                MySqlDataReader reader = command.ExecuteReader();
                //whereQuery = ""; // getting rid of the query as soon as it's called


                PropertyInfo[] p = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        T t = (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null );

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
                                object res = type.GetMethod("Select", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).MakeGenericMethod(prop.PropertyType).Invoke(this, new object[] { "Id = \"" + value + "\";" });

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

        internal T Insert<T>(T model)
        {
            string id = Guid.NewGuid().ToString();
            string sql = "";
            PropertyInfo idProp = typeof(T).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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

            using (MySqlCommand command = new MySqlCommand(sql, this._connection))
            {
                command.ExecuteNonQuery();
            }
            sql = ""; // setting sql to empty string to stop memory dump

            return model;
        }

        internal void Update<T>(T updatedModel, string whereClause)
        {
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo idProp = typeof(T).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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

            using (MySqlCommand command = new MySqlCommand(sql, this._connection))
            {
                command.ExecuteNonQuery();
            }
        }

        internal void Delete<T>(string whereClause)
        {
            string sql = $"DELETE FROM {this._GetTableStringFromModel(Activator.CreateInstance<T>().GetType().Name)} WHERE " + whereClause;
            using (MySqlCommand command = new MySqlCommand(sql, this._connection))
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