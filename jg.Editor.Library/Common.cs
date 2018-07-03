namespace jg.Editor.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Forms;
    using System.Windows.Data;

    class Common
    {
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if ((child != null) && (child is T))
                {
                    return (T)child;
                }
                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return default(T);
        }

        public static double GetMediaTimeLength(string path)
        {
            double duration = 0;
            string[] list;
            using (System.Diagnostics.Process pro = new System.Diagnostics.Process())
            {
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.ErrorDialog = false;
                pro.StartInfo.RedirectStandardError = true;

                pro.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "Lib\\ffmpeg.exe";
                pro.StartInfo.Arguments = " -i " + path;

                pro.Start();
                System.IO.StreamReader errorreader = pro.StandardError;
                pro.WaitForExit(1000);

                string result = errorreader.ReadToEnd();
                if (!string.IsNullOrEmpty(result))
                    result = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);
                if (string.IsNullOrEmpty(result)) return 0;

                list = result.Split(new string[] { ":" }, StringSplitOptions.None);
                if (list.Length != 3) return 0;
                duration = int.Parse(list[0]) * 3600 + int.Parse(list[1]) * 60 + int.Parse(list[2]);
            }
            return duration;
        }

      
    }

    public class DataConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? returnValue;
            if (value == null) return false;
            bool v = false;
            bool.TryParse(value.ToString(), out v);

            returnValue = v;
            return returnValue;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class DataConverterVisible : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Visible;
            bool v = false;
            bool.TryParse(value.ToString(), out v);

            if (v)
                return Visibility.Visible;
            else
                return Visibility.Hidden;

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public abstract class abstractSwitch
    {
        public abstractSwitch()
        {
            _storyboard.Completed += new EventHandler(_storyboard_Completed);
        }

        void _storyboard_Completed(object sender, EventArgs e)
        {
            Panel.Visibility = Visibility.Hidden;
        }

        private Canvas _stage;
        //舞台组件
        public Canvas Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }
        private Rectangle _panel;
        //遮罩层
        public Rectangle Panel
        {
            get { return _panel; }
            set { _panel = value; Panel.Visibility = Visibility.Visible; }
        }
        private Storyboard _storyboard = new Storyboard();
        public Storyboard Storyboard
        {
            get { return _storyboard; }
            set { _storyboard = value; }
        }
    }

    //无效果
    public class SwitchFF00003D : abstractSwitch
    {
        public SwitchFF00003D(Canvas stage, Rectangle panel, TimeSpan timeSpan)
            : base()
        {
            Stage = stage;
            Panel = panel;
            Panel.Visibility = Visibility.Hidden;
        }

    }
    //淡出
    public class SwitchFF00003E : abstractSwitch
    {
        public SwitchFF00003E(Canvas stage, Rectangle panel, TimeSpan timeSpan)
            : base()
        {
            Stage = stage;
            Panel = panel;
            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(timeSpan));
            Storyboard.SetTargetName(doubleAnimation, panel.Name);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(UIElement.Opacity)"));
            Storyboard.Children.Add(doubleAnimation);
        }

    }
    //推进
    public class SwitchFF00003F : abstractSwitch
    {
        public SwitchFF00003F(Canvas stage, Rectangle panel, TimeSpan timeSpan, string Direction)
            : base()
        {
            Stage = stage;
            Panel = panel;

            Panel.Opacity = 0;
            DoubleAnimation thicknessAnimationStage = null;
            ThicknessAnimation thicknessAnimationPanel = null;
            switch (Direction)
            {
                case "Previous":
                    Stage.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    thicknessAnimationStage = new DoubleAnimation(0, stage.ActualWidth, new Duration(timeSpan));
                    //thicknessAnimationPanel = new ThicknessAnimation(new Thickness(0), new Thickness(panel.ActualWidth, 0, 0 - panel.ActualWidth, 0), new Duration(timeSpan));
                    break;
                case "Next":
                    Stage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    thicknessAnimationStage = new DoubleAnimation(0, stage.ActualWidth, new Duration(timeSpan));
                    //thicknessAnimationPanel = new ThicknessAnimation(new Thickness(0), new Thickness(0 - panel.ActualWidth, 0, panel.ActualWidth, 0), new Duration(timeSpan));
                    break;
            }

            Storyboard.SetTargetName(thicknessAnimationStage, stage.Name);
            Storyboard.SetTargetProperty(thicknessAnimationStage, new PropertyPath("(FrameworkElement.Width)"));

            //Storyboard.SetTargetName(thicknessAnimationPanel, panel.Name);
            //Storyboard.SetTargetProperty(thicknessAnimationPanel, new PropertyPath("(FrameworkElement.Margin)"));
            Storyboard.Children.Add(thicknessAnimationStage);
            //Storyboard.Children.Add(thicknessAnimationPanel);
        }
    }
    //擦除
    public class SwitchFF000040 : abstractSwitch
    {
        public SwitchFF000040(Canvas stage, Rectangle panel, TimeSpan timeSpan)
            : base()
        {
            Stage = stage;
            Panel = panel;

            LinearGradientBrush linear = new LinearGradientBrush();

            linear.StartPoint = new Point(0, 0);
            linear.EndPoint = new Point(1, 0);
            GradientStop gradientStop1 = new GradientStop((Color)ColorConverter.ConvertFromString("#FF000000"), 1);
            GradientStop gradientStop2 = new GradientStop((Color)ColorConverter.ConvertFromString("#00FFFFFF"), 1);
            linear.GradientStops.Add(gradientStop1);
            linear.GradientStops.Add(gradientStop2);

            Panel.Fill = linear;

            DoubleAnimation doubleAnimationPanel1 = new DoubleAnimation(1, -1, new Duration(timeSpan));
            DoubleAnimation doubleAnimationPanel2 = new DoubleAnimation(2, 0, new Duration(timeSpan));


            Storyboard.SetTargetName(doubleAnimationPanel1, panel.Name);
            Storyboard.SetTargetProperty(doubleAnimationPanel1, new PropertyPath("(Shape.Fill).(GradientBrush.GradientStops)[0].(GradientStop.Offset)"));

            Storyboard.SetTargetName(doubleAnimationPanel2, panel.Name);
            Storyboard.SetTargetProperty(doubleAnimationPanel2, new PropertyPath("(Shape.Fill).(GradientBrush.GradientStops)[1].(GradientStop.Offset)"));

            Storyboard.Children.Add(doubleAnimationPanel1);
            Storyboard.Children.Add(doubleAnimationPanel2);

        }
    }

    public class SwitchFF00008C : abstractSwitch
    {
        public SwitchFF00008C(Canvas stage, Rectangle panel, TimeSpan timeSpan)
            : base()
        {

            Stage = stage;
            Panel = panel;

            Panel.Opacity = 0;
            DoubleAnimation thicknessAnimationStage = null;
            
            thicknessAnimationStage = new DoubleAnimation(0, stage.ActualWidth, new Duration(timeSpan));
            //thicknessAnimationPanel = new ThicknessAnimation(new Thickness(0), new Thickness(0 - panel.ActualWidth, 0, panel.ActualWidth, 0), new Duration(timeSpan));
            Storyboard.SetTargetName(thicknessAnimationStage, stage.Name);
            Storyboard.SetTargetProperty(thicknessAnimationStage, new PropertyPath("(FrameworkElement.Width)"));
            //Storyboard.SetTargetName(thicknessAnimationPanel, panel.Name);
            //Storyboard.SetTargetProperty(thicknessAnimationPanel, new PropertyPath("(FrameworkElement.Margin)"));
            Storyboard.Children.Add(thicknessAnimationStage);
            //Storyboard.Children.Add(thicknessAnimationPanel);
        }
    }
}