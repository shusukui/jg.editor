using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using jg.Editor.Library;
using System.Windows.Media.Animation;
using System.Windows.Threading;
namespace jg.PCPlayerLibrary
{
    /// <summary>
    /// WindowPreview.xaml 的交互逻辑
    /// </summary>
    public partial class WindowPreview : Window
    {
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
        public WindowPreview()
        {
            PageStage pageStage = null;
            InitializeComponent();

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
        void windowClose__Click(object sender, RoutedEventArgs e)
        {
            windowClose.Close();
            windowTree.Close();
            windowControl.Close();
            this.Close();
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            size = new System.Drawing.Size((int)grid.ActualWidth, (int)grid.ActualHeight);
            windowClose = new windowClose();
            //windowClose.ShowInTaskbar = false;
            windowClose.Top = 0;

            windowClose.Left = size.Width - windowClose.Width;
            windowClose._Click += windowClose__Click;


            windowTree = new windowTree();
            //windowTree.Topmost = true;
            windowTree.ShowInTaskbar = false;
            windowTree.Height = size.Height - 65;
            windowTree.Left = 0;
            windowTree.Top = 0;
            windowTree._SelectedItemChanged += windowTree__SelectedItemChanged;
            windowTree._MouseEnter += windowControl__MouseEnter;
            windowTree._MouseLeave += windowControl__MouseLeave;




            windowControl = new windowControl();
            //windowControl.Topmost = true;
            windowControl.ShowInTaskbar = false;
            windowControl.Width = size.Width;
            windowControl.Left = 0;
            windowControl.Top = size.Height - 65;


            windowClose.Owner = this;
            windowTree.Owner = this;
            windowControl.Owner = this;

            windowClose.Show();
            windowTree.Show();
            windowControl.Show();


            if (pageStageList.Count > 0)
            {
                LoadPage(pageStageList[0].PageId);
            }

            windowTree.treeView.ItemsSource = Globals.treeviewSource;

            windowControl._MouseEnter += windowControl__MouseEnter;
            windowControl._MouseLeave += windowControl__MouseLeave;
            windowControl._ValueChanged += windowControl__ValueChanged;
            windowControl._PlayStop += windowControl__PlayStop;
            windowControl._Previous += windowControl__Previous;
            windowControl._Next += windowControl__Next;
            windowControl._PlayPause += windowControl__PlayPause;
            windowControl._ShrinkChanged += windowControl__ShrinkChanged;
            windowControl._MenuVisibleChanged += windowControl__MenuVisibleChanged;
            windowControl._VolumeChanged += windowControl__VolumeChanged;

        }
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
                    Next();
                else
                    Previous();
            }
            else if (time / timelength < moveLength / this.ActualWidth / 2)
            {
                if (downPoint.X - e.GetPosition(this).X > 0)
                    Next();
                else
                    Previous();
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

    }
}