using jg.Editor.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace jg.PCPlayerLibrary
{
    /// <summary>
    /// windowAssetShow.xaml 的交互逻辑
    /// </summary>
    public partial class windowAssetShow : Window
    {
        private AssetType AssetType = AssetType.Image;
        private string assetpath = "";
        public string AssetPath
        {
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                string TempPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp" + value.Substring(value.LastIndexOf("\\"));
                switch (AssetType)
                {
                    case Editor.Library.AssetType.Image:
                        ImageSource imageSource = new BitmapImage(new Uri(TempPath, UriKind.Absolute));
                        image.Source = imageSource;
                        break;
                    case Editor.Library.AssetType.Movie:
                        mediaElement.Source = new Uri(TempPath, UriKind.Absolute);
                        mediaElement.Play();
                        break;
                }
            }
        }


        public ObservableCollection<string> stringlist = null;
        private UIElement _item;
        public UIElement item
        {
            get { return _item; }
            set
            {
                _item = value;
                if (value != null)
                {



                    if (value is RichTextBox)
                    {

                        string xaml = System.Windows.Markup.XamlWriter.Save(value);
                        RichTextBox rtb2 = System.Windows.Markup.XamlReader.Parse(xaml) as RichTextBox;
                        rtb2.Background = new SolidColorBrush(Colors.White);
                        GridBody.Children.Add(rtb2);
                    }
                    else if (value is Editor.Library.Control.TPageControl)
                    {
                        if (stringlist != null)
                        {
                            Editor.Library.Control.TPageControl page = new Editor.Library.Control.TPageControl();
                            page.Width = 1024;
                            page.Height = 768;

                            UpdateImgGroup(stringlist, page);
                            UIElement element = (UIElement)CloneObject(value);
                            GridBody.Children.Add(page);
                        }

                    }


                    else
                    {
                        UIElement element = (UIElement)CloneObject(value);
                        GridBody.Children.Add(element);
                    }
                }
            }
        }

        /// <summary>
        /// 更新图片组
        /// </summary>

        private static void UpdateImgGroup(System.Collections.ObjectModel.ObservableCollection<string> observable, Editor.Library.Control.TPageControl PageControl)
        {
            System.Collections.ObjectModel.ObservableCollection<Panel> Panels = new System.Collections.ObjectModel.ObservableCollection<Panel>();

            for (int i = 0; i < observable.Count; i++)
            {
                WrapPanel rectangle = new WrapPanel();
                rectangle.Width = PageControl.Width - 100;
                rectangle.Height = PageControl.Height - 2;
                BitmapImage bitimage = new BitmapImage();
                bitimage.BeginInit();
                bitimage.UriSource = new Uri(observable[i], UriKind.Absolute);
                bitimage.EndInit();

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bitimage;
                rectangle.Background = brush;
                Panels.Add(rectangle);
            }
            PageControl.AddPage(Panels, 1);
        }
        public delegate void SetUIElementpage();
        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();
            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    object value = pi.GetValue(o, null);

                    pi.SetValue(p, value, null);

                }
            }
            return p;
        }

        public TimeSpan Position
        {
            get { return mediaElement.Position; }
            set { mediaElement.Position = value; }
        }

        public windowAssetShow(AssetType assetType, ObservableCollection<string> stringcoll)
        {
            stringlist = stringcoll;
            InitializeComponent();
            AssetType = assetType;

            image.Visibility = System.Windows.Visibility.Collapsed;
            mediaElement.Visibility = System.Windows.Visibility.Collapsed;
            GridBody.Visibility = Visibility.Collapsed;

            switch (AssetType)
            {
                //case Editor.Library.AssetType.Image:
                //    image.Visibility = System.Windows.Visibility.Visible;
                //    break;
                case Editor.Library.AssetType.Movie:
                    mediaElement.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    GridBody.Visibility = Visibility.Visible;
                    break;
            }
            mediaElement.btnZoomChecked = false;
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += (a, b) =>
                {
                    this.Close();
                    timer.Stop();
                };
            //timer.Start();
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                imageScale.ScaleX += 0.1;
                imageScale.ScaleY += 0.1;
            }
            else
            {
                if (imageScale.ScaleX <= 0.5) return;
                imageScale.ScaleX -= 0.1;
                imageScale.ScaleY -= 0.1;
            }
        }

        private void mediaElement_ScaleChanged(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }

}
