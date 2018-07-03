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
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace jg.Editor.Library.Control
{
    /// <summary>
    /// ControlPlay.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPlay : UserControl
    {
        DispatcherTimer SliderTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 3) };
        DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 100) };
        DateTime dt1 = DateTime.Now, dt2 = DateTime.Now;


        public delegate void delChanged();
        public event delChanged EventHandler = null;
        public event RoutedPropertyChangedEventHandler<double> ValueChanged = null;
        private delegate void ActionLine();
        public event RoutedEventHandler PlayStop = null;
        public event RoutedPropertyChangedEventHandler<bool> PlayPause = null;
        public event RoutedEventHandler Previous = null;
        public event RoutedEventHandler Next = null;
        public event RoutedPropertyChangedEventHandler<bool> ShrinkChanged = null; //自动显示控制条。
        public event RoutedPropertyChangedEventHandler<bool> MenuVisibleChanged = null;//是否显示菜单栏。

        public event RoutedPropertyChangedEventHandler<double> VolumeChanged = null;

        public double Volume { get { return controlSound.Volume; } }

        private double _maximum = 100;
        /// <summary>
        /// 最大值
        /// </summary>
        public double Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                slider.Maximum = value;
                progressBar.Maximum = value;
            }
        }

        public List<TimeLineItemInfo> timeLineItemList = new List<TimeLineItemInfo>();
        private double _minimum = 0;
        /// <summary>
        /// 最小值
        /// </summary>
        public double Minimum
        {
            get { return 0; }
            set
            {
                _minimum = 0;
                slider.Minimum = 0;
                progressBar.Minimum = 0;
            }
        }

        private double _value = 0;
        /// <summary>
        /// 当前值
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                slider.Value = value;
                progressBar.Value = value;
                NewValue = value;
            }
        }

        private double NewValue;
        private int playItems = 0;
        public Guid CurrentItem;
        public ControlPlay()
        {

            InitializeComponent();

            //播放器禁用时间轴，这块先注释，等时间轴用起来再说吧，(￣ . ￣)  。
            dispatcherTimer.Tick += (s, e) =>
            {
                DispatcherTimer timer = s as DispatcherTimer;
                TimeLineItemInfo timeLineItem = timer.Tag as TimeLineItemInfo;
                CurrentItem = timeLineItem.Id;

                TimeSpan span = dt1 - dt2;

                if (ValueChanged != null)
                    ValueChanged(this, new RoutedPropertyChangedEventArgs<double>(Value, NewValue));
                Value =  NewValue;
                NewValue = Math.Round(NewValue + 0.1, 1);
                if (Value >= GetMaxTime(timeLineItem.Id))
                {
                    tbPlay.IsChecked = false;
                    if (PlayStop != null)
                    {
                        dispatcherTimer.Stop();
                        //PlayStop(this, new RoutedEventArgs());
                        NewValue = 0;
                        playItems++;
                        if (playItems < timeLineItemList.Count)
                        {
                            SetActionActionLineArray(playItems);
                        }
                        else
                        {
                            CurrentItem = new Guid();
                        }
                    }

                }
                int interval = 100 - Convert.ToInt32(span.TotalMilliseconds - 100);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval <= 0 ? 100 : interval);

                dt2 = dt1;
                dt1 = DateTime.Now;

            };
            slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slider_ValueChanged);
            controlSound.VolumeChanged += controlSound_VolumeChanged;
            if (DesignerProperties.GetIsInDesignMode(this))
                dispatcherTimer.Start();
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



        void controlSound_VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.VolumeChanged != null)
                VolumeChanged(this, new RoutedPropertyChangedEventArgs<double>(e.OldValue, e.NewValue));
        }
        void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Value = Math.Round(e.NewValue, 1);
        }
        private void progressBar_MouseEnter(object sender, MouseEventArgs e)
        {
            slider.Visibility = System.Windows.Visibility.Visible;
            SliderTimer.Stop();
        }
        private void progressBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SliderHidden();
        }
        void SliderHidden()
        {
            SliderTimer.Tick += (s, ea) => { slider.Visibility = System.Windows.Visibility.Hidden; SliderTimer.Stop(); };
            SliderTimer.Start();
        }
        private void slider_MouseEnter(object sender, MouseEventArgs e)
        {
            slider.Visibility = System.Windows.Visibility.Visible;
            SliderTimer.Stop();
        }

        private void slider_MouseLeave(object sender, MouseEventArgs e)
        {
            SliderHidden();
        }

        public void Start()
        {
            tbPlay.IsChecked = true;
        }

        public void Stop()
        {
            tbPlay.IsChecked = false;
        }
        ActionLine[] ActionLineArray;
        private void tbPlay_Checked(object sender, RoutedEventArgs e)
        {
            tbPlay_Checked();
        }


        public void tbPlay_Checked()
        {
            playItems = 0;

            if (timeLineItemList.Count > 0)
            {
                ActionLineArray = new ActionLine[timeLineItemList.Count];

                ActionLineArray[playItems] = new ActionLine(StartPlayLine);

                ActionActionLineArray();
            }
        }
        public void SetActionActionLineArray(int parm)
        {
            ActionLineArray[parm] = new ActionLine(StartPlayLine);
            ActionActionLineArray();
        }

        public void ActionActionLineArray()
        {
            ActionLineArray[playItems]();
        }

        private void StartPlayLine()
        {
            NewValue = 0;
            dispatcherTimer.Tag = timeLineItemList[playItems];
            dispatcherTimer.Start();
            if (PlayPause != null) PlayPause(this, new RoutedPropertyChangedEventArgs<bool>(true, false));
        }

        private void tbPlay_Unchecked(object sender, RoutedEventArgs e)
        {
            tbPlay_Unchecked();
        }
        public void tbPlay_Unchecked()
        {
            dispatcherTimer.Stop();
            NewValue -= 0.1;
            if (PlayPause != null) PlayPause(this, new RoutedPropertyChangedEventArgs<bool>(false, true));
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (Previous != null)
                Previous(this, new RoutedEventArgs());
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Next != null)
                Next(this, new RoutedEventArgs());
        }

        private void toggleButtonShrink_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb == null) return;
            if (ShrinkChanged != null)
                ShrinkChanged(this, new RoutedPropertyChangedEventArgs<bool>(tb.IsChecked == true ? true : false, tb.IsChecked == true ? false : true));
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            bool IsChecked;
            if (tb == null) return;

            IsChecked = tb.IsChecked == true ? false : true;

            if (MenuVisibleChanged != null)
                MenuVisibleChanged(this, new RoutedPropertyChangedEventArgs<bool>(IsChecked, !IsChecked));
        }
    }
}