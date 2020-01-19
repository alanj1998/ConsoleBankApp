using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using SSD.Lib;
using SSD.Models;

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
                "Z/ea5[ro{i[%hCM&");
            key = Encoding.UTF8.GetBytes(
                "lZi&&D~gcy`a.M@m.Dtvc<s^A?1nR[i}");
            logfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".consoleBankAppSSD");

            if (Log == null)
            {
                Log = new List<SecureString>();
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
            
            Log.Insert(0, s);
            AppendLog(logEntry);
        }

        internal static int GetCountOfLogs()
        {
            return Log.Count;
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

                        // Write log entry to stream backwards so newest is on top
                        sw.Write(logEntry);

                        // Rewrite the last block, if present. We even skip
                        // the case of block present but empty
                        if (previousLength != 0)
                        {
                            cs.Write(previousBytes, 0, previousLength);
                        }
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

                try
                {
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
                                            else if(c == '\0')
                                            {
                                                continue;
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
                catch(IOException)
                {
                    Log = new List<SecureString>();
                }
            }
        }
    }
}