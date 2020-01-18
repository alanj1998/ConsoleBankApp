using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using SSD.Lib;

namespace SSD.Controllers
{
    internal sealed class LoggerController: IController
    {
        private static byte[] iv;
        private static byte[] key;
        private static string logfilePath;
        private static List<SecureString> Log;

        internal LoggerController(SQL sql) : base(sql)
        {
            iv = Encoding.UTF8.GetBytes(
                "Z/ea5[ro{i[%hCM&AgJq+5bqU1UJ#)!)9V*KcOqz6K{6vqZ?UF0G0O^JdVavlWlixoE#M+v.,Mw0$bja0UtGa3Ul$5)k1]8YF7@I#efDPN~QT09anJ8@T7:zsLNwU-z8$ek>7:FKtCM*Gy39wOU6cD]m^V.^8PZUon#Z-$2Xn2$,EY98z-89&w6,`5doW#N,Q.!ayqFruN/_EAugIqw]Bjl+,G$-T*6Oop[]J-_Pr@moyR8No_xTc+AvyC40sFgQ");
            key = Encoding.UTF8.GetBytes(
                "lZi&&D~gcy`a.M@m.Dtvc<s^A?1nR[i}e7)`AsIj,C&rn,Qbl`>r*[x59,.93Om_Z*#{!qGi}%+){H^L6($zo`{Ckn-n.A#iCEE@b*bH,wihKhwsbcYDirZN1Sinj>-!rvE.`iSv]fd?_XTP^W^TFPW7G6UCWFH}u,bAi:rubNf<Y$H{l@X+$N.6T:@UrSsIyN%Rs(TYNo4nH`H,#N,bd:F+uFLy0V]P%NfOXQgvY].ShHY^yz2Dz}L?&<fOqm_f");
            logfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".consoleBankAppSSD");

            if (Log == null)
            {
                GetLogs();
            }
        }

        internal static void AddToLog(string logEntry)
        {
            SecureString s = new SecureString();
            foreach (char c in logEntry)
            {
                s.AppendChar(c);
            }
            
            Log.Add(s);
            AppendLog(logEntry);
        }

        internal static List<SecureString> Get5LinesOfLog(int offset = 0)
        {
            return Log.Skip(5 * offset).Take(5).ToList();
        }

        private static void AppendLog(string logEntry)
        {
            using (Rijndael algo = Rijndael.Create())
            {
                algo.Key = key;
                algo.Mode = CipherMode.CBC;
                algo.Padding = PaddingMode.PKCS7;
                    
                using (FileStream file = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] previousBytes = null;
                    int previousLength = 0;
                    long fileLength = file.Length;

                    if (fileLength != 0)
                    {
                        byte[] block = new byte[iv.Length];

                        if (fileLength >= iv.Length * 2)
                        {
                            // At least 2 blocks, take the second last block
                            file.Position = fileLength - iv.Length * 2;
                            file.Read(block, 0, block.Length);
                            algo.IV = block;
                        }
                        else
                        {
                            // A single block present, use the IV given
                            file.Position = fileLength - iv.Length;
                            algo.IV = iv;
                        }

                        // Read the last block
                        file.Read(block, 0, block.Length);
                        file.Position = fileLength - iv.Length;
                        
                        using (var ms = new MemoryStream(block))
                        using (ICryptoTransform decryptor = algo.CreateDecryptor())
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            previousBytes = new byte[iv.Length];
                            previousLength = cs.Read(previousBytes, 0, previousBytes.Length);
                        }
                    }
                    else
                    {
                        // Use the IV given
                        algo.IV = iv;
                    }
                    
                    using (ICryptoTransform encryptor = algo.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(file, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        // Rewrite the last block, if present. We even skip
                        // the case of block present but empty
                        if (previousLength != 0)
                        {
                            cs.Write(previousBytes, 0, previousLength);
                        }

                        // Write all data to the stream.
                        sw.Write(logEntry);
                    }
                }
            }
        }

        private static void GetLogs()
        {
            using (Rijndael algo = Rijndael.Create())
            {
                algo.Key = key;
                algo.IV = iv;
                algo.Mode = CipherMode.CBC;
                algo.Padding = PaddingMode.PKCS7;

                // Create the streams used for decryption.
                using (FileStream file = new FileStream(logfilePath, FileMode.Open, FileAccess.Read))
                {
                    using (ICryptoTransform decryptor = algo.CreateDecryptor())
                    {
                        using (CryptoStream cs = new CryptoStream(file, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] buffer = new byte[1048576];

                            try
                            {
                                int read;
                                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    SecureString s = new SecureString();
                                    foreach (char c in Encoding.UTF8.GetChars(buffer))
                                    {
                                        if (c == '\n')
                                        {
                                            Log.Add(s);
                                            s = new SecureString();
                                        }
                                        else
                                        {
                                            s.AppendChar(c);
                                        }
                                    }
                                }
                            }
                            catch (CryptographicException e)
                            {
                                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                            }

                            cs.Flush();
                            file.Flush();
                        }
                    }
                }
            }
        }
    }
}