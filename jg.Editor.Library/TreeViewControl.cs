
namespace jg.Editor.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Runtime.InteropServices;
    using System.Windows.Documents;
    using System.Windows.Input;

    using System.Xml.Serialization;
    [Serializable]
    public class TreeViewControl : TreeView
    {
        // 树节点变更事件
        //public delegate void OnUserItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e);

        //public event OnUserItemsChanged ItemsChanged = null;
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler SourceItemChanged = null;
        bool MouseButtonDown = false;
        Point PushPoint = new Point(0, 0);
        double height, width;
        AdornerLayer mAdornerLayer = null;
        DragDropAdorner adorner = null;

        private ObservableCollection<TreeViewItemInfo> source = new ObservableCollection<TreeViewItemInfo>();

        public ObservableCollection<TreeViewItemInfo> Source
        {
            get { return source; }
            set { source = value; this.ItemsSource = value; }
        }

        private TreeViewItem SelItem;

        private ContextMenu contextMenu = new ContextMenu();

        public TreeViewControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            MenuItem[] menuItem = new MenuItem[] { 
                new MenuItem() { Header = FindResource("FF00002E") ,Tag ="Add"},
                new MenuItem() { Header = FindResource("FF00002F") ,Tag="AddSub"}, 
                new MenuItem() { Header = FindResource("FF000030") ,Tag="Edit"} ,
                new MenuItem() { Header = FindResource("FF000031") ,Tag="Del"} 
            };
            
            foreach (var v in menuItem)
                v.Click += TreeViewControl_Click;

            contextMenu.ItemsSource = menuItem;
            source.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(source_CollectionChanged);
        }

        void source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SourceItemChanged != null) SourceItemChanged(sender, e);
        }

        void TreeViewControl_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item == null) return;
            if (SelItem == null) return;

            TreeViewItemInfo info = (TreeViewItemInfo)SelItem.Header;
            TreeViewItemInfo addinfo;
            TreeViewItemInfo parentInfo;
            switch (item.Tag.ToString())
            {
                case "Add":
                    addinfo = new TreeViewItemInfo();
                    addinfo.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(source_CollectionChanged);
                    addinfo.ParentId = info.ParentId;
                    parentInfo = GetParentInfo(info, source);
                    if (parentInfo == null)
                    {
                        source.Add(addinfo);
                    }
                    else
                    {
                        parentInfo.Children.Add(addinfo);
                    }
                    addinfo.IsEdit = true;
                    break;
                case "AddSub":
                    addinfo = new TreeViewItemInfo();
                    addinfo.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(source_CollectionChanged);
                    addinfo.ParentId = info.Id;
                    info.Children.Add(addinfo);
                    addinfo.IsEdit = true;
                    break;
                case "Edit":
                    //info.IsEdit = true;
                    //break;
                case "Del":
                    break;
            }
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);

            
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);

            TreeViewItem item = Common.FindVisualParent<TreeViewItem>((DependencyObject)this.InputHitTest(e.GetPosition(null)));
            if (item == null) return;
            contextMenu.IsOpen = true;
        }
        
        //protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        //{
        //    base.OnPreviewMouseMove(e);

        //    if (Mouse.LeftButton != MouseButtonState.Pressed)
        //        return;
        //    if (SelItem == null) return;

        //    adorner = new DragDropAdorner(SelItem, height, width);

        //    mAdornerLayer = AdornerLayer.GetAdornerLayer(this);

        //    if (MouseButtonDown && PushPoint != e.GetPosition(null))
        //    {
        //        mAdornerLayer.Add(adorner);
        //        MouseButtonDown = false;
        //    }
        //    else
        //    {
        //        mAdornerLayer.Update();
        //    }
        //}

        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock tb = Common.FindVisualParent<TextBlock>((DependencyObject)InputHitTest(e.GetPosition(null))) as TextBlock;
            if (tb == null) return;

            height = tb.ActualHeight;
            width = tb.ActualWidth;

            SelItem = Common.FindVisualParent<TreeViewItem>((DependencyObject)InputHitTest(e.GetPosition(null))) as TreeViewItem;

            if (SelItem != null)
            {
                PushPoint = e.GetPosition(null);
                MouseButtonDown = true;
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        //protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    TreeViewItem tvItem = Common.FindVisualParent<TreeViewItem>((DependencyObject)InputHitTest(e.GetPosition(null))) as TreeViewItem;

        //    if (adorner != null)
        //        mAdornerLayer.Remove(adorner);
        //    if (SelItem == null) return;
        //    if (!(SelItem.Header is TreeViewItemInfo)) return;
        //    if (SelItem != null && tvItem != null)
        //    {
        //        if (SelItem == tvItem)
        //        {
        //            SelItem = null;
        //            return;
        //        }
        //    }

        //    if (SelItem != null && tvItem != null && ((TreeViewItemInfo)SelItem.Header).Id != ((TreeViewItemInfo)tvItem.Header).Id)
        //    {
        //        MoveTreeViewItem((TreeViewItemInfo)SelItem.Header, (TreeViewItemInfo)tvItem.Header);
        //    }
        //    else if (SelItem != null && tvItem == null)
        //    {
        //        MoveTreeViewItem((TreeViewItemInfo)SelItem.Header, null);
        //    }

        //    base.OnPreviewMouseLeftButtonUp(e);
        //}

        void MoveTreeViewItem(TreeViewItemInfo SourceInfo, TreeViewItemInfo TargetInfo)
        {
            TreeViewItemInfo sourceParent = GetParentInfo(SourceInfo, Source);
            if (sourceParent != null)
            {
                sourceParent.Children.Remove(SourceInfo);
                if (TargetInfo != null)
                {
                    TargetInfo.Children.Add(SourceInfo);
                    SourceInfo.ParentId = TargetInfo.Id;
                }
                else
                {
                    Source.Add(SourceInfo);
                    SourceInfo.ParentId = new Guid();
                }
            }
            else
            {
                Source.Remove(SourceInfo);
                if (TargetInfo != null)
                {
                    TargetInfo.Children.Add(SourceInfo);
                    SourceInfo.ParentId = TargetInfo.Id;
                }
                else
                {
                    Source.Add(SourceInfo);
                    SourceInfo.ParentId = new Guid();
                }
            }
        }

        public int GetItemIndex(TreeViewItemInfo info)
        {
            int index = 0;
            bool state = GetItemIndex(Source, info, ref index);
            return index;
        }

        private bool GetItemIndex(ObservableCollection<TreeViewItemInfo> list, TreeViewItemInfo info, ref int index)
        {
            bool state = false;

            foreach (TreeViewItemInfo _info in list)
            {
                index++;
                if (_info.Id == info.Id) { state = true; break; }

                state = GetItemIndex(_info.Children, info, ref index);
                if (state) break;
            }

            return state;
        }

        TreeViewItemInfo GetParentInfo(TreeViewItemInfo info, ObservableCollection<TreeViewItemInfo> list)
        {
            TreeViewItemInfo model = null;
            foreach (var v in list)
            {
                if (v.Id == info.ParentId)
                {
                    return v;
                }
                else
                {
                    model = GetParentInfo(info, v.Children);
                }
            }
            return model;
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            //if (ItemsChanged != null) ItemsChanged(e);           
        }

    }

    [Serializable]
    public class TreeViewItemInfo : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        public TreeViewItemInfo()
        {
            _children = new ObservableCollection<TreeViewItemInfo>();
            Children.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Children_CollectionChanged);
        }


        private System.Windows.Visibility _WpfVisibility = System.Windows.Visibility.Visible;

        /// <summary>
        /// wpf的显示与隐藏
        /// </summary>
        [XmlAttribute("WpfVisibility")]
        public System.Windows.Visibility WpfVisibility
        {
            get { return _WpfVisibility; }
            set { 
                _WpfVisibility = value;
                if (value == Visibility.Visible)
                {
                    Height = 20;
                }
                else
                {
                    Height = 0;
                }
            }
        }

       
        private Guid id = System.Guid.NewGuid();
        [XmlAttribute("Id")]
        public Guid Id { get { return id; } set { id = value; } }

        private string title = "";
        [XmlAttribute("Title")]
        public string Title { get { return title; } set { title = value; OnPropertyChanged("Title"); } }

        private double height = 10;
        [XmlAttribute("Height")]
        public double Height { get { return height; } set { height = value; OnPropertyChanged("Height"); } }

        private Guid parentid;
        [XmlAttribute("ParentId")]
        public Guid ParentId
        {
            get { return parentid; }
            set { parentid = value; }
        }

        private ObservableCollection<TreeViewItemInfo> _children;
        [XmlElement("Children")]
        public ObservableCollection<TreeViewItemInfo> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null) CollectionChanged(sender, e);
        }
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isselected;
        [XmlAttribute("IsSelected")]
        public bool IsSelected
        {
            get { return _isselected; }
            set { _isselected = value; OnPropertyChanged("IsSelected"); }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool isedit = false;
        [XmlAttribute("IsEdit")]
        public bool IsEdit
        {
            get { return isedit; }
            set
            {
                isedit = value;
                OnPropertyChanged("IsEdit");
            }
        }

        private int _Sort = 0;
         [XmlAttribute("Sort")]
        public int Sort
        {
            get { return _Sort; }
            set
            {
                _Sort = value;
                OnPropertyChanged("Sort");
            }
        }
        private TreeViewItemInfo _ClassInfoBind;

        public TreeViewItemInfo ClassInfoBind
        {
            get { return this; }
        }

        //textBox的显示隐藏操作
        private System.Windows.Visibility _isTxtVisabled = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility IsTxtVisabled
        {
            get { return _isTxtVisabled; }
            set { _isTxtVisabled = value; OnPropertyChanged("IsTxtVisabled"); }
        }

        //textBolck的显示隐藏操作
        private System.Windows.Visibility _isTbVisabled = System.Windows.Visibility.Visible;
        public System.Windows.Visibility IsTbVisabled
        {
            get { return _isTbVisabled; }
            set { _isTbVisabled = value; OnPropertyChanged("IsTbVisabled"); }
        }


        //上一级横条
        private System.Windows.Visibility  _isUpVis = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility isUpVis
        {
            get { return _isUpVis; }
            set { _isUpVis = value; OnPropertyChanged("isUpVis"); }
        }

        //下一级横条
        private System.Windows.Visibility _isDownVis = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility isDownVis
        {
            get { return _isDownVis; }
            set { _isDownVis = value; OnPropertyChanged("isDownVis"); }
        }
    }

    public class DragDropAdorner : Adorner
    {
        double height, width;
        public DragDropAdorner(UIElement parent, double height, double width)
            : base(parent)
        {

            IsHitTestVisible = false; // Seems Adorner is hit test visible?
            mDraggedElement = parent as FrameworkElement;
           
            this.height = height;
            this.width = width;
           
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (mDraggedElement != null)
            {
                Win32.POINT screenPos = new Win32.POINT();
                if (Win32.GetCursorPos(ref screenPos))
                {
                    Point pos = PointFromScreen(new Point(screenPos.X + 5, screenPos.Y + 5));
                    Rect rect = new Rect(pos.X, pos.Y, width * 2.5, height);
                    drawingContext.PushOpacity(1.0);

                    // if (highlight != null)
                    //    drawingContext.DrawRectangle(highlight, new Pen(Brushes.Transparent, 0), rect);

                    drawingContext.DrawRectangle(new VisualBrush(mDraggedElement), new Pen(Brushes.Transparent, 0), rect);

                    drawingContext.Pop();
                }
            }
        }

        FrameworkElement mDraggedElement = null;
    }

    public static class Win32
    {
        public struct POINT { public Int32 X; public Int32 Y; }

        // During drag-and-drop operations, the position of the mouse cannot be 
        // reliably determined through GetPosition. This is because control of 
        // the mouse (possibly including capture) is held by the originating 
        // element of the drag until the drop is completed, with much of the 
        // behavior controlled by underlying Win32 calls. As a workaround, you 
        // might need to use Win32 externals such as GetCursorPos.
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT point);
    }

    
}