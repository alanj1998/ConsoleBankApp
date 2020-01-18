using SSD.Lib;
using SSD.Models;
using System;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SSD.Controllers
{
    internal sealed class LoginController
    {
        private SQL _sql;
        private const int _saltSize = 16;
        private const int _keySize = 32;
        private const int _iterations = 100000;

        internal LoginController(SQL sqlLib)
        {
            this._sql = sqlLib;
        }
        internal Person Login(string email, SecureString password)
        {
            if (!Regex.IsMatch(email, @"\w{3,}@\w{1,}.(com|ie|co.uk|pl)"))
            {
                return null;
            }

            LoginDetails l = LoginDetails.Login(email);
            if (l == null)
            {
                password.Dispose();
                l = null;
                GC.Collect();
                return null; // no user
            }
            else if(!VerifyPasswordHash(password, l.Password, l.Salt))
            {
                l = null;
                GC.Collect();
                return null; // password did not match
            }
            else
            {
                password.Dispose();
                try
                {
                    Person p = null;
                    if (l.Role == Roles.Admin)
                    {
                        l = null;
                        BankAdmin temp = Person.SelectById<BankAdmin>(l.UserId);
                        if(temp.Id != l.UserId)
                        {
                            return null; // data integrity failed
                        }

                        p = temp;
                    }
                    else
                    {
                        BankUser temp = Person.SelectById<BankUser>(l.UserId);
                        if (temp.Id != l.UserId)
                        {
                            l = null;
                            GC.Collect();
                            return null; // data integrity failed
                        }
                        l = null;
                        GC.Collect();

                        p = temp;
                    }
                    LogEntry.SetActor($"{p.FirstName} {p.LastName}", p.Role);

                    return p;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        internal bool Register(string username, SecureString password, Person p)
        {
            string salt;
            string hash = CreatePasswordHash(password, out salt);

            LoginDetails l = new LoginDetails()
            {
                Email = username,
                Password = hash,
                Salt = salt,
                Role = p.Role,
                UserId = p.Id
            };

            try
            {
                LoginDetails.InsertNewObject(l);
                l = null;
                GC.Collect();
                return true;
            }
            catch (Exception e)
            {
                l = null;
                GC.Collect();
                Console.WriteLine(e.Message);
                return false;
            } 
        }

        private string CreatePasswordHash(SecureString password, out string salt)
        {
            using (Rfc2898DeriveBytes shaHash = 
                new Rfc2898DeriveBytes(
                Helpers.ConvertFromSecureToNormalString(password), 
                _saltSize,
                _iterations, 
                HashAlgorithmName.SHA512)) 
            {
                password.Dispose();
                string hash = Convert.ToBase64String(shaHash.GetBytes(_keySize));
                salt = Convert.ToBase64String(shaHash.Salt);
                return hash;
            }
        }

        private bool VerifyPasswordHash(SecureString password, string hash, string salt)
        {
            // Hash plaintext password using same salt and iterations as passed hash value
            using (Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(
              Helpers.ConvertFromSecureToNormalString(password),
              Convert.FromBase64String(salt),
              _iterations,
              HashAlgorithmName.SHA512))
            {
                password.Dispose();
                // Get the bytes of the new hash value
                byte[] keyToCheck = algorithm.GetBytes(_keySize);
                // Compare the two hash values and if identical return true.
                return keyToCheck.SequenceEqual(Convert.FromBase64String(hash));
            }
        }
    }
}