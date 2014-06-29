using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace ProcessInsideControl
{
    public class ProcessInsideControlClass
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);


        /// <summary>
        /// Use this method if you'd like to start a process, then put its window inside a control.
        /// </summary>
        /// <param name="control">The control where the window should be put inside in.</param>
        /// <param name="programPath">The path to the program which is going to get executed and put inside the control.</param>
        /// <param name="rectangle">Add/subtract to the X/Y/width/height variables inside the variable in-case the window doesn't properly fit inside your control. (set to null if you don't want to add any value)</param>
        /// <returns>The process created.</returns>
        public static Process StartAndPutWindow(Control control, string programPath, Rectangle rectangle)
        {
            if(File.Exists(programPath))
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo() { UseShellExecute = true, FileName = programPath };
                Process p = Process.Start(processStartInfo);

                p.WaitForInputIdle();

                SetParent(p.MainWindowHandle, control.Handle);

                IntPtr processWHandle = p.MainWindowHandle;

                if(rectangle == null)
                {
                    MoveWindow(processWHandle, control.Bounds.X, control.Bounds.Y, control.Bounds.Width, control.Bounds.Height, true);
                }
                else
                {
                    MoveWindow(processWHandle, control.Bounds.X + rectangle.X, control.Bounds.Y + rectangle.Y, control.Bounds.Width + rectangle.Width, control.Bounds.Height + rectangle.Height, true);
                }

                return p;
            }
            else
            {
                throw new FileNotFoundException("Couldn't find the file required for the method StartAndPutWindow", programPath);
            }
        }

        /// <summary>
        /// Use this method if you'd like to distinguish the process from its container and kill it.
        /// </summary>
        /// <param name="process">The Process variable returned by the method "StartAndPutWindow".</param>
        /// <param name="kill">Set to true if you want to kill the process aswell. (default: true).</param>
        public static void RemoveAndStopProcess(Process process, bool kill)
        {
            SetParent(process.MainWindowHandle, IntPtr.Zero);
            if (kill)
                process.Kill();
        }
    }
}
