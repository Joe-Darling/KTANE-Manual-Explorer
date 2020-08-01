using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manual_Explorer
{
    class DrawingManager
    {
        private Point currentPoint = new Point();
        private Line currentLine;
        private double lineThinkness = 3;
        private Canvas currentCanvas;
        private Dictionary<string, BitmapImage> savedDrawings = new Dictionary<string, BitmapImage>();

        public DrawingManager() { }

        public void ClearPage(Canvas canvas)
        {
            canvas.Children.Clear();
        }

        public void SaveDrawing(string currPage, BitmapImage drawing)
        {
            if (!savedDrawings.ContainsKey(currPage))
            {
                savedDrawings.Add(currPage, drawing);
            }
            Trace.WriteLine(savedDrawings.Count);
        }

        public void OnMouseEnter(Canvas canvas, MouseEventArgs e)
        {
            currentPoint = e.GetPosition(canvas);
        }

        public void MouseButtonDown(Canvas canvas, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentCanvas = canvas;
                currentPoint = e.GetPosition(canvas);
                if(e.ChangedButton == MouseButton.Right)
                {
                    currentLine = new Line();
                    currentLine.X1 = currentPoint.X;
                    currentLine.Y1 = currentPoint.Y;
                    currentLine.StrokeThickness = lineThinkness;
                    currentLine.Stroke = new SolidColorBrush(Colors.Transparent);
                    canvas.Children.Add(currentLine);
                }
            }
        }

        public void MouseMove(Canvas canvas, MouseEventArgs e)
        {
            if (currentCanvas == canvas)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    HandleLeftDrag(canvas, e);
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    HandleRightDrag(canvas, e);
                }
            }
        }

        public void MouseButtonUp(MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Released)
            {
                currentLine = null;
                currentCanvas = null;
            }
        }

        private void HandleLeftDrag(Canvas canvas, MouseEventArgs e)
        {
            Line line = new Line();
            line.X1 = currentPoint.X;
            line.Y1 = currentPoint.Y;
            line.StrokeThickness = lineThinkness;
            line.Stroke = new SolidColorBrush(Colors.Red);//SystemColors.WindowFrameBrush;
            line.X2 = e.GetPosition(canvas).X;
            line.Y2 = e.GetPosition(canvas).Y;
            currentPoint = e.GetPosition(canvas);

            canvas.Children.Add(line);
        }

        private void HandleRightDrag(Canvas canvas, MouseEventArgs e)
        {
            currentLine.Stroke = new SolidColorBrush(Colors.Red);
            currentLine.X2 = e.GetPosition(canvas).X;
            currentLine.Y2 = e.GetPosition(canvas).Y;
        }

        public bool CanvasContentCheck(Canvas canvas) //true if something is drawn
        {
            return canvas.Children.Count != 0;
        }

        public WriteableBitmap SaveAsWriteableBitmap(Canvas canvas)
        {
            if (canvas == null) return null;

            // Save current canvas transform
            Transform transform = canvas.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            canvas.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            //Restore previously saved layout
            canvas.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);
        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }
        //public BitmapImage BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        //{
        //    BitmapImage bmpImage;
        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
        //        enc.Save(outStream);
        //        bmpImage = new BitmapImage(outStream);
        //    }
        //    return bmpImage;
        //}
    }
}
