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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jg.Editor.Library.Control
{
    /// <summary>
    /// PageControl.xaml 的交互逻辑
    /// </summary>
    public partial class TPageControl : UserControl
    {
        #region 自定义属性
        static TPageControl()
        {
            
            LeftWidthProperty = DependencyProperty.Register("LeftWidth", typeof(double), typeof(TPageControl), new FrameworkPropertyMetadata(50.0, LeftWidthChange),
               ShareClass.UnDoubleValueCheck);
            RightWidthProperty = DependencyProperty.Register("RightWidth", typeof(double), typeof(TPageControl), new FrameworkPropertyMetadata(50.0, RightWidthChange),
               ShareClass.UnDoubleValueCheck);
          
         
        }

        //设置左右按钮图片

        //设置左按钮区域宽度
        public static readonly DependencyProperty LeftWidthProperty;
        public double LeftWidth
        {
            get { return (double)GetValue(LeftWidthProperty); }
            set { SetValue(LeftWidthProperty, value); }
        }
        private static void LeftWidthChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TPageControl pageControl = (TPageControl)sender;
            pageControl.LeftGrid.Width = new GridLength(pageControl.LeftWidth);
        }

        //设置右按钮区域宽度
        public static readonly DependencyProperty RightWidthProperty;
        public double RightWidth
        {
            get { return (double)GetValue(RightWidthProperty); }
            set { SetValue(RightWidthProperty, value); }
        }
        private static void RightWidthChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TPageControl pageControl = (TPageControl)sender;
            pageControl.RightGrid.Width = new GridLength(pageControl.RightWidth);
        }
        #endregion

      public  int pageCount = 0;
      public int pageSelect = 0;



     
      public int ImgCount { get; set; }

    
      public double ShowHeight { get; set; }

     
      public double ShowWidth { get; set; }



      public ObservableCollection<AssResInfo> Children { get; set; } 
      public bool isDown = false;
      public double down_pX = 0;
      public double down_pY = 0;
      public bool isMoveSure = false;
      public double oldX = 0;

       public bool isInMove = false;
        object changeLock = new object();
        Storyboard sboard;

        Storyboard sboardLeftIamge, sboardRightIamge;
   
        public TPageControl()
        {
            InitializeComponent();
            sboardLeftIamge = (Storyboard)this.FindResource("StoryboardLeftImage");
            sboardRightIamge = (Storyboard)this.FindResource("StoryboardRightImage");
          

        }
        public double setH
        {
            get { return canvasPageContent.Height; }
            set{ canvasPageContent.Height=value;}
        }

        public double setW
        {
            get { return canvasPageContent.Width; }
            set { canvasPageContent.Width = value; }
        }

        public ObservableCollection<AssResInfo> observable = new ObservableCollection<AssResInfo>();
        /// <summary>
        /// 添加页,为了实现拖拽功能,panel一律不准使用MouseLeftDown来实现触发事件,否则会影响翻页拖拽动作
        /// </summary>
        /// <param name="panels">页(继承Panel即可,无论大小,强制拉升填满页)</param>
        /// <param name="defaultPageNum">添加完后默认页,0:保持在当前页</param>
        public void AddPage(ObservableCollection<Panel> panels, int defaultPageNum)
        {
          
            _AddPage(panels, defaultPageNum);
        }


        public void AddPageVideo(ObservableCollection<Panel> panels, int defaultPageNum)
        {

            _AddPageVideo(panels, defaultPageNum);
        }

        public  void UpdateImgGroup()
        {

            System.Collections.ObjectModel.ObservableCollection<Panel> Panels = new System.Collections.ObjectModel.ObservableCollection<Panel>();

            for (int i = 0; i < this.Children.Count; i++)
            {
                WrapPanel rectangle = new WrapPanel();
                rectangle.Width = this.canvasPageContent.Width;
                rectangle.Height = this.canvasPageContent.Height;
                BitmapImage bitimage = new BitmapImage();
                bitimage.BeginInit();
                bitimage.UriSource = new Uri(this.Children[i].Thumbnails, UriKind.Absolute);
                bitimage.EndInit();

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bitimage;
                rectangle.Background = brush;
                Panels.Add(rectangle);
            }
            this.AddPage(Panels, 1);
        }



        /// <summary>
        /// 获取页宽
        /// </summary>
        /// <returns></returns>
        public double GetPageWidth()
        {
            return canvasPageContent.ActualWidth;
            
        }

        /// <summary>
        /// 获取页高
        /// </summary>
        /// <returns></returns>
        public double GetPageHieght()
        {
            return canvasPageContent.ActualHeight - 2;
        }

        /// <summary>
        /// 清理所有页
        /// </summary>
        public void ClearPage()
        {
            _ClearPage();
        }

        #region 添加页
        private void _ClearPage()
        {
            wrapPanelPages.Children.Clear();
            pageCount = 0;
            pageSelect = 0;
            imageLeft.Visibility = System.Windows.Visibility.Hidden;
            imageRight.Visibility = System.Windows.Visibility.Hidden;
        }



       

        private void _AddPage(ObservableCollection<Panel> panels, int defaultPageNum)
        {
            wrapPanelPages.Children.Clear();
            if (panels.Count > 0)
            {
                //计算总页数
                pageCount = panels.Count;
                //设置wrapPanelPages宽度
                wrapPanelPages.Width = canvasPageContent.ActualWidth * pageCount;
                //加载页
                foreach (Panel panel in panels)
                {
                    //使用viewbox来控制Panel铺满页面
                    Viewbox viewbox = new Viewbox();
                    viewbox.Stretch = Stretch.Fill;
                    viewbox.Width = canvasPageContent.ActualWidth;
                    viewbox.Height = canvasPageContent.ActualHeight;
                    viewbox.Child = panel;
                    wrapPanelPages.Children.Add(viewbox);
                }
                //改变页
                if (defaultPageNum > 0)
                    pageSelect = defaultPageNum;
                if (pageSelect > pageCount)
                    pageSelect = pageCount;
                if (pageSelect == 0)
                    pageSelect = 1;
                Canvas.SetLeft(wrapPanelPages, -canvasPageContent.ActualWidth * (pageSelect - 1));
                //改变按钮状态
                ChangeButtonStatus();
                pageBar1.CreatePageEllipse(pageCount);
                pageBar1.SelectPage(pageSelect);
                pageBar1.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                pageSelect = 0;
            }
        }

        private void _AddPageVideo(ObservableCollection<Panel> panels, int defaultPageNum)
        {
            wrapPanelPages.Children.Clear();
            if (panels.Count > 0)
            {
                //计算总页数
                pageCount += panels.Count;
                //设置wrapPanelPages宽度
                wrapPanelPages.Width = canvasPageContent.Width * pageCount;
                //加载页
                foreach (Panel panel in panels)
                {
                    //使用viewbox来控制Panel铺满页面
                    Viewbox viewbox = new Viewbox();
                    viewbox.Stretch = Stretch.Fill;
                    viewbox.Width = canvasPageContent.Width;
                    viewbox.Height = canvasPageContent.Height;
                    viewbox.Child = panel;
                    wrapPanelPages.Children.Add(viewbox);
                }
                //改变页
                if (defaultPageNum > 0)
                    pageSelect = defaultPageNum;
                if (pageSelect > pageCount)
                    pageSelect = pageCount;
                if (pageSelect == 0)
                    pageSelect = 1;
                Canvas.SetLeft(wrapPanelPages, -canvasPageContent.Width * (pageSelect - 1));
                //改变按钮状态
                ChangeButtonStatus();
                pageBar1.CreatePageEllipse(pageCount);
                pageBar1.SelectPage(pageSelect);
                pageBar1.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                pageSelect = 0;
            }
        }
        #endregion

        #region 翻页实现
        /// <summary>
        /// 翻页实现
        /// </summary>
        /// <param name="page"></param>
        /// <param name="needAnimation"></param>
        private void ChangePage(bool isRight)
        {
            double pageWidth = canvasPageContent.ActualWidth;
            lock (changeLock)
            {
                if (isInMove)
                    return;
                isInMove = true;
            }
            if (isRight)
            {
                if (pageSelect == pageCount)
                {
                    lock (changeLock)
                    {
                        isInMove = false;
                    }
                    return;
                }
                else
                {

                    isInMove = true;
                    double listLeft_now = Canvas.GetLeft(wrapPanelPages);
                    double listLeft_sur = -(pageSelect - 1) * pageWidth;
                    double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                    double toX = -pageWidth;
                    double time = 1000 * Math.Abs(Math.Abs(toX) - Math.Abs(formX)) / pageWidth;
                    sboardRightBegin(formX, toX, time);
                }
            }
            else
            {
                if (pageSelect == 1)
                {
                    lock (changeLock)
                    {
                        isInMove = false;
                    }
                    return;
                }
                else
                {
                    isInMove = true;
                    double listLeft_now = Canvas.GetLeft(wrapPanelPages);
                    double listLeft_sur = -(pageSelect - 1) * pageWidth;
                    //启动左翻动画-翻页
                    double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                    double toX = pageWidth;
                    double time = 1000 * Math.Abs(Math.Abs(toX) - Math.Abs(formX)) / pageWidth;
                    sboardLeftBegin(formX, toX, time);
                }
            }
        }
        #endregion

        #region 改变翻页按钮状态
        /// <summary>
        /// 改变翻页按钮状态
        /// </summary>
        private void ChangeButtonStatus()
        {
            if (pageCount == 0)
            {
                return;
            }
            if (pageSelect == 1 && pageCount == 1)
            {
                imageLeft.Visibility = System.Windows.Visibility.Hidden;
                imageRight.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (pageSelect == 1 && pageCount > 1)
            {
                imageLeft.Visibility = System.Windows.Visibility.Hidden;
                imageRight.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (pageSelect == pageCount)
                {
                    imageLeft.Visibility = System.Windows.Visibility.Visible;
                    imageRight.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    imageLeft.Visibility = System.Windows.Visibility.Visible;
                    imageRight.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        #endregion

        #region 拖拽实现

        private void canvasPageContent_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDown = true;
            System.Windows.Point position = e.GetPosition(canvasPageContent);
            down_pX = position.X;
            down_pY = position.Y;
            oldX = down_pX;
        }

        private void canvasPageContent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDown)
            {
                e.Handled = isMoveSure;
                changePos();
            }
        }

        private void canvasPageContent_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                System.Windows.Point position = e.GetPosition(canvasPageContent);
                Canvas.SetLeft(wrapPanelPages, Canvas.GetLeft(wrapPanelPages) + (position.X - oldX));
                oldX = position.X;

                if (Math.Abs(down_pX - position.X) > 150)
                {
                    isMoveSure = true;
                }
            }
        }

        private void canvasPageContent_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                changePos();
            }
        }

        private void changePos()
        {
            double pageWidth = canvasPageContent.ActualWidth;
            isDown = false;
            isMoveSure = false;
            double listLeft_now = Canvas.GetLeft(wrapPanelPages);
            double listLeft_sur = -(pageSelect - 1) * pageWidth;
            //右翻页动作
            if (listLeft_now < listLeft_sur)
            {
                if (pageSelect == pageCount)
                {
                    //已经达到最大页面-回滚
                    isInMove = true;
                    double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                    double toX = 0;
                    double time = 1000 * Math.Abs(formX) / pageWidth;
                    Canvas.SetLeft(wrapPanelPages, listLeft_sur);
                    sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
                    sboard.Completed += sboardNoChange_Completed;
                    sboard.Begin();
                }
                else
                {
                    bool SureRight = false;
                    //移动距离
                    double dis = Math.Abs(listLeft_now - listLeft_sur);
                    //达到翻页确认标准
                    if (dis >= 150)
                        SureRight = true;
                    if (SureRight)
                    {
                        //启动右翻动画-翻页
                        isInMove = true;
                        double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                        double toX = -pageWidth;
                        double time = 1000 * Math.Abs(Math.Abs(toX) - Math.Abs(formX)) / pageWidth;
                        Canvas.SetLeft(wrapPanelPages, listLeft_sur);
                        sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
                        sboard.Completed += sboardRight_Completed;
                        sboard.Begin();
                    }
                    else
                    {
                        //未达到翻页要求-回滚
                        isInMove = true;
                        double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                        double toX = 0;
                        double time = 1000 * Math.Abs(formX) / pageWidth;
                        Canvas.SetLeft(wrapPanelPages, listLeft_sur);
                        sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
                        sboard.Completed += sboardNoChange_Completed;
                        sboard.Begin();
                    }
                }
            }
            //左翻页确认
            else
            {
                //第一页左翻-回滚
                if (pageSelect == 1)
                {
                    isInMove = true;
                    double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                    double toX = 0;
                    double time = 1000 * Math.Abs(formX) / pageWidth;
                    Canvas.SetLeft(wrapPanelPages, listLeft_sur);
                    sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
                    sboard.Completed += sboardNoChange_Completed;
                    sboard.Begin();
                }
                else
                {
                    //确认翻页参数
                    bool SureLeft = false;
                    //移动距离
                    double dis = Math.Abs(listLeft_now - listLeft_sur);
                    //达到翻页确认标准
                    if (dis >= 150)
                        SureLeft = true;
                    if (SureLeft)
                    {
                        isInMove = true;
                        //启动左翻动画-翻页
                        double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                        double toX = pageWidth;
                        double time = 1000 * Math.Abs(Math.Abs(toX) - Math.Abs(formX)) / pageWidth;
                        Canvas.SetLeft(wrapPanelPages, listLeft_sur);
                        sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
                        sboard.Completed += sboardLeft_Completed;
                        sboard.Begin();
                    }
                    else
                    {
                        isInMove = true;
                        //未达到翻页要求-回滚
                        double formX = listLeft_now + (pageSelect - 1) * pageWidth;
                        double toX = 0;
                        double time = 1000 * Math.Abs(formX) / pageWidth;
                        Canvas.SetLeft(wrapPanelPages, listLeft_sur);
                        sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
                        sboard.Completed += sboardNoChange_Completed;
                        sboard.Begin();
                    }
                }
            }
        }
        #endregion

        #region 翻页按钮

        private void imageLeft_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageLeft.Effect = new DropShadowEffect();
        }

        private void imageLeft_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageLeft.Effect = null;
        }

        private void imageLeft_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //翻页
            sboardLeftIamge.Begin();
            ChangePage(false);
        }

        private void imageRight_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageRight.Effect = new DropShadowEffect();
        }

        private void imageRight_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageRight.Effect = null;
        }

        private void imageRight_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //翻页
            sboardRightIamge.Begin();
            ChangePage(true);
        }
        #endregion

        #region 动画处理

        //左翻动画
        private void sboardLeftBegin(double formX, double toX, double time)
        {
            sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
            sboard.Completed += sboardLeft_Completed;
            sboard.Begin();
        }

        //右翻动画
        private void sboardRightBegin(double formX, double toX, double time)
        {
            sboard = ShareClass.CeaterAnimation_Xmove(wrapPanelPages, formX, toX, time, 0);
            sboard.Completed += sboardRight_Completed;
            sboard.Begin();
        }

        //翻页回滚结束
        private void sboardNoChange_Completed(object sender, EventArgs e)
        {
            lock (changeLock)
            {
                isInMove = false;
            }
        }

        //右翻页结束
        private void sboardRight_Completed(object sender, EventArgs e)
        {
            pageSelect++;
            pageBar1.SelectPage(pageSelect);
            sboard.Stop();
            ChangeButtonStatus();
            Canvas.SetLeft(wrapPanelPages, -(pageSelect - 1) * canvasPageContent.ActualWidth);
            lock (changeLock)
            {
                isInMove = false;
            }
        }

        //左翻页结束
        private void sboardLeft_Completed(object sender, EventArgs e)
        {
            pageSelect--;
            pageBar1.SelectPage(pageSelect);
            sboard.Stop();
            ChangeButtonStatus();
            Canvas.SetLeft(wrapPanelPages, -(pageSelect - 1) * canvasPageContent.ActualWidth);
            lock (changeLock)
            {
                isInMove = false;
            }
        }
        #endregion


        /// <summary>
        /// 当画布改变时，动态更新更新自己的画面内容
        /// </summary>
        public void SizeUpdateThisGroup()
        {
            wrapPanelPages.Width = canvasPageContent.ActualWidth * pageCount;
            foreach (var v in wrapPanelPages.Children)
            {
                Viewbox viewbox = v as Viewbox;
                viewbox.Stretch = Stretch.Fill;
                viewbox.Width = canvasPageContent.ActualWidth;
                viewbox.Height = canvasPageContent.ActualHeight;
            }
        }

        public void SizeUpdateThisGroupNoActual()
        {
            wrapPanelPages.Width = canvasPageContent.Width * pageCount;
            foreach (var v in wrapPanelPages.Children)
            {
                Viewbox viewbox = v as Viewbox;
                viewbox.Stretch = Stretch.Fill;
                viewbox.Width = canvasPageContent.Width;
                viewbox.Height = canvasPageContent.Width;
            }
        }
        #region 页面大小变化
        //页面大小变化，修改canvasPageContent的裁剪范围
        private void canvasPageContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvasPageRectangle.Rect = new Rect(0, 0, canvasPageContent.ActualWidth, canvasPageContent.ActualHeight);
            SizeUpdateThisGroup();
        }
        #endregion

    }
}
