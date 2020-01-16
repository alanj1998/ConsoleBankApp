using System;
using System.IO;
using System.Security.Permissions;
using System.Reflection;
using System.Security;
using System.Security.Policy;

namespace SSD
{
    class Program 
    {
        internal static void Main(string[] args)
        {
            BankProgram.Start();
        }
    }
}