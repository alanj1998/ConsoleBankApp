using SSD.Lib;

namespace SSD.Models
{
    public class LoginDetails : IModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public Roles Role { get; set; }

        public static LoginDetails Login(string email, string password)
        {
            LoginDetails[] logins = SQL.GetInstance().Select<LoginDetails>($"Email = \"{email}\" AND Password = \"{password}\"");

            if (logins != null && logins.Length > 0)
            {
                return logins[0];
            }
            else
            {
                return null;
            }
        }
    }
}