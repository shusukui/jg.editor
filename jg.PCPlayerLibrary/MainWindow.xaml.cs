using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using jg.Editor.Library;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

namespace jg.PCPlayerLibrary
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        GridLength m_WidthCache;
        GridLength m_WidthCache2;
        public string TittleName { get; set; }
        public event RoutedPropertyChangedEventHandler<double> _ValueChanged = null;
        public event RoutedEventHandler _PlayStop = null;
        public event RoutedEventHandler _Previous = null;
        public event RoutedEventHandler _Next = null;
        public event RoutedPropertyChangedEventHandler<bool> _PlayPause = null;
        public event RoutedPropertyChangedEventHandler<bool> _ShrinkChanged = null;
        public event RoutedPropertyChangedEventHandler<bool> _MenuVisibleChanged = null;
        public event RoutedPropertyChangedEventHandler<double> _VolumeChanged = null;
        public event RoutedPropertyChangedEventHandler<object> _SelectedItemChanged = null;
        public event MouseEventHandler _MouseEnter = null;
        public event MouseEventHandler _MouseLeave = null;
        private List<SavePageInfo> savePageList = new List<SavePageInfo>();
        public List<PageStage> pageStageList = new List<PageStage>();
        double CurrentVolume = 1;

        double stopMinute = -1; //下一步如果暂停，需要的时间
        bool IsDown = false;

        PageStage pageStage = null;
        bool IsShrink = false; //自动收缩树目录和控制条。
        bool IsExpand = true;//控制条是否显示。
        windowClose windowClose;
        windowTree windowTree;
        windowControl windowControl;
        System.Drawing.Size size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;

        const double timelength = 2000;
        DateTime dt1;

        Point downPoint = new Point(0, 0);
        public MainWindow()
        {
            PageStage pageStage = null;
            InitializeComponent();

            this.SourceInitialized += new EventHandler(win_SourceInitialized);
            PageStage.PageChanged += PageStage_PageChanged;

            savePageList = Globals.savePageList;

            this.Closed += new EventHandler(WindowPreview_Closed);
            ClearSelected(Globals.treeviewSource);
            if (Globals.treeviewSource.Count > 0)
                Globals.treeviewSource[0].IsSelected = true;
            AddPage(Globals.treeviewSource);

            if (pageStageList.Count > 0)
                frame.Navigate(pageStageList[0]);

            PageStage.ScaleChanged += PageStage_ScaleChanged;
        }

        #region 最大化显示任务栏

        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            WinInterop.HwndSource.FromHwnd(handle).AddHook(new WinInterop.HwndSourceHook(WindowProc));
        }

        private static System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }


        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };



        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //播放器的初始化；
            InitPlayer();
        }



        RoutedPropertyChangedEventArgs<object> Olde = null;
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            try
            {
                if (_SelectedItemChanged != null)
                {
                    _SelectedItemChanged(sender, e);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
          
        }

        private void treeView_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_MouseEnter != null)
                _MouseEnter(sender, e);
        }

        private void treeView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_MouseLeave != null)
                _MouseLeave(sender, e);
        }

        private void treeView_Selected(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TreeViewItem).IsExpanded = true;
        }

        void ClearSelected(ObservableCollection<TreeViewItemInfo> list)
        {
            foreach (var v in list)
            {
                v.IsSelected = false;
                ClearSelected(v.Children);
            }
        }
        void AddPage(ObservableCollection<TreeViewItemInfo> list)
        {
            PageStage pageStage = null;
            foreach (var v in list)
            {
                var vv = Globals.savePageList.Find(model => model.PageId == v.Id);
                pageStage = new PageStage((enumStageSwitch)vv.StageSwitch, vv, "Next");
                pageStage.PageLoaded += new RoutedEventHandler(pageStage_PageLoaded);
                pageStageList.Add(pageStage);
                AddPage(v.Children);
            }
        }

        void PageStage_ScaleChanged(object sender, RoutedEventArgs e)
        {
            Shrink(false);
        }

        void PageStage_PageChanged(object sender, RoutedPropertyChangedEventArgs<Guid> e)
        {
            LoadPage(e.NewValue);
        }

        void windowTree__SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItemInfo info = e.NewValue as TreeViewItemInfo;
            if (info == null) return;
            LoadPage(info.Id);
        }
        void windowControl__VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pageStage.Volume = CurrentVolume = e.NewValue;
        }
        void windowControl__MenuVisibleChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
                windowTree.Opacity = 1;
            else
                windowTree.Opacity = 0;
        }
        void windowControl__ShrinkChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            IsShrink = e.NewValue;
            Shrink(!e.NewValue);
        }
        void windowControl__PlayPause(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (pageStage != null) pageStage.Pause(e.NewValue);
        }
        void windowControl__Next(object sender, RoutedEventArgs e)
        {
            Next();
        }
        void windowControl__Previous(object sender, RoutedEventArgs e)
        {
            Previous();
        }
        //时间轴暂时不用，自动跳页功能去掉
        void windowControl__PlayStop(object sender, RoutedEventArgs e)
        {
            //if (pageStage == null) return;
            //if (pageStage.PageItem.AutoNext)
            //{
            //    for (int i = 0; i < pageStageList.Count; i++)
            //    {
            //        if (pageStageList[i].PageId == pageStage.PageId && i < (pageStageList.Count - 1))
            //        {
            //            LoadPage(pageStageList[i + 1].PageId);
            //            return;
            //        }
            //    }
            //}
        }
        //时间轴暂时不用。
        void windowControl__ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            jg.Editor.Library.Control.ControlPlay timer = sender as jg.Editor.Library.Control.ControlPlay;
            Guid key = timer.CurrentItem;
            double _stopMinute;
            if (pageStage == null) return;

            if (e.NewValue == this.stopMinute && this.stopMinute > 0)
            {
                windowControl.Stop();
                this.stopMinute = 0;
                return;
            }

            if ((_stopMinute = pageStage.PlayAnimation(e.NewValue, e.OldValue, key)) > 0)
            {
                this.stopMinute = e.NewValue + _stopMinute;
            }
        }
        void windowControl__MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsExpand && IsShrink)
                Shrink(false);
        }
        void windowControl__MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!IsExpand)
                Shrink(true);
        }
        void controlPlay_VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pageStage.Volume = CurrentVolume = e.NewValue;
        }
        void pageStage_PageLoaded(object sender, RoutedEventArgs e)
        {
            windowControl.Start();
        }
        void WindowPreview_Closed(object sender, EventArgs e)
        {
            foreach (var v in pageStageList)
            {
                foreach (var vv in v.canvas.Children.OfType<ToolboxItem>())
                {
                    switch (vv.AssetType)
                    {
                        case AssetType.Sound:
                            ((MediaPlayer)vv.Content).Close();
                            break;
                        case AssetType.Movie:
                            ((jg.Editor.Library.Control.ControlMediaElement)vv.Content).Close();
                            break;
                    }
                }
                v.Dispose();
            }
            GC.Collect();
            foreach (var v in pageStageList)
                v.Dispose();
        }


        void LoadPage(Guid id)
        {

            PageStage _pageStage = pageStageList.Find(model => model.PageId == id);
            if (pageStage == null)
                _pageStage.Direction = "Next";
            else
                foreach (var v in pageStageList)
                {
                    if (v.PageId == pageStage.PageId)
                    {
                        _pageStage.Direction = "Next";
                        break;
                    }
                    if (v.PageId == _pageStage.PageId)
                    {

                        _pageStage.Direction = "Previous";
                        break;
                    }
                }
            pageStage = _pageStage;
            windowControl.Maximum = pageStage.GetMaxTime();
            windowControl.timeLineItemList = pageStage.GettimeLineItemList();
            windowControl.Minimum = 0;
            windowControl.Value = 0;
            pageStage.Volume = windowControl.Volume;

            frame.Navigate(pageStage);

            SelTree(Globals.treeviewSource.ToList(), id);
        }



        void SelTree(List<TreeViewItemInfo> list, Guid key)
        {
            //windowTree.SelItem(key);
            foreach (var v in list)
            {
                if (v.Id == key)
                {
                    v.IsSelected = true;
                    return;
                }
                else
                    v.IsSelected = false;
                SelTree(v.Children.ToList(), key);
            }

        }
        public void InitPlayer()
        {
            size = new System.Drawing.Size((int)grid.ActualWidth, (int)grid.ActualHeight);



            SetTittle(TittleName);




            windowTree = new windowTree();
            //windowTree.Topmost = true;

            _SelectedItemChanged += windowTree__SelectedItemChanged;
            _MouseEnter += windowControl__MouseEnter;
            _MouseLeave += windowControl__MouseLeave;

            windowControl = new windowControl();
            if (pageStageList.Count > 0)
            {
                LoadPage(pageStageList[0].PageId);
            }

            treeView.ItemsSource = Globals.treeviewSource;
            windowControl.Opacity = 0;
            windowControl.Height = 0;
            windowControl.Width = 0;
            windowControl._ValueChanged += windowControl__ValueChanged;
            windowControl._PlayStop += windowControl__PlayStop;
            windowControl._Previous += windowControl__Previous;
            windowControl._Next += windowControl__Next;
            windowControl._PlayPause += windowControl__PlayPause;
            windowControl._VolumeChanged += windowControl__VolumeChanged;



            controlSound.VolumeChanged += windowControl__VolumeChanged;
            btnPrevious.Click += windowControl__Previous;
            btnNext.Click += windowControl__Next;
            windowControl.Show();
        }

        public void SetTittle(string Name)
        {
            int result = Name.Length;
            this.txtTittle.ToolTip = Name;
            if (result > 11)
            {
                this.txtTittle.Text = Name.Substring(0, 8) + "...";
            }
            else
            {
                this.txtTittle.Text = Name;
            }
        }

        //public int TittleLen(string Name)
        //{
        //    int result = 0;
        //    var NameGroup = Name.ToCharArray();
        //    for (int i = 0; i < NameGroup.Length; i++)
        //    {
        //        if ((int)NameGroup[i] < 299)
        //        {
        //            result = result + 1;
        //        }
        //        else
        //        {
        //            result = result + 2;
        //        }
        //    }
        //    return result;
        //}
        void Previous()
        {
            for (int i = 0; i < pageStageList.Count; i++)
            {
                if (pageStageList[i].PageId == pageStage.PageId && i > 0)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (pageStageList[j].PageItem.IsVisable)
                        {
                            windowTree._SelectedItemChanged -= windowTree__SelectedItemChanged;
                            LoadPage(pageStageList[j].PageId);
                            windowTree._SelectedItemChanged += windowTree__SelectedItemChanged;
                            return;
                        }
                    }

                }
            }
        }
        private void controlPlay_Next(object sender, RoutedEventArgs e)
        {
            Next();
        }


        void Next()
        {
            for (int i = 0; i < pageStageList.Count; i++)
            {
                if (pageStageList[i].PageId == pageStage.PageId && i < (pageStageList.Count - 1))
                {
                    for (int j = i + 1; j < pageStageList.Count; j++)
                    {
                        if (pageStageList[j].PageItem.IsVisable)
                        {

                            LoadPage(pageStageList[j].PageId);

                            return;
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 控制条是否展开
        /// </summary>
        /// <param name="IsExpand"></param>
        void Shrink(bool IsExpand)
        {
            DoubleAnimation doubleAnimation;
            this.IsExpand = IsExpand;
            if (IsExpand)
            {
                doubleAnimation = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                Storyboard.SetTargetName(doubleAnimation, windowTree.Name);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Window.Left"));
                windowTree.BeginAnimation(System.Windows.Window.LeftProperty, doubleAnimation);

                doubleAnimation = new DoubleAnimation(size.Height - 65, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                Storyboard.SetTargetName(doubleAnimation, windowControl.Name);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Window.Top"));
                windowControl.BeginAnimation(System.Windows.Window.TopProperty, doubleAnimation);
            }
            else
            {
                doubleAnimation = new DoubleAnimation(5 - windowTree.Width, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                Storyboard.SetTargetName(doubleAnimation, windowTree.Name);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Window.Left"));
                windowTree.BeginAnimation(System.Windows.Window.LeftProperty, doubleAnimation);

                doubleAnimation = new DoubleAnimation(size.Height - 5, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
                Storyboard.SetTargetName(doubleAnimation, windowControl.Name);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Window.Top"));
                windowControl.BeginAnimation(System.Windows.Window.TopProperty, doubleAnimation);
            }
        }
        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dt1 = DateTime.Now;
                downPoint = e.GetPosition(this);
                IsDown = true;
            }
        }
        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan span = DateTime.Now - dt1;
            IsDown = false;
            //if (!IsExpand)
            //    Shrink(true);
            double moveLength = Math.Abs(downPoint.X - e.GetPosition(this).X);
            double time = span.TotalMilliseconds;

            if (Math.Abs(downPoint.X - e.GetPosition(this).X) > this.ActualWidth / 2)
            {
                if (downPoint.X - e.GetPosition(this).X > 0)
                    Previous(); 
                else
                  Next();  
            }
            else if (time / timelength < moveLength / this.ActualWidth / 2)
            {
                if (downPoint.X - e.GetPosition(this).X > 0)
                    Previous(); 
                else
                    Next();  
            }
            downPoint = new Point(0, 0);
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    Previous();
                    break;
                case Key.Right:
                    Next();
                    break;
            }
        }

        private void Label_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            windowControl.Close();
            this.Close();

        }

        private void tbPlay_Checked(object sender, RoutedEventArgs e)
        {
            windowControl.controlPlay.tbPlay_Checked();
        }

        private void tbPlay_Unchecked(object sender, RoutedEventArgs e)
        {
            windowControl.controlPlay.tbPlay_Unchecked();
        }
        void btnGrdSplitter2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {



        }
        void btnGrdSplitter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void gsSplitterr_Click(object sender, RoutedEventArgs e)
        {
            Button LeftButton = sender as Button;
            if (LeftButton != null)
            {
                LeftGrid.Width = new GridLength(Math.Abs(280 - LeftGrid.Width.Value));

            }
        }

        private void gsSplitterr2_Click(object sender, RoutedEventArgs e)
        {
            Button RightButton = sender as Button;

            if (RightButton != null)
            {

                RightGrid.Width = new GridLength(Math.Abs(85 - RightGrid.Width.Value));
            }
        }





    }
}
