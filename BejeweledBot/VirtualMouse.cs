using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BejeweledBot
{
    internal class VirtualMouse
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private Cursor Cursor;

        public VirtualMouse()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
        }

        public void Move(int X, int Y)
        {
            Cursor.Position = new Point(X, Y);
        }

        public void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
        }
    }
}