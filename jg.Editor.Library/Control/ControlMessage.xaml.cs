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
    /// ControlMessage.xaml 的交互逻辑
    /// </summary>
    public partial class ControlMessage : UserControl
    {
        public bool IsEdit { get; set; }
        Point point = new Point(0, 0);
        bool IsPush = false;

        private Brush _background = Brushes.White, _foreground = Brushes.Black;
        public Point Location
        {
            get
            {
                return new Point(rectangle.Margin.Left,rectangle.Margin.Top);
            }
            set
            {

                double left, top, right, bottom;
                left = value.X;
                top = value.Y;
                right = this.ActualWidth - (left + rectangle.ActualWidth);
                bottom = this.ActualHeight - (top + rectangle.ActualHeight);
                rectangle.Margin = new Thickness(left, top, right, bottom);
                DrawLine(rectangle, new Point(rectangle.Margin.Left, rectangle.Margin.Top));
            }
        }
        public string Title
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }
        //背景色
        public new Brush Background
        {
            get { return _background; }
            set
            {
                _background = value;
                path.Fill = value;
                path.Stroke = value;
                rectangleBack.Fill = value;
            }
        }
        
        //文本色
        public new Brush Foreground
        {
            get { return _foreground; }
            set
            {
                _foreground = value;
                textBox.Foreground = value;
            }
        }


        public ControlMessage(bool isEdit)
        {
            InitializeComponent();
            IsEdit = isEdit;
            if (isEdit)
                rectangle.Visibility = System.Windows.Visibility.Visible;
            else
                rectangle.Visibility = System.Windows.Visibility.Hidden;
        }
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            
            if (IsPush && e.LeftButton == MouseButtonState.Pressed)
            {
                element.CaptureMouse();
               
                DrawLine(element,new Point(e.GetPosition(this).X - point.X, e.GetPosition(this).Y - point.Y));
            }
        }

        void DrawLine(FrameworkElement element,Point newpoint)
        {
            double left, top, right, bottom;
            if (element == null) return;
            double a1, a2, a3, a4, a5, a6;
            a1 = grid.Margin.Left + grid.ActualWidth * 0.25;
            a2 = grid.Margin.Top + grid.ActualHeight * 0.5;
            a3 = newpoint.X;
            a4 = newpoint.Y;
            a5 = grid.Margin.Left + grid.ActualWidth * 0.75;
            a6 = grid.Margin.Top + grid.ActualHeight * 0.5;

            left = newpoint.X;
            top = newpoint.Y;
            right = this.ActualWidth - (left + element.ActualWidth);
            bottom = this.ActualHeight - (top + element.ActualHeight);

            if (bottom < 0)
                path.Margin = new Thickness(path.Margin.Left, path.Margin.Top, path.Margin.Right, bottom);
            if (right < 0)
                path.Margin = new Thickness(path.Margin.Left, path.Margin.Top, right, path.Margin.Bottom);

            element.Margin = new Thickness(left, top, right, bottom);


            GeometryConverter gc = new GeometryConverter();
            path.Data = (Geometry)gc.ConvertFromString(string.Format("M{0},{1} L{2},{3} L{4},{5}", a1, a2, a3, a4, a5, a6));
        }

        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            double x = System.Math.Abs(x1 - x2);
            double y = System.Math.Abs(y1 - y2);
            return Math.Sqrt(x * x + y * y);
        }

        private double Gety1(double len, double x1, double x2, double y2)
        {
            double y1 = Math.Abs(Math.Sqrt(len * len - Math.Abs(x1 - x2) * Math.Abs(x1 - x2)) - y2);
            return y1;
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

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawLine(rectangle,new Point(rectangle.Margin.Left, rectangle.Margin.Top));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawLine(rectangle, new Point(rectangle.Margin.Left, rectangle.Margin.Top));
        }

        
    }
}