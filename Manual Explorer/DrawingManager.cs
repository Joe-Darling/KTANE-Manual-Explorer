using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Markup;
using System.Drawing;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using System.Linq;

namespace Manual_Explorer
{
    class DrawingManager
    {
        private Point currentPoint = new Point();
        private Polyline currentLine;
        private double lineThinkness = 3;
        private Canvas currentCanvas;

        public DrawingManager() { }

        public void ClearPage(Canvas canvas)
        {
            canvas.Children.Clear();
        }

        public void OnMouseEnter(Canvas canvas, MouseEventArgs e)
        {
            currentPoint = e.GetPosition(canvas);
        }

        public void MouseButtonDown(Canvas canvas, MouseButtonEventArgs e)
        {
            if(currentLine != null)
            {
                return;
            }

            if (e.ButtonState == MouseButtonState.Pressed && (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right))
            {
                currentCanvas = canvas;
                currentPoint = e.GetPosition(canvas);

                currentLine = new Polyline();
                currentLine.Stroke = new SolidColorBrush(Colors.Red);
                currentLine.StrokeThickness = lineThinkness;
                currentLine.Points.Add(currentPoint);
                canvas.Children.Add(currentLine);

                if(e.ChangedButton == MouseButton.Right)
                {
                    currentLine.Points.Add(currentPoint);
                }
            }
        }

        public void MouseMove(Canvas canvas, MouseEventArgs e)
        {
            if (currentCanvas == canvas && currentLine != null)
            {
                currentPoint = e.GetPosition(canvas);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    HandleLeftDrag();
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    HandleRightDrag();
                }
            }
        }

        public void MouseButtonUp(MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Released)
            {
                currentCanvas = null;
                currentLine = null;
            }
        }

        private void HandleLeftDrag()
        {
            currentLine.Points.Add(currentPoint);
        }

        private void HandleRightDrag()
        {
            currentLine.Points[currentLine.Points.Count - 1] = currentPoint;
        }
    }
}
