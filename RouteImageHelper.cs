using System.Collections.Generic;
using System.Drawing;

namespace Technical_Solution
{
    internal static class RouteImageHelper
    {
        public static Bitmap CreateThinnedImage(Bitmap inputImage)
        {
            Bitmap roadDetectedImage = EdgeDetection.RoadDetection(inputImage);
            try
            {
                return ImageThinning.ZhangSuenThinning(roadDetectedImage);
            }
            finally
            {
                roadDetectedImage.Dispose();
            }
        }

        public static Dictionary<Point, List<Point>> CreateGraph(Bitmap thinnedImage)
        {
            return Graph.WhitePixels(thinnedImage);
        }
    }
}