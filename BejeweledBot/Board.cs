using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace BejeweledBot
{
    class Board
    {
        private Point corner;
        private Color cornerPixel = Color.FromArgb(0, 114, 82, 59);
        private const int boardRows = 8;
        private const int boardColumns = 8;

        public int TileWidth { get; set; }
        public List<Color> CurrentColorList { get; set; }
        public Color[,] TileColors { get; set; } = new Color[boardRows, boardColumns];
        public int[,] SimplifiedTiles { get; set; } = new int[boardRows, boardColumns];

        public Board(int tileWidth, Bitmap bmp)
        {
            TileWidth = tileWidth;
            getBoardCorner(bmp);
        }

        private void getBoardCorner(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Height * bmp.Width; i++)
            {
                int row = i / bmp.Width;
                int col = i % bmp.Width;
                var pixel = bmp.GetPixel(col, row);
                if (pixel.R == cornerPixel.R && pixel.G == cornerPixel.G)
                {
                    corner = new Point(row, col);
                    break;
                }
            }
        }

        public void Update(Bitmap bmp)
        {
            setTilesColors(bmp);
            simplifyTiles();
        }

        private void setTilesColors(Bitmap bmp)
        {
            for (int row = 0; row < boardRows; row++)
                for (int col = 0; col < boardColumns; col++)
                {
                    //int x = corner.X + row * TileWidth;
                    //int y = corner.Y + col * TileWidth;
                    int x = row * TileWidth;
                    int y = col * TileWidth;
                    Rectangle cloneRect = new Rectangle(x, y, TileWidth, TileWidth);
                    PixelFormat format = bmp.PixelFormat;
                    var clonedobpm = bmp.Clone(cloneRect, format);
                    using (Bitmap tile = clonedobpm)
                    {
                        TileColors[row, col] = getSingleTileAverageColor(tile);
                    }
                }
            SimplifyColors(TileColors);
        }

        private void simplifyTiles()
        {
            int width = TileColors.GetLength(0);
            int height = TileColors.GetLength(1);
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                {
                    SimplifiedTiles[row, col] = CurrentColorList.FindIndex(c => (c.R == TileColors[row, col].R &&
                                                                                c.G == TileColors[row, col].G &&
                                                                                c.B == TileColors[row, col].B));
                }
        }

        private void SimplifyColors(Color[,] colors)
        {
            CurrentColorList = new List<Color>();
            int width = colors.GetLength(0);
            int height = colors.GetLength(1);
            int threshold = 15;//15
            for (int row = 0; row < width; row++)
                for (int col = 0; col < height; col++)
                {
                    Color selectedColor = colors[row, col];
                    int similarColorIndex = CurrentColorList.GetSimilar(selectedColor, threshold);
                    if (similarColorIndex == -1)
                    {
                        CurrentColorList.Add(colors[row, col]);
                    }
                    else
                    {
                        colors[row, col] = CurrentColorList[similarColorIndex];
                    }
                }
        }

        private Color getSingleTileAverageColor(Bitmap tile)
        {
            //http://stackoverflow.com/questions/6177499/how-to-determine-the-background-color-of-document-when-there-are-3-options-usin/6185448#6185448
            int width = tile.Width;
            int height = tile.Height;
            int red = 0;
            int green = 0;
            int blue = 0;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = tile.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = tile.LockBits(new Rectangle(0, 0, tile.Width, tile.Height), ImageLockMode.ReadOnly, tile.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totals[2] += Convert.ToInt32(red / 15) * 15;
                            totals[1] += Convert.ToInt32(green / 15) * 15;
                            totals[0] += Convert.ToInt32(blue / 15) * 15;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }

            int count = width * height - dropped;
            if (count == 0)
            {
                return Color.FromArgb(255, 255, 255);
            }
            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            return Color.FromArgb(avgR, avgG, avgB);
        }
    }

    public static class ExtendColorList
    {
        public static int GetSimilar(this List<Color> colorList, Color sourceColor, int threshold)
        {
            for (int i = 0; i < colorList.Count; i++)
            {
                if (inRange(sourceColor.R, colorList[i].R, threshold) &&
                    inRange(sourceColor.G, colorList[i].G, threshold) &&
                    inRange(sourceColor.B, colorList[i].B, threshold))
                {
                    return i;
                }
            }
            return -1;
        }

        static bool inRange(int current, int expected, int threshold)
        {
            if ((current >= expected - threshold) && current <= expected + threshold)
            {
                return true;
            }
            return false;
        }
    }
}
