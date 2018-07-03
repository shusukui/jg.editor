namespace jg.Editor
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

    /// <summary>
    /// PageStage.xaml 的交互逻辑
    /// </summary>
    public partial class PageStage : Page, IDisposable
    {        
        enumStageSwitch abstractswitch = enumStageSwitch.SwitchFF00003D;
        static Guid SelItemId = Guid.NewGuid();
        public event RoutedEventHandler PageLoaded = null;
        private string _direction = "";

        List<TimeLineItemInfo> timeLineItemList = new List<TimeLineItemInfo>();

        public PageStage(enumStageSwitch abstractswitch,SavePageInfo pageitem,string direction)
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
            foreach (var v in canvas.Children.OfType<ToolboxItem>())
            {
                switch (v.AssetType)
                {
                    case AssetType.Sound:
                        ((MediaPlayer)v.Content).Stop();
                        break;
                    case AssetType.Movie:
                        ((MediaElement)v.Content).Stop();
                        break;
                }
            }
        }
        
        public double PlayAnimation(double time)
        {
            bool value = true;
            List<AnimationProperty> animationPropertyList;
            ToolboxItem toolboxItem;
            double stopMinute = 0;
            foreach (var v in timeLineItemList)
            {
                if (v.TimePointList.FirstOrDefault(model => model.Point ==time) == null) continue;
                animationPropertyList = GetItem(time);
                foreach (var vv in animationPropertyList)
                    stopMinute = Math.Max(vv.timeSpan.TotalSeconds, stopMinute);

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
                        ((MediaElement)toolboxItem.Content).Position = new TimeSpan(0, 0, 0, 0,Convert.ToInt32(time * 100));
                        ((MediaElement)toolboxItem.Content).Play();
                        break;
                    default:
                        Play(animationPropertyList, toolboxItem);
                        break;
                }
            }
            if (!value)
                return stopMinute;
            else
                return -1;

        }

        void Play(List<AnimationProperty> animationPropertyList,ToolboxItem toolboxItem)
        {
            TransformGroup transformGroup;
            RotateTransform rotateTransform;
            ScaleTransform scaleTransform;
            SkewTransform skewTransform;
            TranslateTransform translateTransform;

            foreach (var vv in animationPropertyList)
            {
                if (vv == null) continue;
                if (vv.tp1 == null || vv.tp2 == null) continue;
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
                if (v.TimePointList.Count > 0)
                    maxTime = Math.Max(maxTime, v.TimePointList.OrderByDescending(model => model.Point).First().Point);
            return maxTime;
        }

        public Guid PageId { get; set; }

        public static readonly DependencyProperty PageItemProperty =
            DependencyProperty.Register("PageItem",
            typeof(SavePageInfo),
            typeof(PageStage),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PageItemProperty_Changed)));

        private static void PageItemProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageStage pageStage = d as PageStage;
            if (pageStage == null) return;
            pageStage.grid.Height = pageStage.PageItem.Height;
            pageStage.grid.Width = pageStage.PageItem.Width;
            pageStage.PageId = pageStage.PageItem.PageId;
            pageStage.canvas.Height = pageStage.PageItem.Height;
            pageStage.canvas.Width = pageStage.PageItem.Width;
            pageStage.canvas.Background = XamlReader.Load(XmlReader.Create(new StringReader(pageStage.PageItem.Background))) as Brush; 

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
            item.DataContext = info;
            x = Canvas.GetLeft(item);
            y = Canvas.GetTop(item);

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
            item.MouseDoubleClick += (sender, e) =>
                { 
                };
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
                    item = sender as ToolboxItem;
                    if (item == null) return;

                    canvas = item.Parent as Canvas;
                    if (canvas == null) return;
                    if (info.assetActionInfo == null) return;
                    if (string.IsNullOrEmpty(info.assetActionInfo.AssetName)) return;
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
                };
            return item;
        }
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

        // 标志，指示Dispose是否已被调用.
        private bool disposed = false;

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

                foreach (var v in canvas.Children)
                {
                    ToolboxItem item = v as ToolboxItem;
                    if (item == null) continue;

                    if (item.Content.GetType().Name == typeof(WindowsFormsHost).Name)
                    {
                        WindowsFormsHost host = (WindowsFormsHost)item.Content;
                        switch (host.Child.GetType().Name)
                        {
                            case "AxUnityWebPlayer":
                                AxUnityWebPlayerAXLib.AxUnityWebPlayer unity = host.Child as AxUnityWebPlayerAXLib.AxUnityWebPlayer;
                                if (unity != null)
                                {
                                    unity.Dispose();
                                    unity = null;
                                    GC.Collect();
                                }
                                break;
                        }
                    }
                }
                disposed = true;
            }
        }

        static void AssetAction(SaveItemInfo info, ToolboxItem item,double x,double y)
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

        static void UnAssetAction(SaveItemInfo info, ToolboxItem item,double x,double y)
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

    }
}