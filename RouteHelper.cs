using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Technical_Solution
{
    internal static class RouteHelper
    {
        public static Bitmap CreateRouteMap(Bitmap inputImage, string difficulty, Point start, Point goal)
        {
            Bitmap thinnedImage = RouteImageHelper.CreateThinnedImage(inputImage);
            try
            {
                Dictionary<Point, List<Point>> graph = RouteImageHelper.CreateGraph(thinnedImage);
                List<Point> path;

                if (difficulty == "Easy")
                {
                    path = AStar.FindPath(graph, start, goal);
                }
                else
                {
                    HashSet<Point> exploredNodes = new HashSet<Point>();
                    path = DepthFirstSearch.FindPath(graph, exploredNodes, start, goal);
                }

                return DrawRoute(inputImage, path);
            }
            finally
            {
                thinnedImage.Dispose();
            }
        }

        public static byte[] ConvertImageToBytes(Bitmap routeMap)
        {
            using (Bitmap img = new Bitmap(routeMap))
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static Bitmap DrawRoute(Bitmap image, List<Point> path)
        {
            Bitmap outputImage = new Bitmap(image);

            if (path == null || path.Count == 0)
            {
                return outputImage;
            }

            int width = outputImage.Width;
            int height = outputImage.Height;

            foreach (Point p in path)
            {
                if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                {
                    outputImage.SetPixel(p.X, p.Y, Color.Red);
                }
            }

            return outputImage;
        }
    }
}