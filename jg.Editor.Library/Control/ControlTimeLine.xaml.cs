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
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using jg.Editor.Library;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Windows.Interop;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;


namespace jg.Editor.Library
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ControlTimeLine : UserControl
    {
        const int widthStep = 40, heightStep = 21;
        const int Start = 10;

        private DispatcherTimer timer = new DispatcherTimer();
        DateTime dt1 = DateTime.Now, dt2 = DateTime.Now;

        public Guid PageId = Guid.NewGuid();

        //播放进度事件
        public delegate void OnTimeChanging(double Time, AnimationProperty animationPropertyList);
        public event OnTimeChanging TimeChanging = null;
        public Dictionary<UIElement, List<DependencyProperty>> delDictUi = new Dictionary<UIElement, List<DependencyProperty>>();
        public delegate void delChanged();
        public event delChanged EventHandler = null;
        //调整控制Z轴顺序事件
        public delegate void OnSetZIndex(Guid Id, int ZIndex);
        public event OnSetZIndex SetZIndex = null;

        private ObservableCollection<TimeLineItemInfo> timeLineItemList = new ObservableCollection<TimeLineItemInfo>();

        public delegate void OnSelectedItemChanged(object sender, Guid id);
        public event OnSelectedItemChanged SelectedItemChanged = null;

        //选择帧改变
        public delegate void OnSelectedFrameChanged(Guid PageId, Guid ItemId, TimePoint timePoint);
        public event OnSelectedFrameChanged SelectedFrameChanged = null;

        public int _maxtime = 60;

        public int MaxTime
        {
            get { return _maxtime; }
            set { _maxtime = value; DrawImage(); }
        }

        //当前选择的帧
        private Guid SelFrame = Guid.NewGuid();

        //上下拖动操作
        bool IsUp = false;
        Path pathMove = null;
        RadioButton dragControl = null;
        RadioButton targetControl = null;

        public ControlTimeLine()
        {
            InitializeComponent();
        }

        #region 绘制背景

        void DrawImageHead()
        {
            DrawingImage drawingImage = new DrawingImage();
            DrawingGroup drawingGroup = new DrawingGroup();
            GeometryDrawing geometryDrawing = new GeometryDrawing() { Brush = Brushes.Black, Pen = new Pen(Brushes.Black, 1) };
            GeometryGroup geometryGroup = new GeometryGroup();
            GeometryDrawing geometryDrawingBorder = new GeometryDrawing();
            LineGeometry lineGeometry;
            RectangleGeometry rectangleGeometry;
            geometryDrawingBorder.Pen = new Pen() { Brush = Brushes.Transparent, Thickness = 1 };
            rectangleGeometry = new RectangleGeometry(new Rect(0, 0, gridHead.ActualWidth, gridHead.ActualHeight));
            geometryDrawingBorder.Geometry = rectangleGeometry;

            for (int i = 10; i < gridHead.ActualWidth; i += widthStep)
            {
                lineGeometry = new LineGeometry() { StartPoint = new Point(i, 15), EndPoint = new Point(i, 25) };
                geometryGroup.Children.Add(lineGeometry);
                for (int j = 2; j < 10; j += 2)
                {
                    lineGeometry = new LineGeometry() { StartPoint = new Point(i + widthStep * (j * 0.1), 20), EndPoint = new Point(i + widthStep * (j * 0.1), 25) };
                    geometryGroup.Children.Add(lineGeometry);
                }
            }

            geometryDrawing.Geometry = geometryGroup;
            drawingGroup.Children.Add(geometryDrawingBorder);
            drawingGroup.Children.Add(geometryDrawing);
            drawingImage.Drawing = drawingGroup;
            imageHead.Source = drawingImage;

            //drawingImage.Drawing
        }

        void DrawImageBody()
        {
            double height;
            RectangleGeometry rectangleGeometry;
            LineGeometry lineGeometry;
            DrawingImage drawingImage = new DrawingImage();
            DrawingGroup drawingGroup = new DrawingGroup();
            GeometryDrawing geometryDrawingBorder = new GeometryDrawing();
            GeometryDrawing geometryDrawingRect = new GeometryDrawing();
            GeometryDrawing geometryDrawingLine = new GeometryDrawing();


            GeometryGroup geometryRectGroup = new GeometryGroup();
            GeometryGroup geometryLineGroup = new GeometryGroup();

            geometryDrawingBorder.Pen = new Pen() { Brush = Brushes.Transparent };

            geometryDrawingRect.Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCDCDCD"));
            geometryDrawingRect.Pen = new Pen() { Brush = Brushes.Transparent };

            geometryDrawingLine.Brush = Brushes.Black;
            geometryDrawingLine.Pen = new Pen() { Brush = Brushes.Black };

            height = GetDrawHeight();

            rectangleGeometry = new RectangleGeometry(new Rect(0, 0, gridBody.ActualWidth, height));
            geometryDrawingBorder.Geometry = rectangleGeometry;

            for (int i = 0; i < height; i += heightStep)
            {
                rectangleGeometry = new RectangleGeometry(new Rect(0, i, gridBody.ActualWidth, heightStep - 1));
                geometryRectGroup.Children.Add(rectangleGeometry);
            }

            for (int i = 10; i < gridBody.ActualWidth; i += widthStep)
            {
                lineGeometry = new LineGeometry() { StartPoint = new Point(i, 0), EndPoint = new Point(i, height) };
                geometryLineGroup.Children.Add(lineGeometry);
            }
            geometryDrawingRect.Geometry = geometryRectGroup;
            geometryDrawingLine.Geometry = geometryLineGroup;

            drawingGroup.Children.Add(geometryDrawingBorder);
            drawingGroup.Children.Add(geometryDrawingRect);
            drawingGroup.Children.Add(geometryDrawingLine);
            drawingImage.Drawing = drawingGroup;

            imageBody.Source = drawingImage;
        }

        void DrawImage()
        {
            double height;
            gridHead.Width = widthStep * _maxtime;
            gridBody.Width = widthStep * _maxtime;
            DrawImageBody();
            DrawImageHead();
            for (int i = gridHead.Children.Count - 1; i >= 0; i--)
                if (gridHead.Children[i] is TextBlock)
                    gridHead.Children.RemoveAt(i);

            foreach (var v in GetTitle())
                gridHead.Children.Add(v);

            scrollBarV.Maximum = height = GetDrawHeight();
            scrollBarV.Minimum = 0;
            scrollBarV.Value = 0;

            if (height == gridBody.ActualHeight)
                scrollBarV.IsEnabled = false;
            else
                scrollBarV.IsEnabled = true;

            scrollBarH.Maximum = widthStep * _maxtime - column1.ActualWidth;
            scrollBarV.Minimum = 0;
            scrollBarV.Value = 0;
        }
        private double GetDrawHeight()
        {
            double height = 0;
            foreach (var v in gridItem.Children.OfType<RadioButton>())
            {
                height += v.Height;
            }
            height = Math.Max(height, gridBody.ActualHeight);
            return height;
        }

        IEnumerable<TextBlock> GetTitle()
        {
            List<TextBlock> textblockList = new List<TextBlock>();
            TextBlock tb;
            int time = 0;
            for (int i = 10; i < gridHead.ActualWidth; i += widthStep)
            {
                tb = new TextBlock() { FontFamily = new System.Windows.Media.FontFamily("Microsoft YaHei"), Text = time.ToString(), HorizontalAlignment = System.Windows.HorizontalAlignment.Left, Width = widthStep, TextAlignment = TextAlignment.Center };
                tb.Margin = new Thickness(i - widthStep / 2, 0, 0, 0);
                textblockList.Add(tb);
                time++;
            }

            return textblockList;
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawImage();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;

            DrawItem(timeLineItemList, Guid.NewGuid());
        }

        //播放
        void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan span = dt1 - dt2;
            DispatcherTimer dt = sender as DispatcherTimer;
            if (dt != null)
            {
                Guid key = Guid.Parse(dt.Tag.ToString());
                double time = GetCurrentTime();
                if (time >= GetMaxTime(key))
                {
                    timer.Stop(); playItems++; if
                        (playItems < dictionGuidCollection.Keys.Count)
                    {
                        SetAction(playItems);
                    }
                    else
                    {
                        setDelDoubleAnimation();
                        SetPosition(0);
                    } return;
                }

                //设置指针位置
                SetPosition(time + 0.1);

                List<AnimationProperty> animationPropertyList = GetItem(time);
                AnimationProperty animationProperty = animationPropertyList.Find(p => p.ItemId == key);
                if (TimeChanging != null) TimeChanging(time, animationProperty);

                int interval = 100 - Convert.ToInt32(span.TotalMilliseconds - 100);
                timer.Interval = new TimeSpan(0, 0, 0, 0, interval <= 0 ? 100 : interval);

                dt2 = dt1;
                dt1 = DateTime.Now;
            }
        }


        //当前时间的帧
        List<AnimationProperty> GetItem(double time)
        {
            List<AnimationProperty> animationPropertyList = new List<AnimationProperty>();
            AnimationProperty animationProperty;

            foreach (var v in timeLineItemList)
            {
                animationProperty = new AnimationProperty();
                foreach (var vv in v.TimePointList.OrderBy(model => model.Point))
                {
                    if (animationProperty.tp1 != null)
                    {
                        animationProperty.tp2 = vv;
                        break;
                    }
                    if (vv.Point != time)
                        continue;
                    else
                    {
                        animationProperty.tp1 = vv;
                        continue;
                    }
                }
                if (animationProperty.tp1 != null && animationProperty.tp2 != null)
                    animationProperty.timeSpan = GetTimeSpan(animationProperty.tp2.Point - animationProperty.tp1.Point);

                animationProperty.Name = v.Name;
                animationProperty.ItemId = v.Id;
                animationPropertyList.Add(animationProperty);
            }
            return animationPropertyList;
        }

        TimeSpan GetTimeSpan(double Second)
        {
            int second;
            int milliseconds;

            second = (int)Math.Truncate(Second);
            milliseconds = Convert.ToInt32((Second - second) * 1000);
            return new TimeSpan(0, 0, 0, second, milliseconds);
        }

        //设置指针位置
        void SetPosition(double time)
        {
            double left = time * widthStep + (Start - thumb.ActualWidth / 2);
            thumb.Margin = new Thickness(left, thumb.Margin.Top, 0, 0);
        }

        //得到最后时间
        double GetMaxTime()
        {
            double maxTime = 0;
            foreach (var v in timeLineItemList)
                if (v.TimePointList.Count > 0)
                    maxTime = Math.Max(maxTime, v.TimePointList.OrderByDescending(model => model.Point).First().Point);
            return maxTime;
        }
        //得到最后时间
        double GetMaxTime(Guid key)
        {
            double maxTime = 0;
            foreach (var v in timeLineItemList)
                if (v.Id == key)
                {
                    if (v.TimePointList.Count > 0)
                        maxTime = Math.Max(maxTime, v.TimePointList.OrderByDescending(model => model.Point).First().Point);
                }
            return maxTime;
        }
        //得到当前选中的时间点
        double GetCurrentTime()
        {
            double left = Math.Abs(gridBody.Margin.Left) + thumb.Margin.Left - Start + thumb.ActualWidth / 2;
            double time = left / widthStep;

            return Math.Round(time, 1);
        }


        Dictionary<Guid, UIElementCollection> dictionGuidCollection = new Dictionary<Guid, UIElementCollection>();

        //绘制帧
        void DrawFrame(IEnumerable<TimePoint> list, double top, Guid id)
        {
            TimeFrame radioButton;
            for (int i = gridBody.Children.Count - 1; i > 0; i--)
                if (gridBody.Children[i] is TimeFrame)
                {
                    if (Guid.Parse(((TimeFrame)gridBody.Children[i]).Tag.ToString()) == id)
                        gridBody.Children.RemoveAt(i);
                }


            foreach (var v in list)
            {
                radioButton = new TimeFrame(widthStep, Start);
                radioButton.TimePoint = v;
                radioButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                radioButton.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                radioButton.Width = 10;
                radioButton.ActionRectHander -= radioButton_ActionRectHander;
                radioButton.ActionRectHander += radioButton_ActionRectHander;
                radioButton.Tag = id;
                radioButton.Height = heightStep;
                radioButton.Margin = new Thickness(Start + v.Point * widthStep - radioButton.Width / 2, top, 0, 0);
                radioButton.Id = v.Id;
                radioButton.AssetGuid = id;
                ContextMenu menu = new ContextMenu();
                MenuItem item = new MenuItem() { Header = FindResource("FF000031"), Tag = v.Id };
                item.Click += TimeFrameMenuItem_Click;
                menu.Items.Add(item);
                radioButton.ContextMenu = menu;

                if (v.Id == SelFrame) radioButton.IsChecked = true;

                radioButton.Click += (s, e) =>
                {
                    TimeFrame tf = s as TimeFrame;
                    if (tf == null) return;
                    SelFrame = tf.Id;
                    if (SelectedFrameChanged != null)
                        SelectedFrameChanged(PageId, Guid.Parse(tf.Tag.ToString()), tf.TimePoint);
                };

                Panel.SetZIndex(radioButton, 10);
                gridBody.Children.Add(radioButton);

            }
            SetDiction(id, gridBody.Children);
            ActionAddTumb(id);
        }

        void radioButton_ActionRectHander(Guid item)
        {
            ActionAddTumb(item);
        }

        /// <summary>
        ///将时间轴上所有控件，按照资源id存到字典集合里面
        /// </summary>
        /// <param name="key">资源id</param>
        /// <param name="value">时间轴上的控件</param>
        public void SetDiction(Guid key, UIElementCollection value)
        {
            bool IsCantions = false;
            if (dictionGuidCollection.Keys.Count > 0)
            {
                foreach (var v in dictionGuidCollection.Keys)
                {
                    if (v == key)
                    {
                        IsCantions = true;
                        dictionGuidCollection[key] = value;
                        break;
                    }
                }
            }
            if (!IsCantions)
            {
                dictionGuidCollection.Add(key, value);
            }
        }


        /// <summary>
        /// Clear全部矩形
        /// </summary>
        /// <param name="guid"></param>
        public void ClearRect()
        {
            foreach (var item in dictionGuidCollection.Keys)
            {
                IEnumerable<Rectangle> IEnumerableRect = gridBody.Children.OfType<Rectangle>();
                int count = IEnumerableRect.Count();
                for (int i = count - 1; i >= 0; i--)
                {
                    Guid guidkey;

                    Guid.TryParse(IEnumerableRect.ElementAt(i).Tag.ToString(), out guidkey);
                    if (guidkey == item)
                    {
                        gridBody.Children.Remove(IEnumerableRect.ElementAt(i));
                    }
                }
            }

        }



        /// <summary>
        /// Clear矩形
        /// </summary>
        /// <param name="guid"></param>
        public void ClearRect(Guid item)
        {

            IEnumerable<Rectangle> IEnumerableRect = gridBody.Children.OfType<Rectangle>();
            int count = IEnumerableRect.Count();
            for (int i = count - 1; i >= 0; i--)
            {
                Guid guidkey;

                Guid.TryParse(IEnumerableRect.ElementAt(i).Tag.ToString(), out guidkey);

                if (guidkey == item)
                {
                    gridBody.Children.Remove(IEnumerableRect.ElementAt(i));
                }
            }

        }
        /// <summary>
        /// 根据所有资源添加矩形块
        /// </summary>
        public void ActionAddTumbALL()
        {
            ClearRect();
            foreach (var item in dictionGuidCollection.Keys)
            {
                Point point1 = GetMinPoint(item, new Point(0, 0));
                double height = heightStep;
                Point point2 = new Point();
                int count = GetTimeFrameCount(item, dictionGuidCollection[item].OfType<TimeFrame>());
                for (int i = 0; i < count - 1; i++)
                {
                    point2 = GetNextMaxPoint(item, point1);
                    Rectangle rectangle = new Rectangle();
                    rectangle.Height = height;
                    rectangle.Tag = item;
                    rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                    rectangle.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    rectangle.Width = (point2.X - point1.X) < 0 ? 0 : point2.X - point1.X;
                    rectangle.Margin = new Thickness(point1.X, point1.Y - height, 0, 0);
                    rectangle.Fill = new SolidColorBrush(Colors.Yellow);
                    Panel.SetZIndex(rectangle, 10);
                    gridBody.Children.Add(rectangle);
                    point1 = new Point(point2.X + Start, point1.Y);
                }
            }
        }

        /// <summary>
        /// 添加矩形块。针对某一资源
        /// </summary>
        /// <param name="item">资源key</param>
        public void ActionAddTumb(Guid item)
        {
            ClearRect(item);
            Point point1 = GetMinPoint(item, new Point(0, 0));
            double height = heightStep;
            Point point2 = new Point();
            int count = GetTimeFrameCount(item, dictionGuidCollection[item].OfType<TimeFrame>());
            for (int i = 0; i < count - 1; i++)
            {
                point2 = GetNextMaxPoint(item, point1);
                Rectangle rectangle = new Rectangle();
                rectangle.Height = height;
                rectangle.Tag = item;
                rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                rectangle.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                rectangle.Width = (point2.X - point1.X) < 0 ? 0 : point2.X - point1.X;
                rectangle.Margin = new Thickness(point1.X, point1.Y - height, 0, 0);
                rectangle.Fill = new SolidColorBrush(Colors.Yellow);
                Panel.SetZIndex(rectangle, 10);
                gridBody.Children.Add(rectangle);
                point1 = new Point(point2.X + Start, point1.Y);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ParmList"></param>
        /// <returns></returns>
        public int GetTimeFrameCount(Guid item, IEnumerable<TimeFrame> ParmList)
        {
            ObservableCollection<TimeFrame> ObservableCollectionResult = new ObservableCollection<TimeFrame>();
            foreach (var parm in ParmList)
            {
                Guid guidkey;

                Guid.TryParse(parm.Tag.ToString(), out guidkey);

                if (guidkey == item)
                {
                    ObservableCollectionResult.Add(parm);
                }
            }
            return ObservableCollectionResult.Count;


        }
        /// <summary> 
        /// 获取控件组最小的坐标
        /// </summary>
        /// <param name="list">选择的控件组集合</param>
        /// <returns>Point</returns>
        private Point GetMinPoint(Guid key, Point p)
        {
            double maxX = p.X;
            double maxY = p.Y;
            Point pointDouble = new Point();


            foreach (var v in dictionGuidCollection[key].OfType<TimeFrame>())
            {

                Guid guidkey;

                Guid.TryParse(v.Tag.ToString(), out guidkey);

                if (guidkey == key)
                {
                    Thickness point = v.Margin;
                    //X
                    if (maxX == 0)
                    {

                        maxX = point.Left + v.Width;
                    }
                    else
                    {
                        if (maxX > point.Left + v.Width)
                        {
                            maxX = point.Left + v.Width;
                        }
                    }

                    //Y
                    if (maxY == 0)
                    {
                        maxY = point.Top + v.Height;
                    }
                    else
                    {
                        if (maxY > point.Top + v.Height)
                        {
                            maxY = point.Top + v.Height;
                        }
                    }
                }
            }
            pointDouble.X = maxX;
            pointDouble.Y = maxY;
            return pointDouble;
        }
        /// <summary>
        /// 获取控件组最大的坐标
        /// </summary>
        /// <param name="list">选择的控件组集合</param>
        /// <returns>Point</returns>
        private Point GetMaxPoint(Guid key, Point p)
        {
            double maxX = p.X;
            double maxY = p.Y;
            Point pointDouble = new Point();


            foreach (var v in dictionGuidCollection[key].OfType<TimeFrame>())
            {
                Thickness point = v.Margin;
                //X
                if (maxX == 0)
                {
                    maxX = point.Left;
                }
                else
                {
                    if (maxX < point.Left)
                    {
                        maxX = point.Left;
                    }
                }

                //Y
                if (maxY == 0)
                {
                    maxY = point.Top;
                }
                else
                {
                    if (maxY < point.Top)
                    {
                        maxY = point.Top;
                    }
                }
            }
            pointDouble.X = maxX;
            pointDouble.Y = maxY;
            return pointDouble;

        }

        /// <summary>
        /// 获取控件组最大的坐标
        /// </summary>
        /// <param name="list">选择的控件组集合</param>
        /// <returns>Point</returns>
        private Point GetNextMaxPoint(Guid key, Point p)
        {
            double maxX = 0;
            double maxY = 0;
            Point pointDouble = new Point();


            foreach (var v in dictionGuidCollection[key].OfType<TimeFrame>())
            {

                if (v.Margin.Left == p.X - v.Width || v.Margin.Left < p.X - v.Width) continue;
                Guid guidkey;

                Guid.TryParse(v.Tag.ToString(), out guidkey);

                if (guidkey == key)
                {
                    Thickness point = v.Margin;

                    //X
                    if (maxX == 0)
                    {

                        maxX = point.Left;
                    }
                    else
                    {
                        if (maxX > point.Left)
                        {
                            maxX = point.Left;
                        }
                    }
                }

            }
            pointDouble.X = maxX;
            pointDouble.Y = maxY;
            return pointDouble;

        }

        //绘制控件
        void DrawItem(IEnumerable<TimeLineItemInfo> list, Guid SelItem)
        {
            dictionGuidCollection = new Dictionary<Guid, UIElementCollection>();
            double top = 0;
            RadioButton rb;
            int zindex = list.Count();
            Guid CheckedId = System.Guid.NewGuid();

            rb = gridItem.Children.OfType<RadioButton>().FirstOrDefault(model => model.IsChecked == true);
            if (rb != null && rb.IsChecked == true) CheckedId = Guid.Parse(rb.Tag.ToString());

            gridItem.Children.Clear();


            for (int i = gridBody.Children.Count - 1; i > 0; i--)
                if (gridBody.Children[i] is TimeFrame)
                {
                    gridBody.Children.RemoveAt(i);
                }

            for (int i = gridBody.Children.Count - 1; i > 0; i--)
                if (gridBody.Children[i] is Rectangle)
                {
                    gridBody.Children.RemoveAt(i);
                }
            foreach (var v in list)
            {
                rb = new RadioButton()
                {
                    DataContext = v,
                    Content = v.Name,
                    Tag = v.Id,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                    Height = heightStep,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top
                };
                rb.Width = gridItem.ActualWidth;
                if (v.Id == SelItem) rb.IsChecked = true;
                //控件选中状态
                rb.Click += (sender, e) =>
                {
                    RadioButton _rb = sender as RadioButton;
                    if (_rb == null) return;
                    if (_rb.IsChecked != true) return;
                    if (SelectedItemChanged == null) return;

                    SelectedItemChanged(this, Guid.Parse(_rb.Tag.ToString()));
                };

                rb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(radioButton_PreviewMouseLeftButtonDown);
                rb.PreviewMouseMove += new MouseEventHandler(radioButton_PreviewMouseMove);
                rb.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(radioButton_PreviewMouseLeftButtonUp);

                rb.Margin = new Thickness(0, top, 0, 0);
                gridItem.Children.Add(rb);
                DrawFrame(v.TimePointList, top, v.Id); //绘制帧

                if (SetZIndex != null) SetZIndex(v.Id, zindex);

                top += heightStep;
                zindex--;
            }
            DrawImage();
        }

        #region 左侧控件位置拖动
        void radioButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                dragControl = sender as RadioButton;
                pathMove = new Path() { VerticalAlignment = System.Windows.VerticalAlignment.Top, Data = (Geometry)new GeometryConverter().ConvertFromString("M 0,0 L 10,0"), Stretch = Stretch.Fill, Stroke = Brushes.White, StrokeThickness = 2 };
                gridItem.Children.Add(pathMove);
            }
        }

        void radioButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && dragControl != null)
            {
                RadioButton radioButton = Common.FindVisualParent<RadioButton>((DependencyObject)gridItem.InputHitTest(e.GetPosition(gridItem)));
                if (radioButton == null) return;
                if (e.GetPosition(radioButton).Y > radioButton.ActualHeight / 2)
                {
                    IsUp = false;
                    pathMove.Margin = new Thickness(pathMove.Margin.Left, radioButton.Margin.Top + radioButton.ActualHeight, pathMove.Margin.Right, pathMove.Margin.Bottom);
                }
                else
                {
                    IsUp = true;
                    Canvas.SetTop(pathMove, Canvas.GetTop(radioButton));
                    pathMove.Margin = new Thickness(pathMove.Margin.Left, radioButton.Margin.Top, pathMove.Margin.Right, pathMove.Margin.Bottom);
                }
                targetControl = radioButton;
            }
        }

        void radioButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (targetControl != null)
            {
                if (dragControl.Tag != targetControl.Tag)
                {
                    DrawItem(GetList(dragControl, targetControl), Guid.Parse(dragControl.Tag.ToString()));
                }
            }
            dragControl = null;
            targetControl = null;

            gridItem.Children.Remove(pathMove);
        }
        #endregion

        IEnumerable<TimeLineItemInfo> GetList(RadioButton dragControl, RadioButton targetControl)
        {
            TimeLineItemInfo dragInfo = timeLineItemList.First(model => model.Id == Guid.Parse(dragControl.Tag.ToString()));
            if (dragInfo == null) return null;

            TimeLineItemInfo targetInfo = timeLineItemList.First(model => model.Id == Guid.Parse(targetControl.Tag.ToString()));
            if (targetInfo == null) return null;

            timeLineItemList.Remove(dragInfo);

            for (int i = 0; i < timeLineItemList.Count; i++)
                if (timeLineItemList[i].Id == Guid.Parse(targetControl.Tag.ToString()))
                {
                    if (IsUp)
                        timeLineItemList.Insert(i, dragInfo);
                    else
                        timeLineItemList.Insert(i + 1, dragInfo);

                    break;
                }

            return timeLineItemList;
        }

        //尺寸改变重绘
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawImage();
        }

        #region 滚动条
        private void scrollBarV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            gridBody.Margin = new Thickness(gridBody.Margin.Left, 0 - e.NewValue, 0, 0);
            gridItem.Margin = new Thickness(gridItem.Margin.Left, 0 - e.NewValue, 0, 0);
        }

        private void scrollBarH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollBar scrollBar = sender as ScrollBar;
            if (scrollBar == null) return;
            if (e.NewValue > scrollBar.Maximum * .99)
            {
                MaxTime = MaxTime + Convert.ToInt32(MaxTime * 0.01);
            }

            gridBody.Margin = new Thickness(0 - e.NewValue, gridBody.Margin.Top, gridBody.Margin.Right, gridBody.Margin.Bottom);
            gridHead.Margin = new Thickness(0 - e.NewValue, gridHead.Margin.Top, gridHead.Margin.Right, gridHead.Margin.Bottom);
            thumb.Margin = new Thickness(thumb.Margin.Left - (e.NewValue - e.OldValue), thumb.Margin.Top, thumb.Margin.Right, thumb.Margin.Bottom);
        }
        #endregion

        #region 时间指针位置改变
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = sender as Thumb;

            double minLeft = double.MaxValue;

            minLeft = Math.Min(thumb.Margin.Left, minLeft);

            double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
            thumb.Margin = new Thickness(thumb.Margin.Left + deltaHorizontal, thumb.Margin.Top, 0, 0);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = sender as Thumb;

            double start = 10 - thumb.ActualWidth / 2;
            double left = thumb.Margin.Left - start;

            if (thumb.Margin.Left <= thumb.ActualWidth / 2)
                thumb.Margin = new Thickness(start, thumb.Margin.Top, 0, 0);
            else
            {
                if (left % (widthStep / 5) > (widthStep / 5) / 2)
                    thumb.Margin = new Thickness(start + left + (widthStep / 5) - left % (widthStep / 5), thumb.Margin.Top, 0, 0);
                else
                    thumb.Margin = new Thickness(start + left - left % (widthStep / 5), thumb.Margin.Top, 0, 0);
            }
        }

        private void imageBody_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image == null) return;

            double start = Start - thumb.ActualWidth / 2;
            double left = e.GetPosition(imageBody).X + gridBody.Margin.Left - start;

            if (left <= thumb.ActualWidth / 2)
                thumb.Margin = new Thickness(start, thumb.Margin.Top, 0, 0);
            else
            {
                if (left % (widthStep / 5) > Convert.ToDouble((widthStep / 5)) / 2)
                    thumb.Margin = new Thickness(start + left + (widthStep / 5) - left % (widthStep / 5), thumb.Margin.Top, 0, 0);
                else
                    thumb.Margin = new Thickness(start + left - left % (widthStep / 5), thumb.Margin.Top, 0, 0);
            }
        }
        #endregion

        #region 控制功能

        private int playItems = 0;

        private delegate void DelegateWhilePlay();

        DelegateWhilePlay[] DelegateWhilePlaytest;

        /// <summary>
        /// 启动
        /// </summary>
        public void Play()
        {
            playItems = 0;
            delDictUi = new Dictionary<UIElement, List<DependencyProperty>>();
            DelegateWhilePlaytest = new DelegateWhilePlay[dictionGuidCollection.Keys.Count];

            DelegateWhilePlaytest[playItems] = new DelegateWhilePlay(WhilePlay);

            ActionDelegate();

        }

        public void setDelDoubleAnimation()
        {
            foreach (var item in delDictUi.Keys)
            {
                foreach (var item2 in delDictUi[item])
                {
                    item.BeginAnimation(item2, null);
                }

            }
        }
        /// <summary>
        /// 执行动画
        /// </summary>

        public void ActionDelegate()
        {
            DelegateWhilePlaytest[playItems]();
        }

        /// <summary>
        /// 动画
        /// </summary>
        public void WhilePlay()
        {
            gridBody.Dispatcher.Invoke((Action)delegate
            {
                SetPosition(0);
            });

            timer.Dispatcher.Invoke((Action)delegate { timer.Tag = dictionGuidCollection.Keys.ElementAt(playItems); timer.Start(); });

        }

        /// <summary>
        /// 设置下个动画并执行
        /// </summary>
        /// <param name="parm">下一个动画的ID</param>
        public void SetAction(int parm)
        {

            DelegateWhilePlaytest[parm] = new DelegateWhilePlay(WhilePlay);

            ActionDelegate();

        }

        public void Stop()
        {
            SetPosition(0);
            timer.Stop();
            setDelDoubleAnimation();
        }

        public void Pause()
        {
            timer.Stop();
        }



        public void Add(List<abstractAssetProperty> propertyList)
        {
            RadioButton rb = gridItem.Children.OfType<RadioButton>().FirstOrDefault(model => model.IsChecked == true);
            if (rb == null) throw new ArgumentNullException();
            TimeLineItemInfo info = timeLineItemList.FirstOrDefault(model => model.Id == Guid.Parse(rb.Tag.ToString()));
            if (info == null) throw new ArgumentNullException();

            switch (info.assetType)
            {
                case AssetType.Movie:
                case AssetType.Sound:
                    if (info.TimePointList.Count == 0)
                        info.TimePointList.Add(new TimePoint() { Id = Guid.NewGuid(), IsChecked = true, Point = GetCurrentTime(), propertyList = propertyList });
                    if (info.TimePointList.Count == 1)
                        info.TimePointList.Add(new TimePoint() { Id = Guid.NewGuid(), IsChecked = true, Point = info.TimeLength, propertyList = propertyList });
                    break;
                default:
                    info.TimePointList.Add(new TimePoint() { Id = Guid.NewGuid(), IsChecked = true, Point = GetCurrentTime(), propertyList = propertyList });
                    break;
            }

            DrawFrame(info.TimePointList, rb.Margin.Top, info.Id);
        }

        public void Remove()
        {
            RadioButton rb = gridItem.Children.OfType<RadioButton>().FirstOrDefault(model => model.IsChecked == true);
            if (rb == null) throw new ArgumentNullException();



            TimeFrame rbFrame = gridBody.Children.OfType<TimeFrame>().FirstOrDefault(model => model.IsChecked == true);
            if (rbFrame == null) throw new ArgumentNullException();

            TimeLineItemInfo info = timeLineItemList.First(model => model.Id == Guid.Parse(rb.Tag.ToString()));
            if (info == null) throw new ArgumentNullException();

            foreach (var v in info.TimePointList)
                if (v.Id == rbFrame.TimePoint.Id)
                {

                    info.TimePointList.Remove(v);
                    break;
                }
            ActionAddTumb(Guid.Parse(rbFrame.Tag.ToString()));
            gridBody.Children.Remove(rbFrame);

        }

        public bool AddProperty(abstractAssetProperty propertyInfo)
        {
            RadioButton rb = gridItem.Children.OfType<RadioButton>().FirstOrDefault(model => model.IsChecked == true);
            if (rb == null) return false;

            TimeFrame rbFrame = gridBody.Children.OfType<TimeFrame>().FirstOrDefault(model => model.IsChecked == true);
            if (rbFrame == null) return false;

            TimeLineItemInfo info = timeLineItemList.First(model => model.Id == Guid.Parse(rb.Tag.ToString()));
            if (info == null) return false;

            rbFrame.TimePoint.AddProperty(propertyInfo);
            return true;
        }

        public void SetPage(Guid pageId, ObservableCollection<TimeLineItemInfo> timeLineItemList, Guid SelItem)
        {
            PageId = pageId;
            this.timeLineItemList = timeLineItemList;
            DrawItem(timeLineItemList, SelItem);
        }

        #endregion

        private void TimeFrameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item == null) return;
            if (item.Tag == null) return;
            Guid id = (Guid)item.Tag;
            foreach (var v in timeLineItemList)
            {
                foreach (var vv in v.TimePointList)
                    if (vv.Id == id)
                    {
                        TimeFrame tf = gridBody.Children.OfType<TimeFrame>().FirstOrDefault(model => model.Id == vv.Id);
                        if (tf == null) throw new ArgumentNullException();
                        gridBody.Children.Remove(tf);
                        v.TimePointList.Remove(vv);
                        return;
                    }
            }
        }


    }

    //帧控件
    public class TimeFrame : RadioButton
    {
        public int WidthStep { get; set; }
        public int Start { get; set; }

        public TimePoint TimePoint
        {
            get;
            set;
        }
        public delegate void ActionRect(Guid item);
        public event ActionRect ActionRectHander = null;
        public Guid Id { get; set; }

        public Guid AssetGuid { get; set; }
        private Point currentPoint = new Point(0, 0);

        public TimeFrame(int widthstep, int start)
        {
            WidthStep = widthstep;
            Start = start;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(null);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            double minLeft = double.MaxValue, dragDeltaHorizontal = double.MaxValue, minDeltaHorizontal = double.MaxValue;

            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (this.CaptureMouse())
            {
                minLeft = Math.Min(this.Margin.Left, minLeft);
                dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.GetPosition(null).X - currentPoint.X), minDeltaHorizontal);

                this.Margin = new Thickness(this.Margin.Left + dragDeltaHorizontal, this.Margin.Top, 0, 0);
                currentPoint = e.GetPosition(null);
                if (ActionRectHander != null)
                {

                    ActionRectHander(AssetGuid);
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            double left = this.Margin.Left - Start + this.ActualWidth / 2; //去掉开始位置的左边距

            double bound = left % (WidthStep / 10); //距左右最近位置的距离

            if (bound > (WidthStep / 10) / 2)  //大于左边一半以上，向右移
                this.Margin = new Thickness(this.Margin.Left + ((WidthStep / 10) - bound), this.Margin.Top, 0, 0);
            else
                this.Margin = new Thickness(this.Margin.Left - bound, this.Margin.Top, 0, 0);
            TimePoint.Point = (this.Margin.Left - Start + this.ActualWidth / 2) / WidthStep;
        }
    }


    /// <summary>
    /// 时间轴中的节点类
    /// </summary>
    [Serializable]
    public class TimePoint : INotifyPropertyChanged
    {

        public TimePoint()
        {

        }
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public TimePoint(double point)
            : this()
        {
            Point = point;
            _id = System.Guid.NewGuid();
        }

        private bool _ischecked = false;

        /// <summary>
        /// 是否选中
        /// </summary>
        [XmlAttribute("IsChecked")]
        public bool IsChecked
        {
            get { return _ischecked; }
            set { _ischecked = value; OnPropertyChanged("IsChecked"); }
        }

        private Guid _id;
        /// <summary>
        /// Id
        /// </summary>
        [XmlAttribute("Id")]
        public Guid Id { get { return _id; } set { _id = value; OnPropertyChanged("Id"); } }

        double point = 0;
        /// <summary>
        /// 时间 （1/10秒）
        /// </summary>
        [XmlAttribute("Point")]
        public double Point
        {
            get { return point; }
            set
            {
                point = value;
                //this.Margin = new Thickness(point, 0, 0, 0);
                OnPropertyChanged("Point");
            }
        }

        public void AddProperty(abstractAssetProperty propertyInfo)
        {
            var v = propertyList.Find(model => model.Name == propertyInfo.Name);

            if (v != null) propertylist.Remove(v);
            propertyList.Add(propertyInfo);
        }

        private List<abstractAssetProperty> propertylist = new List<abstractAssetProperty> { };
        /// <summary>
        /// 属性列表
        /// </summary>
        public List<abstractAssetProperty> propertyList { get { return propertylist; } set { propertylist = value; } }

        private bool _autoplay = true;
        //动画自动播放
        [XmlAttribute("AutoPlay")]
        public bool AutoPlay
        {
            get { return _autoplay; }
            set { _autoplay = value; }
        }
    }

    /// <summary>
    /// 属性类型
    /// </summary>
    [Serializable]
    public enum PropertyEnum
    {
        DoubleProperty, ColorProperty
    }

    /// <summary>
    /// 属性抽象类
    /// </summary>
    [Serializable]
    public abstract class abstractAssetProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        [XmlAttribute("Name")]
        public abstract string Name { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        [XmlAttribute("propertyEnum")]
        public abstract PropertyEnum propertyEnum { get; }
    }

    /// <summary>
    /// 数值属性
    /// </summary>
    [Serializable]
    public class AssetDoubleProperty : abstractAssetProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        [XmlAttribute("Name")]
        public override string Name { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        [XmlAttribute("propertyEnum")]
        public override PropertyEnum propertyEnum { get { return PropertyEnum.DoubleProperty; } }
        /// <summary>
        /// 属性值
        /// </summary>
        [XmlAttribute("Value")]
        public double Value { get; set; }
    }

    /// <summary>
    /// 颜色属性
    /// </summary>
    [Serializable]
    public class AssetColorProperty : abstractAssetProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        [XmlAttribute("Name")]
        public override string Name { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        [XmlAttribute("propertyEnum")]
        public override PropertyEnum propertyEnum { get { return PropertyEnum.ColorProperty; } }
        /// <summary>
        /// 属性值 
        /// </summary>
        [XmlAttribute("Value")]
        public string Value { get; set; }
    }

    [Serializable]
    public class TimeLineItemInfo : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        //private DesignerItem _designeritem;
        public double TimeLength { get; set; }
        public TimeLineItemInfo()
        {
        }

        public TimeLineItemInfo(string name, int index, Guid ItemId, AssetType assetType, double TimeLength = 0)
            : this()
        {
            Index = index;
            Name = name;
            this.TimeLength = TimeLength;
            _id = ItemId;

            this.assetType = assetType;
        }

        public string Name { get; set; }

        private bool? _ischecked = false;
        public bool? IsChecked
        {
            get { return _ischecked; }
            set { _ischecked = value; OnPropertyChanged("IsChecked"); }
        }

        public int Index { get; set; }

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public AssetType assetType { get; set; }

        private ObservableCollection<TimePoint> _timepointlist = new ObservableCollection<TimePoint>();
        public ObservableCollection<TimePoint> TimePointList
        {
            get { return _timepointlist; }
            set { _timepointlist = value; OnPropertyChanged("TimePointList"); }
        }

        private bool _isvisible = true;
        //是否显示
        public bool IsVisible { get { return _isvisible; } set { _isvisible = value; OnPropertyChanged("IsVisible"); } }

        private bool _islock = false;
        //是否锁定
        public bool IsLock { get { return _islock; } set { _islock = value; OnPropertyChanged("IsLock"); } }
    }
    [Serializable]
    public class AnimationProperty
    {
        public string Name { get; set; }
        public Guid ItemId { get; set; }
        private TimePoint _tp1 = null, _tp2 = null;

        public TimeSpan timeSpan { get; set; }
        public TimePoint tp1 { get { return _tp1; } set { _tp1 = value; } }
        public TimePoint tp2 { get { return _tp2; } set { _tp2 = value; } }
    }


}