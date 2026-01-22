using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Solution
{
    internal class ImageThinning
    {
        public static Bitmap ZhangSuenThinning(Bitmap binaryImage)
        {
            int width = binaryImage.Width;
            int height = binaryImage.Height;
            bool changed;
            bool[,] pixels = new bool[width, height];
            for (int imageRow = 0; imageRow < height; imageRow++)
            {
                for (int imageColumn = 0; imageColumn < width; imageColumn++)
                {
                    if (binaryImage.GetPixel(imageColumn, imageRow).R == 255)
                    {
                        pixels[imageColumn, imageRow] = true;
                    }
                    else
                    {
                        pixels[imageColumn, imageRow] = false;
                    }
                }
            }
            do
            {
                changed = false;
                List<(int, int)> pixelsToRemove = new List<(int, int)>();
                for (int neighboursRow = 1; neighboursRow < height - 1; neighboursRow++)
                {
                    for (int neighboursColumn = 1; neighboursColumn < width - 1; neighboursColumn++)
                    {
                        if (!pixels[neighboursColumn, neighboursRow])
                        {
                            continue;
                        }
                        int neighborCount = CountNeighbors(pixels, neighboursColumn, neighboursRow);
                        int transitionCount = CountTransitions(pixels, neighboursColumn, neighboursRow);
                        if (neighborCount >= 2 && neighborCount <= 6 && transitionCount == 1 && !(pixels[neighboursColumn, neighboursRow - 1] && pixels[neighboursColumn + 1, neighboursRow] && pixels[neighboursColumn, neighboursRow + 1]) && !(pixels[neighboursColumn + 1, neighboursRow] && pixels[neighboursColumn, neighboursRow + 1] && pixels[neighboursColumn - 1, neighboursRow]))
                        {
                            pixelsToRemove.Add((neighboursColumn, neighboursRow));
                        }
                    }
                }
                if (pixelsToRemove.Count > 0)
                {
                    changed = true;
                    foreach ((int, int) pixel in pixelsToRemove)
                    {
                        pixels[pixel.Item1, pixel.Item2] = false;
                    }
                }
                pixelsToRemove.Clear();
                for (int neighboursRow = 1; neighboursRow < height - 1; neighboursRow++)
                {
                    for (int neighboursColumn = 1; neighboursColumn < width - 1; neighboursColumn++)
                    {
                        if (!pixels[neighboursColumn, neighboursRow])
                        {
                            continue;
                        }
                        int neighborCount = CountNeighbors(pixels, neighboursColumn, neighboursRow);
                        int transitionCount = CountTransitions(pixels, neighboursColumn, neighboursRow);
                        if (neighborCount >= 2 && neighborCount <= 6 && transitionCount == 1 && !(pixels[neighboursColumn, neighboursRow - 1] && pixels[neighboursColumn + 1, neighboursRow] && pixels[neighboursColumn - 1, neighboursRow]) && !(pixels[neighboursColumn, neighboursRow - 1] && pixels[neighboursColumn, neighboursRow + 1] && pixels[neighboursColumn - 1, neighboursRow]))
                        {
                            pixelsToRemove.Add((neighboursColumn, neighboursRow));
                        }
                    }
                }
                if (pixelsToRemove.Count > 0)
                {
                    changed = true;
                    foreach ((int, int) pixel in pixelsToRemove)
                    {
                        pixels[pixel.Item1, pixel.Item2] = false;
                    }
                }
            } while (changed);
            Bitmap thinnedImage = new Bitmap(width, height);
            for (int imageRow = 0; imageRow < height; imageRow++)
            {
                for (int imageColumn = 0; imageColumn < width; imageColumn++)
                {
                    if (pixels[imageColumn, imageRow])
                    {
                        thinnedImage.SetPixel(imageColumn, imageRow, Color.White);
                    }
                    else
                    {
                        thinnedImage.SetPixel(imageColumn, imageRow, Color.Black);
                    }
                }
            }
            return thinnedImage;
        }

        private static int CountNeighbors(bool[,] pixels, int xPosition, int yPosition)
        {
            int neighbours = 0;
            if (pixels[xPosition, yPosition - 1])
            {
                neighbours++;
            }
            if (pixels[xPosition + 1, yPosition - 1])
            {
                neighbours++;
            }
            if (pixels[xPosition + 1, yPosition])
            {
                neighbours++;
            }
            if (pixels[xPosition + 1, yPosition + 1])
            {
                neighbours++;
            }
            if (pixels[xPosition, yPosition + 1])
            {
                neighbours++;
            }
            if (pixels[xPosition - 1, yPosition + 1])
            {
                neighbours++;
            }
            if (pixels[xPosition - 1, yPosition])
            {
                neighbours++;
            }
            if (pixels[xPosition - 1, yPosition - 1])
            {
                neighbours++;
            }
            return neighbours;
        }

        private static int CountTransitions(bool[,] pixels, int xPosition, int yPosition)
        {
            int transitions = 0;
            bool[] neighbors = new bool[8];
            neighbors[0] = pixels[xPosition, yPosition - 1];
            neighbors[1] = pixels[xPosition + 1, yPosition - 1];
            neighbors[2] = pixels[xPosition + 1, yPosition];
            neighbors[3] = pixels[xPosition + 1, yPosition + 1];
            neighbors[4] = pixels[xPosition, yPosition + 1];
            neighbors[5] = pixels[xPosition - 1, yPosition + 1];
            neighbors[6] = pixels[xPosition - 1, yPosition];
            neighbors[7] = pixels[xPosition - 1, yPosition - 1];
            for (int i = 0; i < 8; i++)
            {
                if (!neighbors[i] && neighbors[(i + 1) % 8])
                {
                    transitions++;
                }
            }
            return transitions;
        }
    }
}
