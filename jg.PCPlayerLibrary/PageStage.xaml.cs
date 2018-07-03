namespace jg.PCPlayerLibrary
{
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
    using jg.Editor.Library;
    using jg.Editor.Library.Topic;
    using System.Windows.Markup;
    using System.Xml;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Forms;
    using System.Windows.Forms.Integration;
    using System.ComponentModel;
    using System.Windows.Media.Animation;
    using System.Xml.Serialization;
    using jg.Editor.Library.Control;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;

    /// <summary>
    /// PageStage.xaml 的交互逻辑
    /// </summary>
    public partial class PageStage : Page, IDisposable,IWin32Window
    {
        // 标志，指示Dispose是否已被调用.
        private bool disposed = false;
        private string _direction = "";
        //private static double _height = 0, _width = 0, _left = 0, _top = 0;
        //static int _zindex = 0;
        //static bool IsZoom = false; 
        public static event RoutedEventHandler ScaleChanged = null;
        public string Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        public static readonly DependencyProperty PageItemProperty =
            DependencyProperty.Register("PageItem",
            typeof(SavePageInfo),
            typeof(PageStage),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PageItemProperty_Changed)));
        //音量
        public double Volume
        {
            set
            {
                foreach (var v in canvas.Children.OfType<ToolboxItem>())
                {
                    switch (v.AssetType)
                    {
                        case AssetType.Sound:
                            MediaPlayer mediaPlayer = v.Content as MediaPlayer;
                            if (mediaPlayer != null)
                            {
                                mediaPlayer.Volume = value;
                            }
                            break;
                        case AssetType.Movie:
                            MediaElement mediaElement = v.Content as MediaElement;
                            if (mediaElement != null)
                            {
                                mediaElement.Volume = value;
                            }
                            break;
                    }
                }

            }
        }

        enumStageSwitch abstractswitch = enumStageSwitch.SwitchFF00003D;
        static Guid SelItemId = Guid.NewGuid();
        public event RoutedEventHandler PageLoaded = null;

        public static event RoutedPropertyChangedEventHandler<Guid> PageChanged = null;

        List<TimeLineItemInfo> timeLineItemList = new List<TimeLineItemInfo>();
        static windowAssetShow _windowAssetShow;
        public PageStage(enumStageSwitch abstractswitch, SavePageInfo pageitem, string direction)
        {
            InitializeComponent();
            PageItem = pageitem;
            _direction = direction;
            this.abstractswitch = abstractswitch;
            this.Loaded += new RoutedEventHandler(PageStage_Loaded);
            this.Unloaded += PageStage_Unloaded;
            foreach (var v in PageItem.saveItemList)
                timeLineItemList.Add(v.timeLineItemInfo);

        }
        void PageStage_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

                foreach (var v in canvas.Children.OfType<ToolboxItem>())
                {

                    switch (v.AssetType)
                    {
                        case AssetType.Sound:
                            ((MediaPlayer)v.Content).Stop();
                            break;
                        case AssetType.Movie:
                            try
                            {
                                ((jg.Editor.Library.Control.ControlMediaElement)v.Content).Stop();
                            }
                            catch (Exception ex)
                            {

                                System.Windows.MessageBox.Show(ex.Message);
                            }

                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public double PlayAnimation(double time, double oldTime, Guid key)
        {

            bool value = true;
            List<AnimationProperty> animationPropertyList;
            ToolboxItem toolboxItem;
            double stopMinute = 0;


            if (Math.Round(Math.Abs(time - oldTime), 1) != 0.1 && time != 0)
            {
                SetProgress(time);
            }
            foreach (var v in timeLineItemList)
            {
                if (v.TimePointList.FirstOrDefault(model => model.Point == time && v.Id == key) == null) continue;
                animationPropertyList = GetItem(time);
                AnimationProperty animationProperty = animationPropertyList.Find(p => p.ItemId == key);

                stopMinute = Math.Max(animationProperty.timeSpan.TotalSeconds, stopMinute);

                foreach (var vv in v.TimePointList)//遇到不自动播放的祯，返回false,暂停播放。
                {
                    if (!vv.AutoPlay)
                        value = false;
                }

                toolboxItem = GetToolboxItem(v.Id);
                if (toolboxItem == null) return -1;
                switch (toolboxItem.AssetType)
                {
                    case AssetType.Sound:
                        ((MediaPlayer)toolboxItem.Content).Play();
                        break;
                    case AssetType.Movie:
                        ((MediaElement)toolboxItem.Content).Position = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(time * 100));
                        ((MediaElement)toolboxItem.Content).Play();
                        break;
                    default:
                        Play(animationProperty, toolboxItem);
                        break;
                }
            }
            if (!value)
                return stopMinute;
            else
                return -1;

        }
        public void Pause(bool IsPause)
        {
            ToolboxItem toolboxItem;
            foreach (var v in timeLineItemList)
            {
                for (int i = 0; i < v.TimePointList.Count - 1; i++)
                {
                    toolboxItem = GetToolboxItem(v.Id);
                    if (toolboxItem == null) continue;
                    switch (toolboxItem.AssetType)
                    {
                        case AssetType.Movie:
                            if (IsPause)
                                ((ControlMediaElement)toolboxItem.Content).Pause();
                            else
                                ((ControlMediaElement)toolboxItem.Content).Play();

                            break;
                        case AssetType.Sound:
                            if (IsPause)
                                ((MediaPlayer)toolboxItem.Content).Pause();
                            else
                                ((MediaPlayer)toolboxItem.Content).Play();
                            break;
                    }
                }
            }
        }
        private void SetProgress(double time)
        {
            int second = 0, millisecond = 0;

            ToolboxItem toolboxItem;
            foreach (var v in timeLineItemList)
            {
                for (int i = 0; i < v.TimePointList.Count - 1; i++)
                {
                    if (v.TimePointList[i].Point <= time && v.TimePointList[i + 1].Point > time)
                    {
                        toolboxItem = GetToolboxItem(v.Id);
                        if (toolboxItem == null) return;
                        second = (int)Math.Truncate(time - v.TimePointList[i].Point);
                        millisecond = (int)(Math.Truncate(time - v.TimePointList[i].Point) - second) * 1000;
                        switch (toolboxItem.AssetType)
                        {
                            case AssetType.Movie:
                                ((MediaElement)toolboxItem.Content).Position = new TimeSpan(0, 0, 0, second, millisecond);
                                break;
                            case AssetType.Sound:
                                ((MediaPlayer)toolboxItem.Content).Position = new TimeSpan(0, 0, 0, second, millisecond);
                                break;
                        }
                    }
                }
            }
        }
        void Play(AnimationProperty animationProperty, ToolboxItem toolboxItem)
        {
            TransformGroup transformGroup;
            RotateTransform rotateTransform;
            ScaleTransform scaleTransform;
            SkewTransform skewTransform;
            TranslateTransform translateTransform;

            var vv = animationProperty;
            if (vv == null) return;
            if (vv.tp1 == null || vv.tp2 == null) return;
            foreach (var vvv in vv.tp1.propertyList)
            {
                switch (vvv.propertyEnum)
                {
                    case PropertyEnum.ColorProperty:
                        break;
                    case PropertyEnum.DoubleProperty:
                        double v1, v2;
                        AssetDoubleProperty dp1, dp2;
                        dp1 = vvv as AssetDoubleProperty;
                        dp2 = vv.tp2.propertyList.Find(model => model.Name == dp1.Name) as AssetDoubleProperty;
                        if (dp1 == null || dp2 == null) break;
                        if (dp1.Value == dp2.Value) continue;
                        v1 = dp1.Value;
                        v2 = dp2.Value;

                        DoubleAnimation doubleAnimation = new DoubleAnimation(v1, v2, vv.timeSpan);

                        switch (vvv.Name)
                        {
                            case "PropertyFontSizeCommand":// 字号动画
                                Animation(toolboxItem, DesignerItem.FontSizeProperty, doubleAnimation);
                                break;
                            case "PropertyXCommand": // X轴动画
                                Animation(toolboxItem, Canvas.LeftProperty, doubleAnimation);
                                break;
                            case "PropertyYCommand":// Y轴动画
                                Animation(toolboxItem, Canvas.TopProperty, doubleAnimation);
                                break;
                            case "PropertyWidthCommand":// 宽度动画
                                Animation(toolboxItem, DesignerItem.WidthProperty, doubleAnimation);
                                break;
                            case "PropertyHeightCommand":// 高度动画
                                Animation(toolboxItem, DesignerItem.HeightProperty, doubleAnimation);
                                break;
                            case "PropertyOpacityCommand": // 透明度
                                Animation(toolboxItem, Canvas.OpacityProperty, doubleAnimation);
                                break;
                            case "PropertyRotateCommand":// 旋转动画
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                rotateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "RotateTransform") as RotateTransform;
                                if (rotateTransform != null)
                                    Animation(rotateTransform, RotateTransform.AngleProperty, doubleAnimation);
                                break;
                            case "PropertyScaleXCommand":// X轴缩放
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                                if (scaleTransform != null)
                                    Animation(scaleTransform, ScaleTransform.ScaleXProperty, doubleAnimation);
                                break;
                            case "PropertyScaleYCommand":// X轴缩放
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                scaleTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "ScaleTransform") as ScaleTransform;
                                if (scaleTransform != null)
                                    Animation(scaleTransform, ScaleTransform.ScaleYProperty, doubleAnimation);
                                break;
                            case "PropertySkewXCommand":// X轴2D变幻
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                                if (skewTransform != null)
                                    Animation(skewTransform, SkewTransform.AngleXProperty, doubleAnimation);
                                break;
                            case "PropertySkewYCommand":// Y轴2D变幻
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                skewTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "SkewTransform") as SkewTransform;
                                if (skewTransform != null)
                                    Animation(skewTransform, SkewTransform.AngleYProperty, doubleAnimation);
                                break;
                            case "PropertyTranslateXCommand":// X轴平移
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                                if (translateTransform != null)
                                    Animation(translateTransform, TranslateTransform.XProperty, doubleAnimation);
                                break;
                            case "PropertyTranslateYCommand":// Y轴平移
                                transformGroup = toolboxItem.RenderTransform as TransformGroup;
                                if (null == transformGroup) return;
                                translateTransform = transformGroup.Children.FirstOrDefault(model => model.GetType().Name == "TranslateTransform") as TranslateTransform;
                                if (translateTransform != null)
                                    Animation(translateTransform, TranslateTransform.YProperty, doubleAnimation);
                                break;
                        }
                        break;
                }
            }

        }
        void Animation(UIElement element, DependencyProperty property, DoubleAnimation doubleAnimation)
        {
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            element.BeginAnimation(property, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
        }
        void Animation(Transform transform, DependencyProperty property, DoubleAnimation doubleAnimation)
        {
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            transform.BeginAnimation(property, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
        }
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
        ToolboxItem GetToolboxItem(Guid id)
        {
            return (from model in canvas.Children.OfType<ToolboxItem>() where model.ItemId == id select model).FirstOrDefault();
        }
        void PageStage_Loaded(object sender, RoutedEventArgs e)
        {
            var v = GetSwitch(abstractswitch);
            v.Storyboard.Completed += new EventHandler(Storyboard_Completed);
            rectangle.BeginStoryboard(v.Storyboard);

            try
            {
                foreach (var vv in canvas.Children.OfType<ToolboxItem>())
                {
                    switch (vv.AssetType)
                    {
                        case AssetType.Sound:
                            ((MediaPlayer)vv.Content).Play();
                            break;
                          
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }



        }
        void Storyboard_Completed(object sender, EventArgs e)
        {
            if (PageLoaded != null)
                PageLoaded(this, new RoutedEventArgs());
        }
        abstractSwitch GetSwitch(enumStageSwitch stageSwitch)
        {
            abstractSwitch abstractswitch = null;
            switch (stageSwitch)
            {
                case enumStageSwitch.SwitchFF00003D:
                    abstractswitch = new SwitchFF00003D(canvas, rectangle, new TimeSpan(0, 0, 0, 0, 800));
                    break;
                case enumStageSwitch.SwitchFF00003E:
                    abstractswitch = new SwitchFF00003E(canvas, rectangle, new TimeSpan(0, 0, 0, 0, 800));
                    break;
                case enumStageSwitch.SwitchFF00003F:
                    abstractswitch = new SwitchFF00003F(canvas, rectangle, new TimeSpan(0, 0, 0, 0, 800), _direction);
                    break;
                case enumStageSwitch.SwitchFF000040:
                    abstractswitch = new SwitchFF000040(canvas, rectangle, new TimeSpan(0, 0, 0, 0, 800));
                    break;
                case enumStageSwitch.SwitchFF00008C:
                    abstractswitch = new SwitchFF00008C(canvas, rectangle, new TimeSpan(0, 0, 0, 0, 800));
                    break;

            }
            return abstractswitch;
        }
        public SavePageInfo PageItem
        {
            get { return (SavePageInfo)GetValue(PageItemProperty); }
            set
            {
                SetValue(PageItemProperty, value);
            }
        }
        public double GetMaxTime()
        {
            double maxTime = 0;
            foreach (var v in timeLineItemList)
            {
                maxTime += GetMaxTime(v.Id);
            }
            return maxTime;
        }

        public List<TimeLineItemInfo> GettimeLineItemList()
        {
            return timeLineItemList;
        }

        public double GetMaxTime(Guid item)
        {
            double maxTime = 0;
            foreach (var v in timeLineItemList)
                if (v.Id == item)
                {
                    if (v.TimePointList.Count > 0)
                        maxTime = Math.Max(maxTime, v.TimePointList.OrderByDescending(model => model.Point).First().Point);
                }
            return maxTime;
        }
        public Guid PageId { get; set; }
        private static void PageItemProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageStage pageStage = d as PageStage;
            if (pageStage == null) return;
            pageStage.grid.Height = pageStage.PageItem.Height;
            pageStage.grid.Width = pageStage.PageItem.Width;
            pageStage.PageId = pageStage.PageItem.PageId;
            pageStage.canvas.Height = pageStage.PageItem.Height;
            pageStage.canvas.Width = pageStage.PageItem.Width;
            pageStage.canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(pageStage.PageItem.Background));

            ToolboxItem.DirectoryAssResInfo = new Dictionary<string, System.Collections.ObjectModel.ObservableCollection<string>>();
            ToolboxItem.DirectoryTpage = new Dictionary<string, TPageControl>();
            foreach (var v in pageStage.PageItem.saveItemList)
            {
                UIElement uiElement = CreateElement(v);
                pageStage.canvas.Children.Add(uiElement);
            }
        }
        public static UIElement CreateElement(SaveItemInfo info)
        {
            Canvas canvas;
            ToolboxItem item = null;

            double x, y;

            item = ToolboxItem.GetToolBoxItemPreview(info);

            if (item.Content is jg.Editor.Library.Control.ControlMediaElement)
            {
                ((jg.Editor.Library.Control.ControlMediaElement)item.Content).ScaleChanged += PageStage_ScaleChanged;
            }


            item.DataContext = info;
            x = Canvas.GetLeft(item);
            y = Canvas.GetTop(item);

            if (item.AssetType != AssetType.HTML5)
            {
                item.MouseEnter += (sender, e) =>
                    {
                        item = sender as ToolboxItem;
                        if (item == null) return;
                        canvas = item.Parent as Canvas;
                        if (canvas == null) return;

                        if (info.assetActionInfo == null) return;
                        if (string.IsNullOrEmpty(info.assetActionInfo.AssetName)) return;

                        var selectedItems = canvas.Children.OfType<ToolboxItem>().FirstOrDefault(model => model.ItemName == info.assetActionInfo.AssetName);
                        if (selectedItems == null) return;
                        if (info.assetActionInfo.AssetEvent != enumAssetEvent.MouseEnter) return;
                        AssetAction(info, selectedItems, x, y);
                    };

                ShowMaxBox smb = item.Content as ShowMaxBox;

                if (smb != null)
                {
                    smb.eventShowAsset += item_showAsset;
                }


                item.MouseLeave += (sender, e) =>
                    {
                        item = sender as ToolboxItem;
                        if (item == null) return;

                        canvas = item.Parent as Canvas;
                        if (canvas == null) return;
                        if (info.assetActionInfo == null) return;
                        if (string.IsNullOrEmpty(info.assetActionInfo.AssetName)) return;
                        var selectedItems = canvas.Children.OfType<ToolboxItem>().FirstOrDefault(model => model.ItemName == info.assetActionInfo.AssetName);
                        if (selectedItems == null) return;
                        if (info.assetActionInfo.AssetEvent != enumAssetEvent.MouseEnter) return;
                        UnAssetAction(info, selectedItems, x, y);
                    };

                item.MouseLeftButtonUp += (sender, e) =>
                    {
                        try
                        {
                            item = sender as ToolboxItem;
                            if (item == null) return;

                            canvas = item.Parent as Canvas;
                            if (canvas == null) return;

                            if (string.IsNullOrEmpty(info.assetActionInfo.AssetName)) return;
                            switch (info.assetActionInfo.AssetAction)
                            {
                                case enumAssetAction.PageJump:
                                    PageChanged(item, new RoutedPropertyChangedEventArgs<Guid>(info.ItemId, Guid.Parse(info.assetActionInfo.AssetName)));
                                    break;
                                default:
                                    var selectedItems = canvas.Children.OfType<ToolboxItem>().FirstOrDefault(model => model.ItemName == info.assetActionInfo.AssetName);
                                    if (selectedItems == null) return;
                                    if (info.assetActionInfo.AssetEvent != enumAssetEvent.MouseClick) return;
                                    if (SelItemId == item.ItemId)
                                    {
                                        UnAssetAction(info, selectedItems, x, y);
                                        SelItemId = Guid.NewGuid();
                                    }
                                    else
                                    {
                                        AssetAction(info, selectedItems, x, y);
                                        SelItemId = item.ItemId;
                                    }
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    };
            }
           
            return item;
        }

        static void item_Loaded(object sender, RoutedEventArgs e)
        {
            ToolboxItem item = sender as ToolboxItem;
            ShowHTML5(item);
        }
        static void ShowHTML5(ToolboxItem item)
        {
            

            WindowsFormsHost wfh = item.Content as WindowsFormsHost;

            WebBrowerGecko gecko = (wfh.Child as WebBrowerGecko);

            gecko.Navigate(wfh.Tag.ToString());

            
        }

        static void item_showAsset(UIElement item, ObservableCollection<string> liststring)
        {
            windowAssetShow windowAssetShow = new windowAssetShow(AssetType.Image, liststring);
            windowAssetShow.item = item;
            windowAssetShow.ShowDialog();
        }
        static void PageStage_ScaleChanged(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            if (ScaleChanged != null) ScaleChanged(sender, e);
            timer.Tick += (a, b) =>
            {

                _windowAssetShow = new windowAssetShow(AssetType.Movie, null);
                ((ControlMediaElement)sender).Pause();
                _windowAssetShow.Position = ((ControlMediaElement)sender).Position;

                _windowAssetShow.AssetPath = ((ControlMediaElement)sender).Source.OriginalString;

                _windowAssetShow.ShowDialog();
                ((ControlMediaElement)sender).btnZoomChecked = true;
                ((ControlMediaElement)sender).Position = _windowAssetShow.Position;

                ((ControlMediaElement)sender).Play();

                timer.Stop();
            };
            timer.Start();

        }

        //static void PageStage_ScaleChanged(object sender, RoutedEventArgs e)
        //{
        //    //jg.Editor.Library.Control.ControlMediaElement element = sender as jg.Editor.Library.Control.ControlMediaElement;
        //    //int zindex = 0;

        //    //ToolboxItem toolBoxItem = element.Parent as ToolboxItem;



        //    //Canvas canvas = toolBoxItem.Parent as Canvas;
        //    //if (!IsZoom)
        //    //{
        //    //    _height = toolBoxItem.ActualHeight;
        //    //    _width = toolBoxItem.ActualWidth;
        //    //    _top = Canvas.GetTop(toolBoxItem);
        //    //    _left = Canvas.GetLeft(toolBoxItem);
        //    //    _zindex = System.Windows.Controls.Panel.GetZIndex(toolBoxItem);

        //    //    foreach (UIElement v in canvas.Children)
        //    //        zindex = Math.Max(zindex, System.Windows.Controls.Panel.GetZIndex(v));

        //    //    System.Windows.Controls.Panel.SetZIndex(toolBoxItem, zindex++);
        //    //    toolBoxItem.Height = 600;
        //    //    toolBoxItem.Width = 800;
        //    //    Canvas.SetTop(toolBoxItem, 0);
        //    //    Canvas.SetLeft(toolBoxItem, 0);
        //    //}
        //    //{
        //    //    System.Windows.Controls.Panel.SetZIndex(toolBoxItem, _zindex);
        //    //    Canvas.SetTop(toolBoxItem, _top);
        //    //    Canvas.SetLeft(toolBoxItem, _left);
        //    //    toolBoxItem.Height = _height;
        //    //    toolBoxItem.Width = _width;
        //    //}

        //    //IsZoom = !IsZoom;

        //}
        static void item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ToolboxItem toolBoxItem = sender as ToolboxItem;
            if (toolBoxItem == null) return;

            switch (toolBoxItem.Content.GetType().Name)
            {
                case "MediaElement":
                    MediaElement me = toolBoxItem.Content as MediaElement;
                    if (me == null) break;
                    me.Play();
                    break;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            // 调用Dispose需要只是GC不要再调用Finalize（）.
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            // 检查是否已被调用过.
            if (!this.disposed)
            {          
                disposed = true;
            }
        }
        static void AssetAction(SaveItemInfo info, ToolboxItem item, double x, double y)
        {
            Canvas canvas = item.Parent as Canvas;
            if (canvas == null) return;
            if (item == null) return;

            switch (info.assetActionInfo.AssetAction)
            {
                case enumAssetAction.Fade://淡入淡出
                    AssetActionFade(info.assetActionInfo, item, 0, 1);
                    break;
                case enumAssetAction.Left: //左侧飞入
                    AssetActionMove(info.assetActionInfo, item, ((SaveItemInfo)item.DataContext).X, ((SaveItemInfo)item.DataContext).X + canvas.ActualWidth);
                    break;
                case enumAssetAction.Right://右侧飞入
                    AssetActionMove(info.assetActionInfo, item, ((SaveItemInfo)item.DataContext).X, ((SaveItemInfo)item.DataContext).X - canvas.ActualWidth);
                    break;
            }
        }
        static void UnAssetAction(SaveItemInfo info, ToolboxItem item, double x, double y)
        {
            Canvas canvas = item.Parent as Canvas;
            if (canvas == null) return;
            if (item == null) return;
            switch (info.assetActionInfo.AssetAction)
            {
                case enumAssetAction.Fade:
                    AssetActionFade(info.assetActionInfo, item, 1, 0);
                    break;
                case enumAssetAction.Left:
                    AssetActionMove(info.assetActionInfo, item, ((SaveItemInfo)item.DataContext).X, ((SaveItemInfo)item.DataContext).X - canvas.ActualWidth);
                    break;
                case enumAssetAction.Right:
                    AssetActionMove(info.assetActionInfo, item, ((SaveItemInfo)item.DataContext).X, ((SaveItemInfo)item.DataContext).X + canvas.ActualWidth);
                    break;
            }
        }
        static TimeSpan GetSpan(double Second)
        {
            TimeSpan span;
            int second = (int)Math.Truncate(Second);
            int Milliseconds = (int)((Second - second) * 1000);
            span = new TimeSpan(0, 0, 0, second, Milliseconds);
            return span;
        }
        //淡入淡出
        static void AssetActionFade(AssetActionInfo assetAction, ToolboxItem item, double fromValue, double toValue)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(toValue, new Duration(GetSpan(assetAction.Time)));
            item.IsEnabled = false;
            doubleAnimation.Completed += (sender, e) =>
            {
                item.IsEnabled = true;
            };
            item.BeginAnimation(DesignerItem.OpacityProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
        }
        //飞入
        static void AssetActionMove(AssetActionInfo assetAction, ToolboxItem item, double fromValue, double toValue)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(toValue, new Duration(GetSpan(assetAction.Time)));
            item.IsEnabled = false;
            doubleAnimation.Completed += (sender, e) =>
            {
                item.IsEnabled = true;
            };

            item.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
        }

        public IntPtr Handle
        {
            get { throw new NotImplementedException(); }
        }
    }
}