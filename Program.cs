using LuckyWeel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GTABooster
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(
    int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        static void Main()
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }
        private static bool IsInternetLocked = false;
        private static IntPtr HookCallback(
    int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
            Keys k = (Keys)Marshal.ReadInt32(lParam);
            if  ((k >= Keys.F7 && k <= Keys.F12 || k==Keys.L || k == Keys.N) && wParam == (IntPtr)WM_KEYDOWN)
            {
                switch (k)
                {
                    case Keys.F7:
                        spinWheel();
                        break;
                    case Keys.F10:
                        OptimiseGTA();
                        break;
                    case Keys.L:
                        callLester();
                        break;
                    case Keys.N:
                        callMechanic();
                        break;
                    case Keys.F8:
                        resetCayo();
                        break;
                    case Keys.F11:
                        OptimiseGTA(500);
                        break;
                    case Keys.F12:
                        { if (IsInternetLocked) UnlockInternet(); Application.Exit(); }
                        break;

                }
            }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static void resetCayo()
        {
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
            IsInternetLocked = true;
            process.Start();
            foreach (var pro in Process.GetProcessesByName("GTA5"))
            {
                pro.Kill();
            }
            process.WaitForExit();
            Thread.Sleep(5000);
            UnlockInternet();
            IsInternetLocked = false;
        }
        private static void UnlockInternet()
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "ipconfig",
                    Arguments = "/renew",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
        private static void spinWheel()
        {
            //Thread.Sleep(4000); form continue to s

            //Thread.Sleep(5000); when s appears
            Thread.Sleep(4700);
            KeyboardSimulator ks = new KeyboardSimulator();
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_S, timePressMs: 90);
        }
        private static void callMechanic()
        {
            KeyboardSimulator ks = new KeyboardSimulator();
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.BACK, timePressMs: 90);
            Thread.Sleep(500);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.BACK, timePressMs: 90);
            Thread.Sleep(500);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.BACK, timePressMs: 90);
            Thread.Sleep(500);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_I, timePressMs: 90);
            Thread.Sleep(50);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_K, timePressMs: 90);
            Thread.Sleep(50);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.RETURN, timePressMs: 90);
            for (int i = 0; i < 14; i++)
            {
                Thread.Sleep(50);
                ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_I, timePressMs: 90);
            }
            Thread.Sleep(50);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.RETURN, timePressMs: 90);
        }
        private static void callLester()
        {
            KeyboardSimulator ks = new KeyboardSimulator();
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.BACK, timePressMs: 90);
            Thread.Sleep(500);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.BACK, timePressMs: 90);
            Thread.Sleep(500);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.BACK, timePressMs: 90);
            Thread.Sleep(500);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_I, timePressMs: 90);
            Thread.Sleep(50);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_K, timePressMs: 90);
            Thread.Sleep(50);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.RETURN, timePressMs: 90);
            for (int i = 0; i < 14; i++)
            {
                Thread.Sleep(50);
                ks.SendAndWait(KeyboardSimulator.ScanCodeShort.KEY_I, timePressMs: 90);
            }
            Thread.Sleep(50);
            ks.SendAndWait(KeyboardSimulator.ScanCodeShort.RETURN, timePressMs: 90);
        }
        public static void OptimiseGTA(int millliseconds = 12000)
        {
            int pid = FindProcess();
            if (pid == -1) return;
            SuspendProcess(pid);
            Thread.Sleep(millliseconds);
            ResumeProcess(pid);
        }
        private static int FindProcess()
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
