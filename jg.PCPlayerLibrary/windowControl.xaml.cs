using jg.Editor.Library;
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
using System.Windows.Shapes;

namespace jg.PCPlayerLibrary
{
    /// <summary>
    /// windowControl.xaml 的交互逻辑
    /// </summary>
    public partial class windowControl : Window
    {
        public event MouseEventHandler _MouseEnter = null;
        public event MouseEventHandler _MouseLeave = null;
        public event RoutedPropertyChangedEventHandler<double> _ValueChanged = null;
        public event RoutedEventHandler _PlayStop = null;
        public event RoutedEventHandler _Previous = null;
        public event RoutedEventHandler _Next = null;
        public event RoutedPropertyChangedEventHandler<bool> _PlayPause = null;
        public event RoutedPropertyChangedEventHandler<bool> _ShrinkChanged = null;
        public event RoutedPropertyChangedEventHandler<bool> _MenuVisibleChanged = null;
        public event RoutedPropertyChangedEventHandler<double> _VolumeChanged = null;

        public windowControl()
        {
            InitializeComponent();
        }


        public List<TimeLineItemInfo> timeLineItemList
        {
            get { return controlPlay.timeLineItemList; }
            set { controlPlay.timeLineItemList = value; }
        }
        public double Maximum
        {
            get { return controlPlay.Maximum; }
            set { controlPlay.Maximum = value; }
        }
        public double Minimum
        {
            get { return controlPlay.Minimum; }
            set { controlPlay.Minimum = value; }
        }
        public double Value
        {
            get { return controlPlay.Value; }
            set { controlPlay.Value = value; }
        }
        public double Volume
        {
            get { return controlPlay.Volume; }
        }

        private void controlPlay_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_MouseEnter != null) _MouseEnter(sender, e);
        }
        private void controlPlay_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_MouseLeave != null) _MouseLeave(sender, e);
        }
        private void controlPlay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_ValueChanged != null) _ValueChanged(sender, e);
        }
        private void controlPlay_PlayStop(object sender, RoutedEventArgs e)
        {
            if (_PlayStop != null) _PlayStop(sender, e);
        }
        private void controlPlay_Previous(object sender, RoutedEventArgs e)
        {
            if (_Previous != null) _Previous(sender, e);
        }
        private void controlPlay_Next(object sender, RoutedEventArgs e)
        {
            if (_Next != null) _Next(sender, e);
        }
        private void controlPlay_PlayPause(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (_PlayPause != null) _PlayPause(sender, e);
        }
        private void controlPlay_ShrinkChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (_ShrinkChanged != null) _ShrinkChanged(sender, e);
        }
        private void controlPlay_MenuVisibleChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (_MenuVisibleChanged != null) _MenuVisibleChanged(sender, e);
        }
        private void controlPlay_VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_VolumeChanged != null) _VolumeChanged(sender, e);
        }

        public void Stop()
        {
            controlPlay.Stop();
        }
        public void Start()
        {
            controlPlay.Start();
        }
    }
}
