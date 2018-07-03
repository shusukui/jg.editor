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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jg.Editor.Library.Property
{
    /// <summary>
    /// ControlPropertyTPage.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyTPage : UserControl
    {
        public delegate AssResInfo delegateAdd();

        public event delegateAdd AddClick = null;
        public ObservableCollection<AssResInfo> observable = new ObservableCollection<AssResInfo>();


        public delegate void UpdatePageChanged();
        public event UpdatePageChanged UpdatePageChangedHander = null;
        public ControlPropertyTPage()
        {
            InitializeComponent();
        }


        private DesignerItem _source = null;
        Control.TPageControl PageControl = null;
        ToolboxItem toolboxItem = null;
        public DesignerItem Source
        {
            get { return _source; }
            set
            {
                _source = value;


                if ((toolboxItem = _source.Content as ToolboxItem) == null) return;
                if ((PageControl = toolboxItem.Content as Control.TPageControl) == null) return;
                ItemSource = PageControl.Children;

                ViewHeight.TextChanged -= ViewHeight_TextChanged;
                ViewWidth.TextChanged -= ViewWidth_TextChanged;
                ViewHeight.TextChanged += ViewHeight_TextChanged;
                ViewWidth.TextChanged += ViewWidth_TextChanged;
                ViewHeight.Text = PageControl.ShowHeight.ToString();
                ViewWidth.Text = PageControl.Width.ToString();
            }
        }

        void ViewWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            double oldVlue;
            if (double.TryParse(ViewWidth.Text, out oldVlue))
                PageControl.ShowWidth = oldVlue;
        }

        void ViewHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            double oldVlue;
            if (double.TryParse(ViewHeight.Text, out oldVlue))
                PageControl.ShowHeight = oldVlue;
        }



        public object ItemSource
        {
            get { return (DependencyProperty)GetValue(ItemSourceProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(ItemSourceProperty, value);
                    Binding b = new Binding();
                    observable = (ObservableCollection<AssResInfo>)value;

                    b.Source = observable;

                    ImgGroupList.SetBinding(ListView.ItemsSourceProperty, b);

                }
                else
                {
                    observable = new ObservableCollection<AssResInfo>();
                    SetValue(ItemSourceProperty, observable);
                    Binding b = new Binding();
                    b.Source = observable;
                    ImgGroupList.SetBinding(ListView.ItemsSourceProperty, b);
                }

            }

        }
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(object), typeof(ControlPropertyTPage));

        private void Image_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (AddClick != null)
            {
                AssResInfo assResinfo = AddClick();
                if (assResinfo.AssetPath != null && assResinfo.AssetPath != "")
                {
                    assResinfo.AssetName = "图片" + (observable.Count + 1);
                    observable.Add(assResinfo);
                    PageControl.Children = observable;
                    UpdateImgGroup();
                }
            }


        }


        /// <summary>
        /// 更新图片组
        /// </summary>

        private void UpdateImgGroup()
        {
            ObservableCollection<Panel> Panels = new ObservableCollection<Panel>();
            toolboxItem.AssetPathAndThumbnailsList = observable;
            for (int i = 0; i < observable.Count; i++)
            {
                WrapPanel rectangle = new WrapPanel();
                rectangle.Width = PageControl.GetPageWidth();
                rectangle.Height = PageControl.GetPageHieght();
                BitmapImage bitimage = new BitmapImage();
                bitimage.BeginInit();
                bitimage.UriSource = new Uri(observable[i].Thumbnails, UriKind.Absolute);
                bitimage.EndInit();

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bitimage;
                rectangle.Background = brush;
                Panels.Add(rectangle);
            }
            PageControl.AddPage(Panels, 1);
        }

        public bool CheckedContent(AssResInfo ArInfo)
        {
            if (ArInfo == null)
            {
                MessageBox.Show("请选择中一项再做操作！");
                return false;
            }
            return true;
        }


        public bool IsChecked()
        {

            if (observable != null && observable.Count > 0)
            {
                AssResInfo ArInfo = observable.First(p => p.IsChecked);
                if (ArInfo != null)
                {
                    return true;
                }
            }
            return false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AssResInfo ArInfo = ImgGroupList.SelectedItem as AssResInfo;

            if (CheckedContent(ArInfo))
            {
                observable.Remove(ArInfo);
                UpdateImgGroup();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AssResInfo ArInfo = ImgGroupList.SelectedItem as AssResInfo;
            if (CheckedContent(ArInfo))
            {

                AssResInfo ArInfoSource = ArInfo;
                int index = observable.IndexOf(ArInfoSource);
                AssResInfo ArInfoTarget = null;
                ArInfoTarget = observable.ElementAt(index - 1);
                observable[index - 1] = ArInfoSource;
                observable[index] = ArInfoTarget;
                UpdateImgGroup();


            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AssResInfo ArInfo = ImgGroupList.SelectedItem as AssResInfo;
            if (CheckedContent(ArInfo))
            {

                AssResInfo ArInfoSource = ArInfo;
                int index = observable.IndexOf(ArInfoSource);
                AssResInfo ArInfoTarget = null;
                ArInfoTarget = observable.ElementAt(index + 1);
                observable[index + 1] = ArInfoSource;
                observable[index] = ArInfoTarget;
                UpdateImgGroup();
            }
        }



        private void ImgGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssResInfo ArInfo = ImgGroupList.SelectedItem as AssResInfo;
            if (observable.IndexOf(ArInfo) == 0)
            {
                UpButton.IsEnabled = false;
                DownButton.IsEnabled = true;
            }
            else if (observable.IndexOf(ArInfo) == observable.Count - 1)
            {
                UpButton.IsEnabled = true;
                DownButton.IsEnabled = false;
            }
            else
            {
                UpButton.IsEnabled = true;
                DownButton.IsEnabled = true;
            }
        }
    }
}
