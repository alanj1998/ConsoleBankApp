using SSD.Lib;

namespace SSD.Models
{
    public class Person : IModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PhoneNumber { get; set; }
        public Roles Role { get; set; }

        public static Person GetPersonByName(string nameString)
        {
            string[] splitName = nameString.Split(' ');

            Person[] p = SQL.GetInstance().Select<Person>($"FirstName = \"{splitName[0]}\" AND LastName = \"{splitName[1]}\";");

            if (p.Length > 0)
            {
                return p[0];
            }
            else
            {
                return null;
            }
        }
    }

    public enum Roles
    {
        Admin = 1, User = 2
    }
}