// <copyright file="Desktops.cs" company="IGProgram">
//      Copyright © Ivica Gjorgjievski 2010 All rights reserved.
//      http://www.igprogram.webs.com
//      igprogram@hotmail.com
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using System.Diagnostics;
using System.Threading;

using System.Collections;
using System.IO;

namespace virtual_desktop
{
    /// <summary>
    /// Desktops management class.
    /// Win32 desktop functions: http://msdn.microsoft.com/en-us/library/ms687107(v=VS.85).aspx
    /// </summary>
    public class Desktops
    {
        [DllImport("user32.dll")]
        public static extern IntPtr CreateDesktop(
            string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, long dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll")]
        public static extern IntPtr OpenDesktop(
            string lpszDesktop, int dwFlags, bool fInherit, long dwDesiredAccess);

        [DllImport("user32.dll")]
        public static extern IntPtr OpenInputDesktop(
            int dwFlags, bool fInherit, long dwDesiredAccess);

        [DllImport("user32.dll")]
        public static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool GetUserObjectInformation(
            IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, ref int lpnLengthNeeded);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            ref PROCESS_INFORMATION lpProcessInformation
            );

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        // Process priority
        private const int NORMAL_PRIORITY_CLASS = 0x00000020;

        // Desktop access rights
        private const long DESKTOP_CREATEWINDOW = 0x0002L;
        private const long DESKTOP_ENUMERATE = 0x0040L;
        private const long DESKTOP_WRITEOBJECTS = 0x0080L;
        private const long DESKTOP_SWITCHDESKTOP = 0x0100L;
        private const long DESKTOP_CREATEMENU = 0x0004L;
        private const long DESKTOP_HOOKCONTROL = 0x0008L;
        private const long DESKTOP_READOBJECTS = 0x0001L;
        private const long DESKTOP_JOURNALRECORD = 0x0010L;
        private const long DESKTOP_JOURNALPLAYBACK = 0x0020L;

        // Desktop access rights
        private const long rights =
            DESKTOP_CREATEWINDOW |
            DESKTOP_ENUMERATE |
            DESKTOP_WRITEOBJECTS |
            DESKTOP_SWITCHDESKTOP |
            DESKTOP_CREATEMENU |
            DESKTOP_HOOKCONTROL |
            DESKTOP_READOBJECTS |
            DESKTOP_JOURNALRECORD |
            DESKTOP_JOURNALPLAYBACK;




        /// <summary>
        /// Create new desktop, if desktop exists return handle of existent
        /// </summary>
        /// <param name="name">Desktop name</param>
        /// <returns>Desktop handle</returns>
        public static IntPtr DesktopCreate(string name)
        {
            return CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, rights, IntPtr.Zero);

        }// CreateDesktop


        /// <summary>
        /// Open existing desktop
        /// </summary>
        /// <param name="name">Desktop name</param>
        /// <returns>Desktop handle</returns>
        public static IntPtr DesktopOpen(string name)
        {
            return OpenDesktop(name, 0, true, rights);

        }// OpenDesktop


        /// <summary>
        /// Open input (actual) desktop
        /// </summary>
        /// <returns>Desktop handle</returns>
        public static IntPtr DesktopOpenInput()
        {
            return OpenInputDesktop(0, true, rights);

        }//


        /// <summary>
        /// Switch to another desktop
        /// </summary>
        /// <param name="name">Desktop name</param>
        /// <returns>True if sucessful, otherwise false.</returns>
        public static bool DesktopSwitch(string name)
        {
            IntPtr handle = DesktopOpen(name);
            if (handle == IntPtr.Zero) { return false; }
            return SwitchDesktop(handle);

        }//

        public static IntPtr get_DesktopHandle(string name)
        {
            IntPtr handle = DesktopOpen(name);
            if (handle == IntPtr.Zero) { return IntPtr.Zero; }
            return handle;
        }


        /// <summary>
        /// Close an existing desktop
        /// </summary>
        /// <param name="name">Desktop name</param>
        /// <returns>True if sucessful, otherwise false.</returns>
        public static bool DesktopClose(string name)
        {
            Console.WriteLine("DesktopClose call!");
            Process[] processes = Desktops.GetInputProcesses(name);
            Process thisProc = Process.GetCurrentProcess();
            foreach (Process p in processes)
            {
                if (p.ProcessName != thisProc.ProcessName)
                {
                    p.Kill();
                }
            }

            IntPtr handle = DesktopOpen(name);
            
            Console.WriteLine("closeHandle:"+handle);
            
            if (handle == IntPtr.Zero) { return false; }
            else
            {
                Console.WriteLine("call closeDesktop");
                return CloseDesktop(handle);
            }
            
        }//


        /// <summary>
        /// Check if desktop exists 
        /// </summary>
        /// <param name="name">Desktop name</param>
        /// <returns>True if sucessful, otherwise false.</returns>
        public static bool DesktopExists(string name)
        {
            IntPtr handle = DesktopOpen(name);
            if (handle == IntPtr.Zero) { return false; }
            return true;
        }


        /// <summary>
        /// Get desktop name from desktop handle.
        /// </summary>
        /// <param name="handle">Desktop handle</param>
        /// <returns>Desktop name</returns>
        public static string DesktopName(IntPtr handle)
        {
            // If null return
            if (handle == IntPtr.Zero) { return string.Empty; }

            // Get length
            int length = 0;
            GetUserObjectInformation(handle, 2, IntPtr.Zero, 0, ref length);

            // Get name
            IntPtr pointer = Marshal.AllocHGlobal(length);
            bool result = GetUserObjectInformation(handle, 2, pointer, length, ref length);
            string name = Marshal.PtrToStringAnsi(pointer);
            Marshal.FreeHGlobal(pointer);

            // If error 
            if (!result) { return string.Empty; }

            return name;
        }//



        /// <summary>
        /// Create process on desktop.
        /// </summary>
        /// <param name="name">Desktop name</param>
        /// <param name="path">Executable name or full path</param>
        /// <param name="args">Executable arguments separated by space and slash, example: /arg1 /arg2</param>
        /// <returns>True if sucessful, otherwise false.</returns>
        public static bool ProcessCreate(string name, string path, string args)
        {
            try
            {
                // Process parameters.
                PROCESS_INFORMATION processInformation = new PROCESS_INFORMATION();
                STARTUPINFO startupInfo = new STARTUPINFO();
                startupInfo.cb = Marshal.SizeOf(startupInfo);
                startupInfo.lpDesktop = name;      // Desktop name

                // Create and start the process.
                string path2 = string.Format("\"{0}\" {1}", path, args); // Format arguments
                bool result = CreateProcess(path, path2, IntPtr.Zero, IntPtr.Zero, true,
                        NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref startupInfo, ref processInformation);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }// 


        public static Process[] GetInputProcesses(string currentDesktop)
        {
            Console.WriteLine("GetInputProcesses call!");
            // get all processes.
            Process[] processes = Process.GetProcesses();

            ArrayList m_procs = new ArrayList();

            // get the current desktop name.
            // = GetDesktopName(Desktop.Input.DesktopHandle);

            // cycle through the processes.
            int index = 0;

            TextWriter oldOut = Console.Out;
            FileStream ostrm;
            StreamWriter writer;

            try
            {
                ostrm = new FileStream("./output.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return null;
            }
            Console.SetOut(writer);

            foreach (Process process in processes)
            {
                ProcessThreadCollection threadlist = process.Threads;
                int processID = 0;
                int threadID = GetWindowThreadProcessId(process.MainWindowHandle, out processID);
                  
                Console.WriteLine("DesktopName(GetThreadDesktop(pt.Id))::" + GetThreadDesktop(threadID));
                /*
                GetThreadDesktop(threadID)
                foreach (ProcessThread theThread in threadlist)
                {
                    //Console.WriteLine("Thread ID:{0}",theThread.Id);
                    Console.WriteLine("DesktopName(GetThreadDesktop(pt.Id))::" + GetThreadDesktop(theThread.Id));
                }
                 */
            }
            
            
            /*
            foreach (Process process in processes)
            {
                // check the threads of the process - are they in this one?
                foreach (ProcessThread pt in process.Threads)
                {
                    //Console.WriteLine("process value {0} is" + process, index++);
                    // check for a desktop name match.
                   
                    
                    Console.WriteLine("DesktopName(GetThreadDesktop(pt.Id))::" + DesktopName(GetThreadDesktop(pt.Id)));
                    
                    if (DesktopName(GetThreadDesktop(pt.Id)) == currentDesktop)
                    {
                        Console.WriteLine("matched value is" + process);
                        // found a match, add to list, and bail.
                        m_procs.Add(process);
                        break;
                    }
                }
            }
            */
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();

            // put ArrayList into array.
            Process[] procs = new Process[m_procs.Count];

            for (int i = 0; i < procs.Length; i++) procs[i] = (Process)m_procs[i];

            return procs;
        }



    }// class

}// namespace
