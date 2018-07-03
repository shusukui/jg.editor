using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace jg.Editor.Library.Control
{

    public class ControlDragCanvas : Canvas
    {
        RectInfo rectInfo = null;
        jg.Editor.Library.DesignerItem SelControl = null;

        Point point = new Point(0, 0);
        Point PushPoint = new Point(0, 0);
        bool IsAddPanel = false;
        Canvas DragPanel = new Canvas();
        public ControlDragCanvas()
        {
            this.PreviewMouseMove += new MouseEventHandler(MainWindow_PreviewMouseMove);
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonDown);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
        }
        void MainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RectInfo rect = null;
            Panel parent;
            if (SelControl == null) return;
            if (SelControl.Parent == null) return;
            if (Math.Abs(e.GetPosition(this).X - PushPoint.X) <= 2 && Math.Abs(e.GetPosition(this).Y - PushPoint.Y) <= 2 && IsAddPanel == false) return;
            if (IsAddPanel == false) return;
                RemovePanel();

            rect = Collision(rectInfo, new Rect(e.GetPosition(this).X, e.GetPosition(this).Y, SelControl.ActualWidth, SelControl.ActualHeight));
            if (rect == null)
                return;

            parent = SelControl.Parent as Panel;
            if (parent != null) parent.Children.Remove(SelControl);
            
            switch (rect.Container.GetType().Name)
            {
                case "DesignerCanvas":
                    rect.Container.Children.Add(SelControl);
                    Canvas.SetLeft(SelControl, e.GetPosition(rect.Container).X - point.X);
                    Canvas.SetTop(SelControl, e.GetPosition(rect.Container).Y - point.Y);
                    break;
                case "Grid":
                    if (rect.Row != -1 && rect.Column != -1)
                    {
                        Grid.SetRow(SelControl, rect.Row);
                        Grid.SetColumn(SelControl, rect.Column);
                    }
                    SelControl.Height = double.NaN;
                    SelControl.Width = double.NaN;
                    rect.Container.Children.Add(SelControl);
                    break;
            }
            SelControl.IsSelected = true;
            SelControl = null;
        }

        void MainWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = new Rectangle();
            DependencyObject element = this.InputHitTest(e.GetPosition(this)) as DependencyObject;
            if (element == null || element == this)
            {
                element = null;
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelControl = Common.FindVisualParent<jg.Editor.Library.DesignerItem>(element);

                if (SelControl == null)
                {
                    point = new Point(0, 0);
                    return;
                }
                IsAddPanel = false;
                if (SelControl.Parent == null) return;
                rectInfo = new RectInfo(this as Panel, new Point(0, 0));


                if (SelControl.Parent is Grid)
                {
                    SelControl.Height = 100;
                    SelControl.Width = 100;
                }
                point = new Point(SelControl.Width / 2, SelControl.Height / 2);

                PushPoint = e.GetPosition(this);
            }


        }

        void MainWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            RectInfo rect;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SelControl == null) return;
                if (Math.Abs(e.GetPosition(this).X - PushPoint.X) <= 2 && Math.Abs(e.GetPosition(this).Y - PushPoint.Y) <= 2) return;
                if (IsAddPanel == false)
                {
                    AddPanel();
                    Panel panel = SelControl.Parent as Panel;
                    if (panel != null) panel.Children.Remove(SelControl);
                    DragPanel.Children.Add(SelControl);
                }
                Canvas.SetLeft(SelControl, e.GetPosition(DragPanel).X - point.X);
                Canvas.SetTop(SelControl, e.GetPosition(DragPanel).Y - point.Y);

                rect = Collision(rectInfo, new Rect(e.GetPosition(this).X, e.GetPosition(this).Y, SelControl.ActualWidth, SelControl.ActualHeight));
                if (rect == null) return;
                foreach (var v in DragPanel.Children.OfType<Rectangle>())
                    v.Opacity = 0.1;
                Rectangle rectangle = DragPanel.Children.OfType<Rectangle>().FirstOrDefault(model => Guid.Parse(model.Tag.ToString()) == rect.Id);
                if (rectangle != null)
                    rectangle.Opacity = 0;
            }
        }

        void AddPanel()
        {
                DragPanel = new Canvas();
                Panel panel;
                Grid grid;
                DragPanel.Background = Brushes.Black;
                panel = DragPanel.Parent as Panel;
                if (panel != null)
                    panel.Children.Remove(DragPanel);
                switch (this.GetType().Name)
                {
                    case "Grid":
                        grid = this.Content as Grid;
                        Grid.SetRow(DragPanel, 0);
                        Grid.SetColumn(DragPanel, 0);
                        Grid.SetRowSpan(DragPanel, grid.RowDefinitions.Count == 0 ? 1 : grid.RowDefinitions.Count);
                        Grid.SetColumnSpan(DragPanel, grid.ColumnDefinitions.Count == 0 ? 1 : grid.ColumnDefinitions.Count);
                        grid.Children.Add(DragPanel);
                        Panel.SetZIndex(DragPanel, grid.Children.Count);
                        break;
                    default:
                        panel = this as Panel;
                        if (panel != null)
                        {
                            panel.Children.Add(DragPanel);
                            Panel.SetZIndex(DragPanel, 0);
                        }
                        break;
                }
                DrawRectangle(rectInfo);
                IsAddPanel = true;

        }

        void DrawRectangle(RectInfo info)
        {
            Rectangle rectangle = new Rectangle() { Height = info.Rect.Height, Width = info.Rect.Width };
            Canvas.SetLeft(rectangle, info.Rect.X);
            Canvas.SetTop(rectangle, info.Rect.Y);
            rectangle.Tag = info.Id;
            rectangle.Fill = Brushes.Black;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 1;
            //rectangle.Opacity = 0.5;
            DragPanel.Children.Add(rectangle);

            foreach (var v in info.Children)
                DrawRectangle(v);
        }

        void RemovePanel()
        {
            Panel panel = DragPanel.Parent as Panel;
            if (panel != null) panel.Children.Remove(DragPanel);
        }

        RectInfo Collision(RectInfo info, Rect rect)
        {
            RectInfo rectInfo = null;
            RectInfo _rectInfo = null;
            if (rect.IntersectsWith(info.Rect))
            {
                rectInfo = info;
                foreach (var v in info.Children)
                {
                    _rectInfo = Collision(v, rect);
                    if (_rectInfo != null)
                    {
                        rectInfo = _rectInfo;
                        break;
                    }
                }
            }
            return rectInfo;
        }

        public Panel Content { get; set; }
    }

    public class RectInfo
    {
        public Guid Id { get; set; }
        public RectInfo()
        {
            Id = Guid.NewGuid();
            Children = new List<RectInfo>();
            Row = -1;
            Column = -1;
        }
        public RectInfo(Panel panel, Point point)
            : this()
        {
            ControlTextGrid grid;
            List<RectInfo> rectlist = new List<RectInfo>();
            Rect = new Rect(point.X, point.Y, panel.ActualWidth, panel.ActualHeight);
            Container = panel;
            foreach (DesignerItem v in panel.Children.OfType<DesignerItem>())
            {
                if (((ToolboxItem)v.Content).Content.GetType().Name == "ControlTextGrid")
                {
                    grid = ((ToolboxItem)v.Content).Content as ControlTextGrid;
                    Children.Add(GetRect(grid, point));
                }
            }
        }
        public Rect Rect { get; set; }
        public Panel Container { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }

        public List<RectInfo> Children { get; set; }

        List<RectInfo> GetRectList(Grid grid, Point point)
        {
            double height = 0, width = 0;

            List<RectInfo> rectList = new List<RectInfo>();
            RectInfo rectInfo;
            if (grid == null) return rectList;
            foreach (FrameworkElement element in grid.Children)
            {
                if (element.GetType().Name == "DesignerCanvas" || element.GetType().Name == "Grid")
                {
                    rectInfo = GetRect(element, new Point(point.X + element.Margin.Left, point.Y + element.Margin.Top));

                    rectList.Add(rectInfo);
                }
            }

            for (int row = 0; row < grid.RowDefinitions.Count; row++)
            {
                width = 0;
                for (int column = 0; column < grid.ColumnDefinitions.Count; column++)
                {
                    rectInfo = new RectInfo() { Container = grid, Rect = new Rect(width, height, grid.ColumnDefinitions[column].ActualWidth, grid.RowDefinitions[row].ActualHeight) };
                    rectInfo.Row = row;
                    rectInfo.Column = column;
                    rectList.Add(rectInfo);
                    width += grid.ColumnDefinitions[column].ActualWidth;
                }
                height += grid.RowDefinitions[row].ActualHeight;
            }
            return rectList;
        }

        List<RectInfo> GetRectList(Canvas canvas, Point point)
        {
            List<RectInfo> rectList = new List<RectInfo>();
            RectInfo rectInfo;
            if (canvas == null) return rectList;

            foreach (FrameworkElement element in canvas.Children)
            {
                if (element is Panel)
                {
                    rectInfo = GetRect(element, point);
                    rectList.Add(rectInfo);
                }
            }
            return rectList;
        }

        RectInfo GetRect(FrameworkElement element, Point point)
        {
            RectInfo rectInfo;
            Grid grid;
            double height = 0, width = 0;
            rectInfo = new RectInfo();

            switch (element.Parent.GetType().Name)
            {
                case "DesignerCanvas":
                    point = new Point(double.IsNaN(point.X) ? 0 : point.X + Canvas.GetLeft(element), double.IsNaN(point.Y) ? 0 : point.Y + Canvas.GetTop(element));
                    rectInfo.Rect = new Rect(double.IsNaN(point.X) ? 0 : point.X, double.IsNaN(point.Y) ? 0 : point.Y, element.ActualWidth, element.ActualHeight);
                    rectInfo.Container = element as Panel;
                    break;
                case "ControlTextGrid":
                    grid = element.Parent as Grid;
                    rectInfo.Container = element as Panel;
                    for (int i = 0; i < Grid.GetRow(element); i++)
                        height += grid.RowDefinitions[i].ActualHeight;
                    for (int i = 0; i < Grid.GetColumn(element); i++)
                        width += grid.ColumnDefinitions[i].ActualWidth;

                    point = new Point(point.X + width, point.Y + height);
                    rectInfo.Rect = new Rect(point.X, point.Y, element.ActualWidth, element.ActualHeight);

                    break;
            }
            switch (element.GetType().Name)
            {
                case "DesignerCanvas":
                    rectInfo.Children.AddRange(GetRectList(element as ControlDragCanvas, point));
                    break;
                case "ControlTextGrid":
                    rectInfo.Children.AddRange(GetRectList(element as Grid, point));
                    break;
            }
            return rectInfo;
        }
    }
}