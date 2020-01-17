using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SSD.Lib
{
    internal static class Helpers
    {
        internal static string ConvertFromSecureToNormalString(this SecureString secureString)
        {
            if (secureString == null)
            {
                return null;
            }

            IntPtr pString = IntPtr.Zero;

            try
            {
                pString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(pString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(pString);
            }
        }

    }
}
