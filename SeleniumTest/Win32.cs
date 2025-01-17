using System.Drawing;
using System.Runtime.InteropServices;

namespace SeleniumTest;

public class Win32
{
    [DllImport("user32.dll")]
    public static extern long SetCursorPos(int x, int y);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(
        uint dwFlags,
        uint dx,
        uint dy,
        uint cButtons,
        uint dwExtraInfo
    );

    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    public static void DoMouseClick(uint x, uint y)
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
    }

    /// <summary>
    /// Struct representing a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    /// <summary>
    /// Retrieves the cursor's position, in screen coordinates.
    /// </summary>
    /// <see>See MSDN documentation for further information.</see>
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    public static Point GetCursorPosition()
    {
        GetCursorPos(out var lpPoint);
        // NOTE: If you need error handling
        // bool success = GetCursorPos(out lpPoint);
        // if (!success)

        return lpPoint;
    }
}
