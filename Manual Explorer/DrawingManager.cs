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
        private Line currentLine;
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
    }
}
