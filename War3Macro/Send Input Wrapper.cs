using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace War3Macro
{
    public static class SendInputWrapper
    {

        public static void SendString(string inp, bool toAll)
        {
            IEnumerable<INPUT> inputs = new INPUT[0];
            if (toAll)
                inputs = inputs.Concat(new INPUT[2] { LShiftUpInput, LShiftDownInput });
            inputs = inputs.Concat(new INPUT[2] { EnterDownInput, EnterUpInput });
            if (toAll)
                inputs = inputs.Concat(new INPUT[1] { LShiftUpInput });
            inputs = inputs.Concat(BuildInputsForString(inp));
            inputs = inputs.Concat(new INPUT[2] { EnterDownInput, EnterUpInput });
            var finalinputs = inputs.ToArray();

            var sent = SendInput((uint)finalinputs.Length, finalinputs, Marshal.SizeOf(finalinputs[0]));
            var error = "0x" + ((uint)Marshal.GetLastWin32Error() | (uint)0x80000000).ToString("X");

            //var exception = Marshal.GetExceptionForHR((int)((uint)Marshal.GetLastWin32Error() | (uint)0x80000000));
            var exception = Marshal.GetExceptionForHR(Marshal.GetLastWin32Error());
            if (sent != finalinputs.Length)
            {
                var noop = "";
            }
        }

        private static IEnumerable<INPUT> BuildInputsForString(string inp)
        {
            if (string.IsNullOrEmpty(inp))
                yield break;
            foreach (char chr in inp)
            {
                yield return CharDownInput(chr);
                yield return CharUpInput(chr);
            }
        }
        private static INPUT LShiftDownInput
        {
            get
            {
                var ret = new INPUT();
                ret.type = INPUT_KEYBOARD;
                ret.mkhi.ki = new KEYBDINPUT();
                ret.mkhi.ki.wVk = 0x10;
                ret.mkhi.ki.dwFlags = 0;
                return ret;
            }
        }
        private static INPUT LShiftUpInput
        {
            get
            {
                var ret = new INPUT();
                ret.type = INPUT_KEYBOARD;
                ret.mkhi.ki = new KEYBDINPUT();
                ret.mkhi.ki.wVk = 0x10;
                ret.mkhi.ki.dwFlags = KEYEVENTF_KEYUP;
                return ret;
            }
        }
        private static INPUT CharDownInput(char chr)
        {
            var ret = new INPUT();
            ret.type = INPUT_KEYBOARD;
            ret.mkhi.ki = new KEYBDINPUT();
            ret.mkhi.ki.wVk = 0;
            ret.mkhi.ki.wScan = chr;
            ret.mkhi.ki.dwFlags = KEYEVENTF_UNICODE;
            return ret;
        }
        private static INPUT CharUpInput(char chr)
        {
            var ret = new INPUT();
            ret.type = INPUT_KEYBOARD;
            ret.mkhi.ki = new KEYBDINPUT();
            ret.mkhi.ki.wVk = 0;
            ret.mkhi.ki.wScan = chr;
            ret.mkhi.ki.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_UNICODE;
            return ret;
        }
        private static INPUT EnterDownInput
        {
            get
            {
                var ret = new INPUT();
                ret.type = INPUT_KEYBOARD; 
                ret.mkhi.ki = new KEYBDINPUT();
                ret.mkhi.ki.wVk = 0x0D;
                ret.mkhi.ki.dwFlags = 0;
                return ret;
            }
        }
        private static INPUT EnterUpInput
        {
            get
            {
                var ret = new INPUT();
                ret.type = INPUT_KEYBOARD;
                ret.mkhi.ki = new KEYBDINPUT();
                ret.mkhi.ki.wVk = 0x0D;
                ret.mkhi.ki.dwFlags = KEYEVENTF_KEYUP;
                return ret;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential, Size = 28)]
        struct INPUT
        {
            public int type;
            public MOUSEKEYBDHARDWAREINPUT mkhi;
        }

        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_SCANCODE = 0x0008;
    }
}
