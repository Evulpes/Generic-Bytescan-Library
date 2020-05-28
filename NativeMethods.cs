using System;
using System.Runtime.InteropServices;

namespace Generic_Bytescan_Library
{
    public class NativeMethods
    {
        public static class Handleapi
        {
            [DllImport("Kernel32")]
            public static extern bool CloseHandle(IntPtr hObject);
        }

        public static class Memoryapi
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        }
        public static class Processthreadsapi
        {
            [DllImport("Kernel32", SetLastError = true)]
            public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int processId);
        }
        public static class Psapi
        {
            [DllImport("psapi", SetLastError = true)]
            public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, IntPtr lpmodinfo, uint cb);

            public struct ModuleInfo
            {
                public IntPtr lpBaseOfDll;
                public uint SizeOfImage;
                public IntPtr EntryPoint;
            }
        }
    }
}
