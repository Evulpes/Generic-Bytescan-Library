using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace Generic_Bytescan_Library
{
    public class ByteScan : NativeMethods
    {
        public static int FindInBaseModule(Process process, byte[] pattern, out IntPtr[] offsets, bool wildcard=false, byte wildcardChar = 0xff, bool findAll = false)
        {
            List<IntPtr> offsetList = new List<IntPtr>();
            offsets = new IntPtr[] { (IntPtr)0 };

            //Create a handle to the process
            IntPtr processHandle = Processthreadsapi.OpenProcess(0x0010 | 0x0020 | 0x0008, false, process.Id);

            //Initialize the a modInfo struct as new so the size can be safely obtained
            var modInfo = new Psapi.ModuleInfo();

            //Allocated some memory for a ptr
            IntPtr modInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(modInfo));

            //Get the module info for the process base
            bool ntStatus = Psapi.GetModuleInformation(processHandle, process.MainModule.BaseAddress, modInfoPtr, (uint)Marshal.SizeOf(modInfo));
            if (!ntStatus)
                return Marshal.GetLastWin32Error();

            //Convert the ptr to the structure
            modInfo = (Psapi.ModuleInfo)Marshal.PtrToStructure(modInfoPtr, typeof(Psapi.ModuleInfo));

            //Allocate a new buffer based on the size of the image
            byte[] lpBuffer = new byte[modInfo.SizeOfImage];

            //Read the entire image into the buffer
            ntStatus = Memoryapi.ReadProcessMemory(processHandle, process.MainModule.BaseAddress, lpBuffer, (int)modInfo.SizeOfImage, out IntPtr _);
            if (!ntStatus)
                return Marshal.GetLastWin32Error();

            for (int i = 0; i < lpBuffer.Length; i++)
            {
                //Create a new array to copy potential matches to
                byte[] tempArray = new byte[pattern.Length];

                if (lpBuffer[i] == pattern[0])
                {
                    if ((lpBuffer.Length - lpBuffer[i]) < pattern.Length)
                        continue;

                    for (int x = 0; x < pattern.Length; x++)
                        tempArray[x] = lpBuffer[i + x];


                    if (wildcard)
                    {
                        bool match = true;
                        for (int x = 0; x < pattern.Length; x++)
                        {
                            if (pattern[x] == tempArray[x] || pattern[x] == wildcardChar)
                                continue;
                            else
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match)
                            offsetList.Add((IntPtr)i);
                    }

                    else
                    {
                        if (Enumerable.SequenceEqual(tempArray, pattern))
                        {
                            //Add the index of the byte[] to  the offset list
                            offsetList.Add((IntPtr)i);

                            if (!findAll)
                                break;
                        }
                    }


                }
            }
            offsets = offsetList.ToArray();
            Handleapi.CloseHandle(processHandle);
            Marshal.FreeHGlobal(modInfoPtr);
            return 0;
        }
    }
}
