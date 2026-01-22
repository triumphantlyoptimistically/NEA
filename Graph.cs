using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Technical_Solution
{
    internal class Graph
    {
        public static Dictionary<Point, List<Point>> WhitePixels(Bitmap image)
        {
            List<Point> whitePixels = new List<Point>();
            for (int imageRow = 0; imageRow < image.Height; imageRow++)
            {
                for (int imageColumn = 0; imageColumn < image.Width; imageColumn++)
                {
                    Color pixelColor = image.GetPixel(imageColumn, imageRow);
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                        whitePixels.Add(new Point(imageColumn, imageRow));
                    }
                }
            }
            return CreateAdjacencyList(whitePixels);
        }

        public static Dictionary<Point, List<Point>> CreateAdjacencyList(List<Point> whitePixels)
        {
            HashSet<Point> nodeSet = new HashSet<Point>(whitePixels);
            Dictionary<Point, List<Point>> adjacencyList = new Dictionary<Point, List<Point>>();
            int[] horizontalDirections = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] verticalDirections = { -1, -1, -1, 0, 0, 1, 1, 1 };
            foreach (var whitePixel in whitePixels)
            {
                int horizontalPosition = whitePixel.X;
                int verticalPosition = whitePixel.Y;
                List<Point> neighbours = new List<Point>();
                for (int i = 0; i < horizontalDirections.Length; i++)
                {
                    int newHorizontalPosition = horizontalPosition + horizontalDirections[i];
                    int newVerticalPosition = verticalPosition + verticalDirections[i];
                    Point neighbour = new Point(newHorizontalPosition, newVerticalPosition);
                    if (nodeSet.Contains(neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                }
                adjacencyList[whitePixel] = neighbours;
            }
            return adjacencyList;
        }
    }
}

