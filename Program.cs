using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GTABooster
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        static void Main()
        {
            Task blockInternet = new Task(() => {
                        Process process = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = "ipconfig",
                                Arguments = "/release",
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardError = true,
                                RedirectStandardOutput = true
                            }
                        };
                        process.Start();

                        Thread.Sleep(12000);

                        process.StartInfo.FileName = "ipconfig";
                        process.StartInfo.Arguments = "/renew";
                        process.Start();
                        process.WaitForExit();
                    });

            Task suspendProcess = new Task(() => {
                new Program().OptimiseGTA();
            });
            Task testReliability = new Task(() => {
                new Program().OptimiseGTA(500);
            });
            while (true)
            {
                if (GetAsyncKeyState(0x78) == -32767 && blockInternet.Status != TaskStatus.Running)
                    blockInternet.Start();
                if (GetAsyncKeyState(0x79) == -32767 && suspendProcess.Status != TaskStatus.Running)
                    suspendProcess.Start();
                if (GetAsyncKeyState(0x7A) == -32767 && suspendProcess.Status != TaskStatus.Running)
                    testReliability.Start();
                if (GetAsyncKeyState(0x7B) == -32767)
                {
                    while (true)
                    {
                        if (blockInternet.Status != TaskStatus.Running && suspendProcess.Status != TaskStatus.Running)
                            return;
                    }
                }
            }
        }

        public void OptimiseGTA(int millliseconds = 12000)
        {
            int pid = FindProcess();
            if (pid == -1) return;
            SuspendProcess(pid);
            Thread.Sleep(millliseconds);
            ResumeProcess(pid);
        }
        private int FindProcess()
        {
            Process currentProcess = Process.GetProcessesByName("GTA5").FirstOrDefault();
            if (currentProcess == null) return -1;
            int pid = currentProcess.Id;
            return pid;
        }
        [Flags]
        private enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);


        private static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid); // throws exception if process does not exist

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        private static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }
    }
}
