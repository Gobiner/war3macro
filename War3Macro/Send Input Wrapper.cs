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
				inputs = inputs.Concat(new INPUT[1] { LShiftDownInput() });
			inputs = inputs.Concat(new INPUT[2] { EnterDownInput(), EnterUpInput() });
			if (toAll)
				inputs = inputs.Concat(new INPUT[1] { LShiftUpInput() });
			inputs = inputs.Concat(BuildInputsForString(inp));
			inputs = inputs.Concat(new INPUT[2] { EnterDownInput(), EnterUpInput() });
			var finalinputs = inputs.ToArray();
			SendInput((uint)finalinputs.Length, finalinputs, Marshal.SizeOf(finalinputs[0]));
		}

		private static IEnumerable<INPUT> BuildInputsForString(string inp)
		{
			if (string.IsNullOrEmpty(inp))
				yield break;
			foreach (char chr in inp)
			{
				if (char.IsUpper(chr))
				{
					yield return LShiftDownInput();
				}
				yield return CharDownInput(chr);
				yield return CharUpInput(chr);
				if (char.IsUpper(chr))
				{
					yield return LShiftUpInput();
				}
			}
		}
		private static INPUT LShiftDownInput()
		{
			var ret = new INPUT();
			ret.type = INPUT_TYPE.KEYBOARD;
			ret.ki = new KEYBOARD_INPUT();
			ret.ki.wVk = (uint)VK.LSHIFT;
			return ret;
		}
		private static INPUT LShiftUpInput()
		{
			var ret = new INPUT();
			ret.type = INPUT_TYPE.KEYBOARD;
			ret.ki = new KEYBOARD_INPUT();
			ret.ki.wVk = (uint)VK.LSHIFT;
			ret.ki.dwFlags = KEYEVENTF.KEYUP;
			return ret;
		}
		private static INPUT CharDownInput(char chr)
		{
			byte vk = VkKeyScan(chr);
			var ret = new INPUT();
			ret.type = INPUT_TYPE.KEYBOARD;
			ret.ki = new KEYBOARD_INPUT();
			ret.ki.wVk = vk;
			return ret;
		}
		private static INPUT CharUpInput(char chr)
		{
			byte vk = VkKeyScan(chr);
			var ret = new INPUT();
			ret.type = INPUT_TYPE.KEYBOARD;
			ret.ki = new KEYBOARD_INPUT();
			ret.ki.wVk = vk;
			ret.ki.dwFlags = KEYEVENTF.KEYUP;
			return ret;
		}
		private static INPUT EnterDownInput()
		{
			var ret = new INPUT();
			ret.type = INPUT_TYPE.KEYBOARD;
			ret.ki = new KEYBOARD_INPUT();
			ret.ki.wVk = (uint)VK.RETURN;
			return ret;
		}
		private static INPUT EnterUpInput()
		{
			var ret = new INPUT();
			ret.type = INPUT_TYPE.KEYBOARD;
			ret.ki = new KEYBOARD_INPUT();
			ret.ki.wVk = (uint)VK.RETURN;
			ret.ki.dwFlags = KEYEVENTF.KEYUP;
			return ret;
		}


		[DllImport("user32.dll")]
		public static extern byte VkKeyScan(char ch);
		[DllImport("user32.dll")]
		public static extern uint MapVirtualKey(uint uCode, uint uMapType);
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs,[MarshalAs(UnmanagedType.LPArray)] INPUT[] pInputs, int cbSize);
		[StructLayout(LayoutKind.Sequential)]
		private struct KEYBOARD_INPUT
		{
			public uint wVk;
			public ushort wScan;
			public KEYEVENTF dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}
		[StructLayout(LayoutKind.Explicit, Size = 28)]
		private struct INPUT
		{
			[FieldOffset(0)]
			public INPUT_TYPE type;
			[FieldOffset(4)]
			public KEYBOARD_INPUT ki;
		}
		private enum INPUT_TYPE : int
		{
			MOUSE = 0,
			KEYBOARD = 1,
			HARDWARE = 2
		}
		private enum KEYEVENTF : uint
		{
			EXTENDEDKEY = 0x0001,
			KEYUP = 0x0002,
			UNICODE = 0x0004,
			SCANCODE = 0x0008
		}
		private enum VK : ushort
		{
			//
			// Virtual Keys, Standard Set
			//
			LBUTTON = 0x01,
			RBUTTON = 0x02,
			CANCEL = 0x03,
			MBUTTON = 0x04,    // NOT contiguous with L & RBUTTON

			XBUTTON1 = 0x05,    // NOT contiguous with L & RBUTTON
			XBUTTON2 = 0x06,    // NOT contiguous with L & RBUTTON

			// 0x07 : unassigned

			BACK = 0x08,
			TAB = 0x09,

			// 0x0A - 0x0B : reserved

			CLEAR = 0x0C,
			RETURN = 0x0D,

			SHIFT = 0x10,
			CONTROL = 0x11,
			MENU = 0x12,
			PAUSE = 0x13,
			CAPITAL = 0x14,

			KANA = 0x15,
			HANGEUL = 0x15,  // old name - should be here for compatibility
			HANGUL = 0x15,
			JUNJA = 0x17,
			FINAL = 0x18,
			HANJA = 0x19,
			KANJI = 0x19,

			ESCAPE = 0x1B,

			CONVERT = 0x1C,
			NONCONVERT = 0x1D,
			ACCEPT = 0x1E,
			MODECHANGE = 0x1F,

			SPACE = 0x20,
			PRIOR = 0x21,
			NEXT = 0x22,
			END = 0x23,
			HOME = 0x24,
			LEFT = 0x25,
			UP = 0x26,
			RIGHT = 0x27,
			DOWN = 0x28,
			SELECT = 0x29,
			PRINT = 0x2A,
			EXECUTE = 0x2B,
			SNAPSHOT = 0x2C,
			INSERT = 0x2D,
			DELETE = 0x2E,
			HELP = 0x2F,

			ZERO = 0x30,
			ONE = 0x31,
			TWO = 0x32,
			THREE = 0x33,
			FOUR = 0x34,
			FIVE = 0x35,
			SIX = 0x36,
			SEVEN = 0x37,
			EIGHT = 0x38,
			NINE = 0x39,

			// 0x40 : unassigned

			A = 0x41,
			B = 0x42,
			C = 0x43,
			D = 0x44,
			E = 0x45,
			F = 0x46,
			G = 0x47,
			H = 0x48,
			I = 0x49,
			J = 0x4A,
			K = 0x4B,
			L = 0x4C,
			M = 0x4D,
			N = 0x4E,
			O = 0x4F,
			P = 0x50,
			Q = 0x51,
			R = 0x52,
			S = 0x53,
			T = 0x54,
			U = 0x55,
			V = 0x56,
			W = 0x57,
			X = 0x58,
			Y = 0x59,
			Z = 0x5A,

			LWIN = 0x5B,
			RWIN = 0x5C,
			APPS = 0x5D,

			//
			// 0x5E : reserved
			//

			SLEEP = 0x5F,

			NUMPAD0 = 0x60,
			NUMPAD1 = 0x61,
			NUMPAD2 = 0x62,
			NUMPAD3 = 0x63,
			NUMPAD4 = 0x64,
			NUMPAD5 = 0x65,
			NUMPAD6 = 0x66,
			NUMPAD7 = 0x67,
			NUMPAD8 = 0x68,
			NUMPAD9 = 0x69,
			MULTIPLY = 0x6A,
			ADD = 0x6B,
			SEPARATOR = 0x6C,
			SUBTRACT = 0x6D,
			DECIMAL = 0x6E,
			DIVIDE = 0x6F,
			F1 = 0x70,
			F2 = 0x71,
			F3 = 0x72,
			F4 = 0x73,
			F5 = 0x74,
			F6 = 0x75,
			F7 = 0x76,
			F8 = 0x77,
			F9 = 0x78,
			F10 = 0x79,
			F11 = 0x7A,
			F12 = 0x7B,
			F13 = 0x7C,
			F14 = 0x7D,
			F15 = 0x7E,
			F16 = 0x7F,
			F17 = 0x80,
			F18 = 0x81,
			F19 = 0x82,
			F20 = 0x83,
			F21 = 0x84,
			F22 = 0x85,
			F23 = 0x86,
			F24 = 0x87,

			//
			// 0x88 - 0x8F : unassigned
			//

			NUMLOCK = 0x90,
			SCROLL = 0x91,

			//
			// L* & R* - left and right Alt, Ctrl and Shift virtual keys.
			// Used only as parameters to GetAsyncKeyState() and GetKeyState().
			// No other API or message will distinguish left and right keys in this way.
			//
			LSHIFT = 0xA0,
			RSHIFT = 0xA1,
			LCONTROL = 0xA2,
			RCONTROL = 0xA3,
			LMENU = 0xA4,
			RMENU = 0xA5,

			BROWSER_BACK = 0xA6,
			BROWSER_FORWARD = 0xA7,
			BROWSER_REFRESH = 0xA8,
			BROWSER_STOP = 0xA9,
			BROWSER_SEARCH = 0xAA,
			BROWSER_FAVORITES = 0xAB,
			BROWSER_HOME = 0xAC,

			VOLUME_MUTE = 0xAD,
			VOLUME_DOWN = 0xAE,
			VOLUME_UP = 0xAF,
			MEDIA_NEXT_TRACK = 0xB0,
			MEDIA_PREV_TRACK = 0xB1,
			MEDIA_STOP = 0xB2,
			MEDIA_PLAY_PAUSE = 0xB3,
			LAUNCH_MAIL = 0xB4,
			LAUNCH_MEDIA_SELECT = 0xB5,
			LAUNCH_APP1 = 0xB6,
			LAUNCH_APP2 = 0xB7,

			//
			// 0xB8 - 0xB9 : reserved
			//

			OEM_1 = 0xBA,   // ';:' for US
			OEM_PLUS = 0xBB,   // '+' any country
			OEM_COMMA = 0xBC,   // ',' any country
			OEM_MINUS = 0xBD,   // '-' any country
			OEM_PERIOD = 0xBE,   // '.' any country
			OEM_2 = 0xBF,   // '/?' for US
			OEM_3 = 0xC0,   // '`~' for US

			//
			// 0xC1 - 0xD7 : reserved
			//

			//
			// 0xD8 - 0xDA : unassigned
			//

			OEM_4 = 0xDB,  //  '[{' for US
			OEM_5 = 0xDC,  //  '\|' for US
			OEM_6 = 0xDD,  //  ']}' for US
			OEM_7 = 0xDE,  //  ''"' for US
			OEM_8 = 0xDF

			//
			// 0xE0 : reserved
			//
		}
	}

	static class NativeMethods
    {
        public const int INPUT_KEYBOARD = 1;
        public const uint KEY_UP = 0x0002;

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Explicit, Size = 28)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public uint type;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
        };

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

    }
}
