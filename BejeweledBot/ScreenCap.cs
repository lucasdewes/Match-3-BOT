using System.Drawing;

namespace BejeweledBot
{
    static class ScreenCap
    {
        public static Bitmap Grab(Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X,
                                 rect.Y,
                                 0, 0,
                                 bmp.Size,
                                 CopyPixelOperation.SourceCopy);
                return bmp;
            }
        }
    }
}
