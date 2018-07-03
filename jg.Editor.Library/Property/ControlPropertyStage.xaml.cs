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

namespace jg.Editor.Library.Property
{
    /// <summary>
    /// ControlPropertyStage.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyStage : UserControl
    {
        public event RoutedPropertyChangedEventHandler<double> PropertyWidthChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyHeightChanged = null;
        public event RoutedPropertyChangedEventHandler<Color> PropertyBackgroundChanged = null;
        public event RoutedPropertyChangedEventHandler<string> PropertyBackimageChanged = null;

        double oldHeight, oldWidth, height = 720, width = 1280;
        public double Height { get { return height; } set { height = value; } }
        public double Width { get { return width; } set { width = value; } }

        Color oldColor, color = Brushes.LightBlue.Color;

        public Color Color { get { return color; } set { color = value; } }

        string oldBackimage;

        public ControlPropertyStage()
        {
            InitializeComponent();
            txtHeight.TextChanged += new TextChangedEventHandler(txtHeight_TextChanged);
            txtHeight.KeyDown += new KeyEventHandler(txtHeight_KeyDown);

            txtWidth.TextChanged += new TextChangedEventHandler(txtWidth_TextChanged);
            txtWidth.KeyDown += new KeyEventHandler(txtWidth_KeyDown);

            colorPicker.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color>(colorPicker_SelectedColorChanged);

            txtAssetId.TextChanged += new TextChangedEventHandler(txtAssetId_TextChanged);
            txtAssetId.KeyDown += new KeyEventHandler(txtAssetId_KeyDown);

            btnSelAsset.Click += new RoutedEventHandler(btnSelAsset_Click);
        }

        void btnSelAsset_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofDialog = new System.Windows.Forms.OpenFileDialog() { Filter = "位图文件(*.bmp;*.dib)|*.bmp;*.dib|JPEG (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|GIF (*.gif)|*.gif|TIFF (*.tif;*.tiff)|*.tif;*.tiff|PNG (*.png)|*.png" };
            if (ofDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImageBrush imageBrush = new ImageBrush();
                ImageSource imageSource = new BitmapImage(new Uri(ofDialog.FileName, UriKind.Absolute));
                imageBrush.ImageSource = imageSource;
                Source.Background = imageBrush;
            }
        }

        void txtAssetId_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            oldBackimage = txt.Text;
        }

        void txtAssetId_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            string backimage;
            if (_source != null && txt != null)
            {
                backimage = txt.Text;
                if (PropertyBackimageChanged != null)
                    PropertyBackimageChanged(_source, new RoutedPropertyChangedEventArgs<string>(oldBackimage, backimage));
            }
        }

        void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (_source != null)
            {
                oldColor = e.OldValue;
                color = e.NewValue;
                if (chkPublic.IsChecked == true)
                {
                    foreach (var v in ((Grid)_source.Parent).Children.OfType<DesignerCanvas>())
                        v.Background = new SolidColorBrush(e.NewValue);
                }
                else
                    _source.Background = new SolidColorBrush(e.NewValue);

                if (PropertyBackgroundChanged != null)
                    PropertyBackgroundChanged(_source, new RoutedPropertyChangedEventArgs<Color>(e.OldValue, e.NewValue));
            }
        }

        void txtWidth_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldWidth);
        }

        void txtWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (_source != null && txt != null)
            {
                if (double.TryParse(txt.Text, out width))
                {
                    foreach (var v in ((Grid)_source.Parent).Children.OfType<DesignerCanvas>())
                        v.Width = width;

                    if (PropertyWidthChanged != null)
                        PropertyWidthChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldWidth, width));
                }
                else
                    txt.Text = oldWidth.ToString();
            }
        }

        void txtHeight_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldHeight);
        }

        void txtHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (_source != null && txt != null)
            {
                if (double.TryParse(txt.Text, out height))
                {
                    foreach (var v in ((Grid)_source.Parent).Children.OfType<DesignerCanvas>())
                        v.Height = height;

                    if (PropertyHeightChanged != null)
                        PropertyHeightChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldHeight, height));
                }
                else
                {
                    txt.Text = oldHeight.ToString();
                }
            }
        }

        private DesignerCanvas _source = null;

        public DesignerCanvas Source
        {
            get { return _source; }
            set
            {
                _source = value;
                itemHeight = value.ActualHeight;
                itemWidth = value.ActualWidth;
                chkAutoNext.Click -= CheckBox_Checked;
                chkAutoNext.IsChecked = value.AutoNext;
                chkAutoNext.Click += CheckBox_Checked;
                chkIsVisable.Click -= chkIsVisable_Checked;
                chkIsVisable.IsChecked = value.IsVisable;
                chkIsVisable.Click += chkIsVisable_Checked;
                cmbSwitch.SelectedIndex = (int)value.StageSwitch;
                if (value.Background.GetType().Name == "SolidColorBrush")
                    itemBackground = (Color)ColorConverter.ConvertFromString(value.Background.ToString());

            }
        }

        public double itemHeight
        {
            get
            {
                if (_source == null)
                    return 0;
                else
                    return _source.ActualHeight;
            }
            set
            {
                if (_source == null) return;

                txtHeight.TextChanged -= txtHeight_TextChanged;
                txtHeight.Text = value.ToString();
                txtHeight.TextChanged += txtHeight_TextChanged;

            }
        }

        public double itemWidth
        {
            get
            {
                if (_source == null)
                    return 0;
                else
                    return _source.ActualWidth;
            }
            set
            {
                if (_source == null) return;


                txtWidth.TextChanged -= txtWidth_TextChanged;
                txtWidth.Text = value.ToString();
                txtWidth.TextChanged += txtWidth_TextChanged;

            }
        }

        public Color itemBackground
        {
            get
            {
                if (_source == null)
                    return Color.FromArgb(255, 0, 0, 0);
                else
                    return (Color)ColorConverter.ConvertFromString(_source.Background.ToString());
            }
            set
            {
                if (_source == null) return;
                colorPicker.SelectedColorChanged -= colorPicker_SelectedColorChanged;
                colorPicker.SelectedColor = value;
                colorPicker.SelectedColorChanged += colorPicker_SelectedColorChanged;
            }
        }

        public string itemBackimage
        {
            get
            {
                if (_source == null)
                    return "";
                else
                    return _source.Name;
            }
            set
            {
                if (_source == null) return;

            }
        }

        private void cmbSwitch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb == null) return;
            if (_source == null) return;
            _source.StageSwitch = (enumStageSwitch)cmb.SelectedIndex;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;
            if (_source == null) return;
            _source.AutoNext = checkBox.IsChecked == true ? true : false;
        }

        private void chkIsVisable_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;
            if (_source == null) return;
            _source.IsVisable = checkBox.IsChecked == true ? true : false;
        }
    }
}