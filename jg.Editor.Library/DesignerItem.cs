
namespace jg.Editor.Library
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.ComponentModel;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Animation;
    public class DesignerItem : ContentControl, INotifyPropertyChanged
    {
        public Guid ItemId
        {
            get
            {
                if (this.Content == null) return new Guid();
                return ((ToolboxItem)this.Content).ItemId;
            }
        }


        private double _AngledX;

        public double AngledX
        {
            get { return _AngledX; }
            set { _AngledX = value; }
        }

        private double _AngledY;

        public double AngledY
        {
            get { return _AngledY; }
            set { _AngledY = value; }
        }


        private double _initialAngle;

        /// <summary>
        /// 初始化旋转角度
        /// </summary>
        public double InitialAngle
        {
            get { return _initialAngle; }
            set { _initialAngle = value; }
        }

        private double _LineHeight;
        /// <summary>
        /// 行高
        /// </summary>
        public double LineHeight
        {
            get { return ((ToolboxItem)this.Content).LineHeight; }
            set { ((ToolboxItem)this.Content).LineHeight = value; }
        }

        private RotateTransform _rotateTransform;

        /// <summary>
        /// 初始旋转类
        /// </summary>
        public RotateTransform RotateTransform
        {
            get { return _rotateTransform; }
            set { _rotateTransform = value; }
        }
        private Vector _startVector;

        /// <summary>
        /// 初始Vector
        /// </summary>
        public Vector StartVector
        {
            get { return _startVector; }
            set { _startVector = value; }
        }
        private Point _centerPoint;

        /// <summary>
        /// 中心点
        /// </summary>
        public Point centerPoint
        {
            get { return  this.TranslatePoint(
                       new Point(this.ActualWidth / 2,
                                 this.ActualHeight / 2),
                                 this.canvas); 
            }
            set { _centerPoint = value; }
        }

        private Canvas _canvas;

        /// <summary>
        /// 父层画布
        /// </summary>
        public Canvas canvas
        {
            get { return _canvas; }
            set { _canvas = value; }
        }

   
        private ContextMenu contextMenu;

        public ContextMenu _ContextMenu
        {
            get { return contextMenu; }
            set { contextMenu = value; }
        }

        public string ItemName
        {
            get { return ((ToolboxItem)this.Content).ItemName; }
            set { ((ToolboxItem)this.Content).ItemName = value; }
        }

        public bool IsLongText
        {
            get { return ((ToolboxItem)this.Content).IsLongText; }
            set { ((ToolboxItem)this.Content).IsLongText = value; }
        }
        private bool isShowDiv;

        /// <summary>
        /// 是否弹出层
        /// </summary>
        public bool IsShowDiv
        {
            get { return ((ToolboxItem)this.Content).IsShowDiv; }
            set { ((ToolboxItem)this.Content).IsShowDiv = value; }
        }
        public bool IsDescPt
        {
            get { return ((ToolboxItem)this.Content).IsDescPt; }
            set { ((ToolboxItem)this.Content).IsDescPt = value; }
        }
        private Button btnOk = null;
        public delegate void OnItemDragComplete(object sender, double width, double height, double left, double top, double oldwidth, double oldheight, double oldleft, double oldtop);

        public delegate void OnSelectedChanged(object sender, Guid ItemId, bool IsSelected);
        public event OnSelectedChanged SelectedChanged = null;

        public event OnItemDragComplete ItemDragComplete = null;

        public event PropertyChangedEventHandler PropertyChanged = null;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                this.OnPropertyChanged("IsSelected");
                if (SelectedChanged != null) SelectedChanged(this, ItemId, value);

            }
        }

        public bool IsLock = false;

        int AssetActionCount = 0;

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(DesignerItem),
                                      new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MoveThumbTemplateProperty = DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        private TimeLineItemInfo timelineiteminfo = null;

        /// <summary>
        /// 时间轴属性。
        /// </summary>
        public TimeLineItemInfo timeLineItemInfo
        {
            get { return timelineiteminfo; }
            set
            {
                timelineiteminfo = value;

                if (timelineiteminfo.IsVisible)
                    this.Visibility = System.Windows.Visibility.Visible;
                else
                    this.Visibility = System.Windows.Visibility.Hidden;
                IsLock = timelineiteminfo.IsLock;

                timelineiteminfo.PropertyChanged += timelineiteminfo_PropertyChanged;

            }
        }
        void timelineiteminfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsVisible":
                    if (timelineiteminfo.IsVisible)
                        this.Visibility = System.Windows.Visibility.Visible;
                    else
                        this.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "IsLock":
                    IsLock = timelineiteminfo.IsLock;
                    break;
            }
        }

        private AssetActionInfo _assetactioninfo = new AssetActionInfo();

        /// <summary>
        /// 素材动作。
        /// </summary>
        public AssetActionInfo assetActionInfo
        {
            get { return _assetactioninfo; }
            set { _assetactioninfo = value; }
        }

        public static ControlTemplate GetMoveThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
        }

        public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(MoveThumbTemplateProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.btnOk = this.FindVisualChild<Button>(this);

            if (this.btnOk.Name.ToLower() == "btnok")
            {
                this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.MoveThumbVisible = Visibility.Visible;
            if (this.btnOk != null)
            {
                this.btnOk.Visibility = Visibility.Hidden;
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if ((child != null) && (child is childItem))
                {
                    return (childItem)child;
                }
                childItem childOfChild = this.FindVisualChild<childItem>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return default(childItem);
        }

        static DesignerItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem()
        {
            this.Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);

            this.Background = Brushes.Transparent;


        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    this.IsSelected = !this.IsSelected;
                }
                else
                {
                    if (!this.IsSelected)
                    {
                        designer.DeselectAll();
                        this.IsSelected = true;
                    }
                    if (this.IsSelected)
                    {
                        DesignerCanvas canvas = this.Parent as DesignerCanvas;
                        this.IsSelected = true;
                        if (canvas != null) canvas.Focus();
                    }
                }
            }

            e.Handled = false;
        }



        private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Template != null)
            {
                ContentPresenter contentPresenter = this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;

                MoveThumb thumb = this.Template.FindName("PART_MoveThumb", this) as MoveThumb;

                if (contentPresenter != null && thumb != null)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;

                    if (contentVisual != null)
                    {
                        ControlTemplate template = DesignerItem.GetMoveThumbTemplate(contentVisual) as ControlTemplate;

                        if (template != null)
                        {
                            thumb.Template = template;
                        }
                    }
                }
            }

            InitMeun();

        }

        public void InitMeun()
        {
            MenuItem menuItem1 = new MenuItem();
            menuItem1.Header = "组合";
            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "分离";
            contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem1);
            contextMenu.Items.Add(menuItem2);
            this.ContextMenu = contextMenu;
        }
        public void SetItemDragComplete(double oldwidth, double oldheight, double oldleft, double oldtop)
        {
            if (this.ItemDragComplete != null)
                this.ItemDragComplete(this, base.ActualWidth, base.ActualHeight, Canvas.GetLeft(this), Canvas.GetTop(this), oldwidth, oldheight, oldleft, oldtop);
        }

        private Visibility movethumbvisible = Visibility.Visible;

        public Visibility MoveThumbVisible
        {
            get
            {
                return this.movethumbvisible;
            }
            set
            {
                this.movethumbvisible = value;
                this.OnPropertyChanged("MoveThumbVisible");
            }
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDoubleClick(e);
            this.MoveThumbVisible = Visibility.Hidden;
            if (this.btnOk != null)
            {
                this.btnOk.Visibility = Visibility.Visible;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
            if (AssetActionCount % 2 == 0)
                AssetAction(enumAssetEvent.MouseClick);
            else
                UnAssetAction(enumAssetEvent.MouseClick);
            AssetActionCount++;

        }

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            AssetAction(enumAssetEvent.MouseEnter);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UnAssetAction(enumAssetEvent.MouseEnter);
        }

        void AssetAction(enumAssetEvent assetEvent)
        {
            DesignerCanvas canvas = base.Parent as DesignerCanvas;
            if (canvas == null) return;
            if (assetActionInfo == null) return;
            if (string.IsNullOrEmpty(assetActionInfo.AssetName)) return;
            var selectedItems = canvas.Children.OfType<DesignerItem>().FirstOrDefault(model => model.ItemName == assetActionInfo.AssetName);
            if (selectedItems == null) return;
            if (assetActionInfo.AssetEvent != assetEvent) return;
            AssetAction(assetActionInfo, selectedItems);
        }

        void UnAssetAction(enumAssetEvent assetEvent)
        {
            DesignerCanvas canvas = base.Parent as DesignerCanvas;
            if (canvas == null) return;
            if (assetActionInfo == null) return;
            if (string.IsNullOrEmpty(assetActionInfo.AssetName)) return;
            var selectedItems = canvas.Children.OfType<DesignerItem>().FirstOrDefault(model => model.ItemName == assetActionInfo.AssetName);
            if (selectedItems == null) return;
            if (assetActionInfo.AssetEvent != assetEvent) return;
            UnAssetAction(assetActionInfo, selectedItems);
        }

        void AssetAction(AssetActionInfo assetAction, DesignerItem item)
        {
            DesignerCanvas canvas = item.Parent as DesignerCanvas;
            if (canvas == null) return;
            if (item == null) return;
            switch (assetAction.AssetAction)
            {
                case enumAssetAction.Fade:
                    AssetActionFade(assetAction, item, 0, 1);
                    break;
                case enumAssetAction.Left:
                    AssetActionMove(assetAction, item, Canvas.GetLeft(item), Canvas.GetLeft(item) + canvas.ActualWidth);
                    break;
                case enumAssetAction.Right:
                    AssetActionMove(assetAction, item, Canvas.GetLeft(item), Canvas.GetLeft(item) - canvas.ActualWidth);

                    break;
            }
        }

        void UnAssetAction(AssetActionInfo assetAction, DesignerItem item)
        {
            DesignerCanvas canvas = item.Parent as DesignerCanvas;
            if (canvas == null) return;
            if (item == null) return;
            switch (assetAction.AssetAction)
            {
                case enumAssetAction.Fade:
                    AssetActionFade(assetAction, item, 1, 0);
                    break;
                case enumAssetAction.Left:
                    AssetActionMove(assetAction, item, Canvas.GetLeft(item) + canvas.ActualWidth, Canvas.GetLeft(item));
                    break;
                case enumAssetAction.Right:
                    AssetActionMove(assetAction, item, Canvas.GetLeft(item), Canvas.GetLeft(item) - canvas.ActualWidth);

                    break;
            }
        }

        TimeSpan GetSpan(double Second)
        {
            TimeSpan span;
            int second = (int)Math.Truncate(Second);
            int Milliseconds = (int)((Second - second) * 1000);
            span = new TimeSpan(0, 0, 0, second, Milliseconds);
            return span;
        }


        //推入动画
        void AssetActionPush(AssetActionInfo assetAction, DesignerItem item, double fromValue, double toValue)
        {

            DoubleAnimation doubleAnimation = new DoubleAnimation(fromValue, toValue, new Duration(GetSpan(assetAction.Time)));
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Completed += (sender, e) =>
            {
                item.BeginAnimation(DesignerItem.OpacityProperty, null);
            };
            item.BeginAnimation(DesignerItem.MarginProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);

        }
        //淡入淡出
        void AssetActionFade(AssetActionInfo assetAction, DesignerItem item, double fromValue, double toValue)
        {

            DoubleAnimation doubleAnimation = new DoubleAnimation(fromValue, toValue, new Duration(GetSpan(assetAction.Time)));
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Completed += (sender, e) =>
                {
                    item.BeginAnimation(DesignerItem.OpacityProperty, null);
                };
            item.BeginAnimation(DesignerItem.OpacityProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);

        }

        //左侧飞入
        void AssetActionMove(AssetActionInfo assetAction, DesignerItem item, double fromValue, double toValue)
        {

            DoubleAnimation doubleAnimation = new DoubleAnimation(fromValue, toValue, new Duration(GetSpan(assetAction.Time)));

            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
            doubleAnimation.Completed += (sender, e) =>
            {
                item.BeginAnimation(Canvas.LeftProperty, null);
            };

            item.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
        }
    }
}