using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jg.Editor.Library.Control
{
    /// <summary>
    /// ControlLine.xaml 的交互逻辑
    /// </summary>
    public partial class ControlLine : UserControl
    {
        public bool IsEdit { get; set; }
        Point point = new Point(0, 0);
        bool IsPush = false;
        public ControlLine(bool isEdit)
        {
            InitializeComponent();
            IsEdit = isEdit;
            if (isEdit)
                rectangleEnd.Visibility = System.Windows.Visibility.Visible;
            else
                rectangleEnd.Visibility = System.Windows.Visibility.Hidden;
        }
        private Brush _background = Brushes.Red;

        public new Brush Background
        {
            get { return _background; }
            set
            {
                _background = value;
                rectangle.Fill = value;
            }
        }
        public Point Point1
        {
            get { return new Point(rectangleEnd.Margin.Left, rectangleEnd.Margin.Top); }
            set
            {
                rectangleEnd.Margin = new Thickness(value.X, value.Y, main.ActualWidth - value.X - rectangleEnd.ActualWidth, main.ActualHeight - value.Y - rectangleEnd.ActualHeight);
                DrawLine(rectangleEnd, value);
            }
        }       

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (IsPush && e.LeftButton == MouseButtonState.Pressed)
            {
                element.CaptureMouse();

                DrawLine(element, new Point(e.GetPosition(this).X - point.X, e.GetPosition(this).Y - point.Y));
            }
        }
        void DrawLine(FrameworkElement element, Point newpoint)
        {
            double left, top, right, bottom;
            if (element == null) return;
            
            left = newpoint.X;
            top = newpoint.Y;

            element.Margin = new Thickness(left, top, main.ActualWidth - left - element.ActualWidth, main.ActualHeight - top - element.ActualHeight);
            
            GeometryConverter gc = new GeometryConverter();

            left = Math.Min(rectangleStart.Margin.Left, rectangleEnd.Margin.Left);
            right = Math.Min(rectangleStart.Margin.Right, rectangleEnd.Margin.Right);
            top = Math.Min(rectangleStart.Margin.Top, rectangleEnd.Margin.Top);
            bottom = Math.Min(rectangleStart.Margin.Bottom, rectangleEnd.Margin.Bottom);
            rectangle.Margin = new Thickness(left, top, right, bottom);

        }
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                point = e.GetPosition(element);
                IsPush = true;
            }
        }
        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;
            element.ReleaseMouseCapture();
            IsPush = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            rectangleStart.MouseMove += Rectangle_MouseMove;
            rectangleStart.MouseDown += Rectangle_MouseDown;
            rectangleStart.MouseUp += Rectangle_MouseUp;
            rectangleEnd.MouseMove += Rectangle_MouseMove;
            rectangleEnd.MouseDown += Rectangle_MouseDown;
            rectangleEnd.MouseUp += Rectangle_MouseUp;

            DrawLine(rectangleStart,new Point(0,0));
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawLine(rectangleStart, new Point(rectangleStart.Margin.Left, rectangleStart.Margin.Top));
            DrawLine(rectangleEnd, new Point(rectangleEnd.Margin.Left, rectangleEnd.Margin.Top));
        }
    }
}
