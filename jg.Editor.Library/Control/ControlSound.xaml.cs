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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jg.Editor.Library.Control
{
    /// <summary>
    /// ControlSound.xaml 的交互逻辑
    /// </summary>
    public partial class ControlSound : UserControl
    {
        public event RoutedPropertyChangedEventHandler<double> VolumeChanged = null;
        Point PushPoint = new Point(0, 0);
        bool IsPush = false;
        public double Volume { get; set; }//音量

        public ControlSound()
        {
            InitializeComponent();
            Volume = 1;
        }

        void ControlShow(FrameworkElement frameworkElement, double ToValue)
        {
            DoubleAnimation doubleAnimation = doubleAnimation = new DoubleAnimation(ToValue, new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            frameworkElement.BeginAnimation(FrameworkElement.OpacityProperty, doubleAnimation);
        }

        private void mySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = e.NewValue;
            double NewVolume = value / mySlider.Maximum;
            if (VolumeChanged != null)
                VolumeChanged(this, new RoutedPropertyChangedEventArgs<double>(Volume, NewVolume));
            Volume = NewVolume;
            if (value < 3)
            {
                ControlShow(rectangle1, 0);
                ControlShow(rectangle2, 0);
                ControlShow(rectangle3, 0);
            }
            else if (value < 50)
            {
                ControlShow(rectangle1, 100);
                ControlShow(rectangle2, 0);
                ControlShow(rectangle3, 0);
            }
            else if (value < 100 && value >= 50)
            {
                ControlShow(rectangle1, 100);
                ControlShow(rectangle2, 100);
                ControlShow(rectangle3, 0);
            }
            else
            {
                ControlShow(rectangle1, 100);
                ControlShow(rectangle2, 100);
                ControlShow(rectangle3, 100);
            }

        }
    }
}
