using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Guard {
    internal class StylishMessageBox {
        [DllImport("user32.dll", SetLastError = true)]

        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int MB_OK = 0x00000000;
        private const int MB_ICONINFORMATION = 0x00000040;
        private const int MB_ICONWARNING = 0x00000030;

        public static DialogResult Show(string title, string message, MessageBoxButtons buttons, MessageBoxIcon icon) {
            int style = MB_OK;
            IntPtr iconHandle = IntPtr.Zero;
            switch (icon) {
                case MessageBoxIcon.Information:
                    style |= MB_ICONINFORMATION;
                    break;
                case MessageBoxIcon.Warning:
                    style |= MB_ICONWARNING;
                    break;
            }
            switch (buttons) {
                case MessageBoxButtons.OK:
                    break;
            }
            IntPtr hWnd = FindWindow(null, title);
            SendMessage(hWnd, 0x0018, IntPtr.Zero, IntPtr.Zero);
            return MessageBox.Show(message, title, buttons, icon);
        }
    }
}