using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace web.Handlers
{
    public static class BitmapHandle
    {
        public static void AddBytes(Bitmap bmp, byte[] rawBytes)
        {
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }
    }
}
