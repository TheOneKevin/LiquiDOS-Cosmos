using sys = Cosmos.System;

namespace LiquiDOS.Drivers
{
    public class VBE
    {
        public int ClearColor { get; set; } = 0x167F39;
        private int[] Buffer = new int[0];
        public sys.VBEScreen vbeScreen;
        public VBE()
        {
            vbeScreen = new sys.VBEScreen();
        }

        public void init()
        {
            vbeScreen.SetMode(sys.VBEScreen.ScreenSize.Size1024x768, sys.VBEScreen.Bpp.Bpp24);
            Buffer = new int[vbeScreen.ScreenHeight * vbeScreen.ScreenWidth];
            vbeScreen.Clear((uint)ClearColor);
            for (int i = 0; i < vbeScreen.ScreenHeight * vbeScreen.ScreenWidth; i++)
            {
                Buffer[i] = ClearColor;
            }
        }

        public void SetPixel(int x, int y, int c)
        {
            if (vbeScreen.GetPixel((uint)x, (uint)y) != (uint)c)
                SetPixelRaw(x, y, c);
        }

        public void SetPixelRaw(int x, int y, int c)
        {
            if (x < vbeScreen.ScreenWidth && y < vbeScreen.ScreenHeight)
            {
                vbeScreen.SetPixel((uint)x, (uint)y, (uint)c);
                Buffer[(y * vbeScreen.ScreenWidth) + x] = c;
            }
        }

        public int GetPixel(int x, int y)
        {
            return Buffer[(y * vbeScreen.ScreenWidth) + x];
        }

        //Not needed after every draw VV Just too slow!
        public void UpdateBuffer()
        {
            int c = 0;
            for (int x = 0; x < vbeScreen.ScreenWidth; x++)
            {
                for (int y = 0; y < vbeScreen.ScreenHeight; y++)
                {
                    var px = Buffer[(y * vbeScreen.ScreenWidth) + x];
                    if (vbeScreen.GetPixel((uint)x, (uint)y) != px)
                    {
                        vbeScreen.SetPixel((uint)x, (uint)y, (uint)px);
                    }
                    c++;
                }
            }
            for (int i = 0; i < vbeScreen.ScreenHeight * vbeScreen.ScreenWidth; i++)
            {
                Buffer[i] = ClearColor;
            }
        }

        public void DrawRect(int x, int y, int w, int h, int c)
        {
            for (int w1 = 0; w1 < w; w1++)
            {
                for (int h1 = 0; h1 < h; h1++)
                {
                    SetPixel(x + w1, y + h1, c);
                }
            }
        }
    }
}