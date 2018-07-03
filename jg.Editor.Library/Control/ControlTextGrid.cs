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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Forms.Integration;
using jg.Editor.Library.Topic;
using System.Windows.Markup;
using System.Xml;
using System.IO;
namespace jg.Editor.Library.Control
{
    public class ControlTextGrid : Grid, INotifyPropertyChanged
    {
        public bool IsEdit { get; set; }
        private int rowcount = 0, columncount = 0, borderwidth = 1;
        private Brush _textbackground = Brushes.Transparent, _textforeground = Brushes.Black;

        //文本框背景色
        public Brush TextBackground
        {
            get { return _textbackground; }
            set
            {
                _textbackground = value;
                foreach (var v in base.Children)
                    switch(v.GetType().Name)
                    {
                        case "TextBox":
                            ((TextBox)v).Background = value;
                            break;
                        case "TextBlock":
                            ((TextBlock)v).Background = value;
                            break;
                    }
                        
            }
        }

        //文本框前景色
        public Brush TextForeground
        {
            get { return _textforeground; }
            set
            {
                _textforeground = value;
                foreach (var v in base.Children)
                    if (v is TextBox)
                        ((TextBox)v).Foreground = value;
            }
        }

        //表格行数
        public int RowCount
        {
            get { return rowcount; }
            set
            {
                rowcount = value;
                //if (_isnew) 
                DrawGrid();
            }
        }

        //表格列数
        public int ColumnCount
        {
            get { return columncount; }
            set
            {
                columncount = value;
                //if (_isnew)
                DrawGrid();
            }
        }

        //边框线宽度
        public int BorderWidth
        {
            get { return borderwidth; }
            set
            {
                borderwidth = value;
                DrawBackground();
                OnPropertyChanged("BorderWidth");

            }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ContentGrid), typeof(ControlTextGrid), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(SourceProperty_Changed)));

        public ContentGrid Source
        {
            get { return (ContentGrid)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void SourceProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ControlTextGrid controlTextGrid = d as ControlTextGrid;
            ContentGrid controlGrid = e.NewValue as ContentGrid;

            if (controlTextGrid == null) return;
            if (controlGrid == null) return;

            controlTextGrid.RowCount = controlGrid.RowCount;
            controlTextGrid.ColumnCount = controlGrid.ColumnCount;
            controlTextGrid.BorderWidth = controlGrid.BorderWidth;
            ToolboxItem.DirectoryAssResInfo = new Dictionary<string, System.Collections.ObjectModel.ObservableCollection<string>>();
            ToolboxItem.DirectoryTpage = new Dictionary<string, TPageControl>();
            
            foreach (var v in controlGrid.List)
            {
                if (v.Content != null)
                    controlTextGrid.SetContent(v.Column, v.Row, v.Content);
                if (v.Children != null)
                    controlTextGrid.SetContent(v.Column, v.Row, v.Children);
            }
        }

        public void SetContent(int Column, int Row, string Content)
        {
            TextBox txt = (from model in this.Children.OfType<TextBox>() where Grid.GetColumn(model) == Column && Grid.GetRow(model) == Row select model).FirstOrDefault();
            if (txt == null) return;
            txt.Text = Content;

        }

        public void SetContent(int Column, int Row, SaveItemInfo children)
        {
            ToolboxItem toolBoxItem;
            
            UIElement txt = (from model in this.Children.OfType<UIElement>() where Grid.GetColumn(model) == Column && Grid.GetRow(model) == Row select model).FirstOrDefault();
            if (txt != null)
                Children.Remove(txt);
            if (IsEdit == true)
                toolBoxItem = ToolboxItem.GetToolBoxItem(children);
            else
                toolBoxItem = ToolboxItem.GetGridChildrenPreview(children);

            Grid.SetRow(toolBoxItem, Row);
            Grid.SetColumn(toolBoxItem, Column);
            Children.Add(toolBoxItem);
        }

        public ControlTextGrid()
        {
            IsEdit = true;
            base.AllowDrop = true;
            this.SizeChanged += new SizeChangedEventHandler(ControlGrid_SizeChanged);
            this.Loaded += new RoutedEventHandler(ControlTextGrid_Loaded);
            this.PreviewDragEnter += new DragEventHandler(ControlTextGrid_PreviewDragEnter);
        }

        void ControlTextGrid_PreviewDragEnter(object sender, DragEventArgs e)
        {
            foreach (UIElement v in Children)
            {
                if (v is TextBox)
                    v.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        void ControlTextGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //DrawGrid(); 
        }

        //控件尺寸变化时重绘边框
        void ControlGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawBackground();
        }

        //绘制边框
        void DrawBackground()
        {
            LineGeometry lineGeometryLeft = new LineGeometry(new Point(0, 0), new Point(0, base.ActualHeight));
            LineGeometry lineGeometryTop = new LineGeometry(new Point(0, 0), new Point(base.ActualWidth, 0));
            LineGeometry lineGeometryRight = new LineGeometry(new Point(base.ActualWidth, 0), new Point(base.ActualWidth, base.ActualHeight));
            LineGeometry lineGeometryBottom = new LineGeometry(new Point(0, base.ActualHeight), new Point(base.ActualWidth, base.ActualHeight));

            GeometryGroup myGeometryGroup = new GeometryGroup();
            myGeometryGroup.Children.Add(lineGeometryLeft);
            myGeometryGroup.Children.Add(lineGeometryTop);
            myGeometryGroup.Children.Add(lineGeometryRight);
            myGeometryGroup.Children.Add(lineGeometryBottom);

            GeometryDrawing myGeometryDrawing = new GeometryDrawing();
            myGeometryDrawing.Geometry = myGeometryGroup;

            DrawingGroup myDrawingGroup = new DrawingGroup();
            myDrawingGroup.Children.Add(myGeometryDrawing);

            Pen myPen = new Pen();

            myPen.Thickness = BorderWidth;

            myPen.Brush = Brushes.Black;
            myGeometryDrawing.Pen = myPen;

            DrawingBrush myDrawingImage = new DrawingBrush();
            myDrawingImage.Drawing = myDrawingGroup;

            this.Background = myDrawingImage;
        }

        //根据行数列数绘制表格
        void DrawGrid()
        {
            ColumnDefinition columnDefinition;
            RowDefinition rowDefinition;
            GridSplitter gridSplitter;

            Binding binding;
            if (RowCount == 0 || ColumnCount == 0) return;
            base.ColumnDefinitions.Clear();
            base.RowDefinitions.Clear();
            base.Children.Clear();

            for (int i = 0; i < RowCount; i++)
            {
                rowDefinition = new RowDefinition() { AllowDrop = true };
                base.RowDefinitions.Add(rowDefinition);
                if (i < RowCount - 1)
                    base.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            }

            for (int i = 0; i < ColumnCount; i++)
            {
                columnDefinition = new ColumnDefinition() { AllowDrop = true };
                base.ColumnDefinitions.Add(columnDefinition);
                if (i < ColumnCount - 1)
                    base.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            }

            for (int row = 0; row < base.RowDefinitions.Count; row += 2)
                for (int column = 0; column < base.ColumnDefinitions.Count; column += 2)
                {
                    base.Children.Add(GetTextBox(column, row));
                }

            for (int column = 1; column < base.ColumnDefinitions.Count; column += 2)
            {
                gridSplitter = new GridSplitter() { AllowDrop = false, Background = Brushes.Black, ShowsPreview = true, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                binding = new Binding() { Source = this, Path = new PropertyPath("BorderWidth"), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
                gridSplitter.SetBinding(GridSplitter.WidthProperty, binding);
                Grid.SetColumn(gridSplitter, column);
                Grid.SetRowSpan(gridSplitter, base.RowDefinitions.Count);
                base.Children.Add(gridSplitter);
            }

            for (int row = 1; row < base.RowDefinitions.Count; row += 2)
            {
                gridSplitter = new GridSplitter() { AllowDrop = false, Background = Brushes.Black, ResizeDirection = GridResizeDirection.Rows, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
                binding = new Binding() { Source = this, Path = new PropertyPath("BorderWidth"), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
                gridSplitter.SetBinding(GridSplitter.HeightProperty, binding);

                Grid.SetRow(gridSplitter, row);
                Grid.SetColumnSpan(gridSplitter, base.ColumnDefinitions.Count);
                base.Children.Add(gridSplitter);
            }
        }

        UIElement GetTextBox(int column, int row)
        {
            UIElement element = null;

            TextBox tb = new TextBox()
            {
                BorderThickness = new Thickness(column == 0 ? 1 : 0, row == 0 ? 1 : 0, column == base.ColumnDefinitions.Count - 1 ? 1 : 0, row == base.RowDefinitions.Count - 1 ? 1 : 0),
                BorderBrush = Brushes.Transparent,
                Background = TextBackground,
                AcceptsReturn = true,
                Foreground = TextForeground,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            if (IsEdit)
                tb.IsReadOnly = false;
            else
                tb.IsReadOnly = true;
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, column);
            element = (UIElement)tb;

            //if (IsEdit)
            //{
            //    TextBox tb = new TextBox()
            //    {
            //        BorderThickness = new Thickness(column == 0 ? 1 : 0, row == 0 ? 1 : 0, column == base.ColumnDefinitions.Count - 1 ? 1 : 0, row == base.RowDefinitions.Count - 1 ? 1 : 0),
            //        BorderBrush = Brushes.Transparent,
            //        Background = TextBackground,
            //        AcceptsReturn = true,
            //        Foreground = TextForeground,
            //        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
            //        VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
            //        TextWrapping = TextWrapping.Wrap
            //    };
            //    Grid.SetRow(tb, row);
            //    Grid.SetColumn(tb, column);
            //    element = (UIElement)tb;
            //}
            //else
            //{
            //    TextBlock tb = new TextBlock()
            //    {
            //        Background = TextBackground,
            //        Foreground = TextForeground,
            //        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            //        VerticalAlignment = System.Windows.VerticalAlignment.Center,
            //        TextWrapping = TextWrapping.Wrap
            //    };
            //    Grid.SetRow(tb, row);
            //    Grid.SetColumn(tb, column);
            //    element = (UIElement)tb;
            //}
            return element;
        }

        void tb_Drop(object sender, DragEventArgs e)
        {
            DropControl(e);
        }
        
        //处理拖放操作
        protected override void OnDrop(DragEventArgs e)
        {
            DropControl(e);
            base.OnDrop(e);
        }

        void DropControl(DragEventArgs e)
        {
            if (IsEdit == false) return;

            Point point = e.GetPosition(this);
            int row = 0, column = 0;
            ToolboxItem toolboxItem ;
            toolboxItem = e.Data.GetData("TOOLBOXITEM") as ToolboxItem;
            
            DesignerItem designerItem = toolboxItem.Parent as DesignerItem;
            if (designerItem == null) return;

            designerItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            designerItem.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                       
            
            Panel panel = designerItem.Parent as Panel;

            designerItem.Content = null;
            if (panel != null)
                panel.Children.Remove(designerItem);

            row = GetGridRow(point.Y);
            column = GetGridColumn(point.X);

            foreach (UIElement v in base.Children)
            {
                if (v == null) continue;
                if (Grid.GetRow(v) == row && Grid.GetColumn(v) == column)
                {
                    base.Children.Remove(v);
                    break;
                }
            }

            foreach (UIElement v in Children)
                if (v is TextBox)
                    v.Visibility = System.Windows.Visibility.Visible;

            Grid.SetRow(toolboxItem, row);
            Grid.SetColumn(toolboxItem, column);

            base.Children.Add(toolboxItem);


            for (row = 0; row < base.RowDefinitions.Count; row += 2)
                for (column = 0; column < base.ColumnDefinitions.Count; column += 2)
                    if (Children.OfType<UIElement>().FirstOrDefault(model => ((int)Grid.GetRow(model) == row) && ((int)Grid.GetColumn(model) == column)) == null)
                    {
                        Children.Add(GetTextBox(column, row));
                        return;
                    }
        }

        //根据Y座标获得当前行
        int GetGridRow(double Y)
        {
            double height = 0;
            double[] rowList = new double[base.RowDefinitions.Count];
            for (int i = 0; i < base.RowDefinitions.Count; i++)
                rowList[i] = base.RowDefinitions[i].ActualHeight;

            for (int i = 0; i < rowList.Length; i++)
                if (Y < (height += rowList[i]))
                {
                    if (rowList[i] == 1)
                        return i + 1;
                    else
                        return i;
                }
            return 0;
        }

        //根据X座标获得当前列
        int GetGridColumn(double X)
        {
            double height = 0;
            double[] columnList = new double[base.ColumnDefinitions.Count];
            for (int i = 0; i < base.ColumnDefinitions.Count; i++)
                columnList[i] = base.ColumnDefinitions[i].ActualWidth;

            for (int i = 0; i < columnList.Length; i++)
                if (X < (height += columnList[i]))
                {
                    if (columnList[i] == 1)
                        return i + 1;
                    else
                        return i;
                }
            return 0;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region INotifyPropertyChanged Members
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}