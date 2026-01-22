using System;
using System.Drawing;
using System.Windows.Forms;

namespace Technical_Solution
{
    internal class RouteViewer : Form
    {
        private readonly PictureBox routePictureBox;
        private readonly Label routeLabel;
        private Point snappedPoint = Point.Empty;
        private bool pointSelected = false;
        private Bitmap thinnedForSnap = null;

        public RouteViewer(string routeName, Bitmap image)
        {
            this.routeLabel = new Label
            {
                Text = routeName,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Height = 40
            };
            this.routePictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = image,
            };
            this.Controls.Add(this.routePictureBox);
            this.Controls.Add(this.routeLabel);
            this.Text = "Route Viewer";
            this.WindowState = FormWindowState.Maximized;
            this.routePictureBox.Image = image;
        }

        public Point GetClickedPoint(Bitmap thinnedImage)
        {
            snappedPoint = Point.Empty;
            pointSelected = false;
            thinnedForSnap = thinnedImage;
            this.routePictureBox.MouseClick += MouseClick;
            this.ShowDialog();
            this.routePictureBox.MouseClick -= MouseClick;
            if (snappedPoint == Point.Empty)
            {
                throw new Exception("No point was selected.");
            }
            return snappedPoint;
        }

        private void MouseClick(object sender, MouseEventArgs mouseEvent)
        {
            Point imagePoint = ClientToImagePoint(mouseEvent.Location);
            if (imagePoint.X >= 0 && imagePoint.Y >= 0 && thinnedForSnap != null)
            {
                snappedPoint = GetSnappedPoint(thinnedForSnap, imagePoint);
                pointSelected = true;
            }
            this.Close();
        }

        private Point ClientToImagePoint(Point clientPoint)
        {
            Image image = routePictureBox.Image;
            Size clientSize = routePictureBox.ClientSize;
            float imageWidth = image.Width;
            float imageHeight = image.Height;
            float clientWidth = clientSize.Width;
            float clientHeight = clientSize.Height;
            if (imageWidth <= 0 || imageHeight <= 0 || clientWidth <= 0 || clientHeight <= 0)
            {
                return Point.Empty;
            }
            float scale = Math.Min(clientWidth / imageWidth, clientHeight / imageHeight);
            float displayedImageWidth = imageWidth * scale;
            float displayedImageHeight = imageHeight * scale;
            float offsetX = (clientWidth - displayedImageWidth) / 2;
            float offsetY = (clientHeight - displayedImageHeight) / 2;
            float xInImage = (clientPoint.X - offsetX) / scale;
            float yInImage = (clientPoint.Y - offsetY) / scale;
            if (xInImage < 0 || xInImage >= imageWidth || yInImage < 0 || yInImage >= imageHeight)
            {
                return Point.Empty;
            }
            int pixelColumn = (int)Math.Floor(xInImage);
            int pixelRow = (int)Math.Floor(yInImage);
            return new Point(pixelColumn, pixelRow);

        }

        public Point GetSnappedPoint(Bitmap thinnedImage, Point clickedPoint)
        {
            Point snappedPoint = Point.Empty;
            double minDistance = double.MaxValue;
            for (int y = 0; y < thinnedImage.Height; y++)
            {
                for (int x = 0; x < thinnedImage.Width; x++)
                {
                    Color pixelColor = thinnedImage.GetPixel(x, y);
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                        double distance = Math.Sqrt(Math.Pow(clickedPoint.X - x, 2) + Math.Pow(clickedPoint.Y - y, 2));
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            snappedPoint = new Point(x, y);
                        }
                    }
                }
            }
            return snappedPoint;
        }
    }
}
