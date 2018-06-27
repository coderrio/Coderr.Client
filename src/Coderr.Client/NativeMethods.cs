using System;
using System.Runtime.InteropServices;

namespace Coderr.Client
{
    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetCurrentProcess();
    }
}