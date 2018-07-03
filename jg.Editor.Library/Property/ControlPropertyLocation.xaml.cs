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
    /// ControlPropertyLocation.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyLocation : UserControl
    {
        public event RoutedPropertyChangedEventHandler<double> PropertyHeightChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyWidthChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyXChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyYChanged = null;
        public event RoutedPropertyChangedEventHandler<string> PropertyItemNameChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyOpacityChanged = null;

        double oldHeight, oldWidth, oldX, oldY, oldOpacity;
        string oldItemName;

        public ControlPropertyLocation()
        {
            InitializeComponent();
        }

        private DesignerItem _source = null;
        public DesignerItem Source
        {
            get
            {
                return _source;
            }

            set
            {
                ToolboxItem toolboxItem;
                _source = value;
                toolboxItem = value.Content as ToolboxItem;
                if (toolboxItem == null) return;
                itemHeight = _source.ActualHeight;
                itemWidth = _source.ActualWidth;
                setCheckBox(toolboxItem.AssetType);

                IsShowDiv = _source.IsShowDiv;
                setCheckBoxIsDescPt(toolboxItem.AssetType);
                IsDescPt = _source.IsDescPt;
             
                ItemName = toolboxItem.ItemName;
                itemX = DesignerCanvas.GetLeft(_source);
                itemY = DesignerCanvas.GetTop(_source);
                if (value.timeLineItemInfo != null)
                    itemTimeFrame.ItemsSource = value.timeLineItemInfo.TimePointList;

                itemOpacity = _source.Opacity;
                _source.ItemDragComplete += new DesignerItem.OnItemDragComplete(_source_ItemDragComplete);
            }
        }

        public void setCheckBox(AssetType assettype)
        {
            chkIsShowDiv.Visibility = Visibility.Visible;
            switch (assettype)
            {
                case AssetType.Image:
                    chkIsShowDiv.Visibility = Visibility.Visible;
                    break;
                case AssetType.Movie:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.Sound:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.Shape:
                    chkIsShowDiv.Visibility = Visibility.Visible;
                    break;
                case AssetType.Line:
                    chkIsShowDiv.Visibility = Visibility.Visible;
                    break;
                case AssetType.Topic:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.TopicDrag:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.Text:
                    chkIsShowDiv.Visibility = Visibility.Visible;
                    break;
                case AssetType.Stage:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.TextGrid:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.Document:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.Message:
                    chkIsShowDiv.Visibility = Visibility.Visible;
                    break;
                case AssetType.HTML5:
                    chkIsShowDiv.Visibility = Visibility.Collapsed;
                    break;
                case AssetType.TPageGroup:
                    chkIsShowDiv.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        public void setCheckBoxIsDescPt(AssetType assettype)
        {
            chkIsShowDiv.Visibility = Visibility.Visible;
            switch (assettype)
            {
                case AssetType.Image:
                    chkIsShowDesc.Visibility = Visibility.Visible;
                    break;
                case AssetType.Movie:
                    chkIsShowDesc.Visibility = Visibility.Visible;
                    break;
                case AssetType.Sound:
                    chkIsShowDesc.Visibility = Visibility.Visible;
                    break;
                case AssetType.Shape:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.Line:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.Topic:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.TopicDrag:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.Text:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.Stage:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.TextGrid:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.Document:
                    chkIsShowDesc.Visibility = Visibility.Visible;
                    break;
                case AssetType.Message:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.HTML5:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                case AssetType.TPageGroup:
                    chkIsShowDesc.Visibility = Visibility.Collapsed;
                    _source.IsDescPt = false;
                    break;
                default:
                    break;
            }
        }

        void _source_ItemDragComplete(object sender, double width, double height, double left, double top, double oldwidth, double oldheight, double oldleft, double oldtop)
        {
            this.oldHeight = oldheight;
            this.oldWidth = oldwidth;
            this.oldX = oldleft;
            this.oldY = oldtop;

            itemHeight = height;
            itemWidth = width;
            itemX = left;
            itemY = top;
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
                // _source.Height = value;
                txtHeight.Text = value.ToString();
                txtHeight.TextChanged += txtHeight_TextChanged;
            }
        }

        private bool _IsShowDiv;

        public bool IsShowDiv
        {
            get
            {
                if (_source == null)
                    return false;
                else
                {
                    return _source.IsShowDiv;
                }
            }
            set
            {
                if (_source == null) return;
                if (value)
                {
                    chkIsShowDiv.IsChecked = true;
                    // _source.FontStyle = FontStyles.Italic;
                }
                else
                {
                    chkIsShowDiv.IsChecked = false;
                    // _source.FontStyle = FontStyles.Normal;
                }
            }

        }
        private bool _IsDescPt;

        public bool IsDescPt
        {
            get
            {
                if (_source == null)
                    return false;
                else
                {
                    return _source.IsDescPt;
                }
            }
            set
            {
                if (_source == null) return;
                if (value)
                {
                    chkIsShowDesc.IsChecked = true;
                    // _source.FontStyle = FontStyles.Italic;
                }
                else
                {
                    chkIsShowDesc.IsChecked = false;
                    // _source.FontStyle = FontStyles.Normal;
                }
            }

        }
        public double itemWidth
        {
            get
            {
                if (_source == null) return 0; else return _source.ActualWidth;
            }
            set
            {
                if (_source == null) return;

                txtWidth.TextChanged -= txtWidth_TextChanged;

                // _source.Width = value;
                txtWidth.Text = value.ToString();

                txtWidth.TextChanged += txtWidth_TextChanged;
            }
        }

        public double itemX
        {
            get
            {
                if (_source == null) return 0; else return DesignerCanvas.GetLeft(_source);
            }
            set
            {
                if (_source == null) return;
                txtX.TextChanged -= txtX_TextChanged;

                // DesignerCanvas.SetLeft(_source, value);
                txtX.Text = value.ToString();

                txtX.TextChanged += txtX_TextChanged;
            }
        }

        public double itemY
        {
            get
            {
                if (_source == null) return 0; else return DesignerCanvas.GetTop(_source);
            }
            set
            {
                if (_source == null) return;
                txtY.TextChanged -= txtY_TextChanged;
                txtY.Text = value.ToString();
                txtY.TextChanged += txtY_TextChanged;
            }
        }

        public double itemOpacity
        {
            get
            {
                if (_source == null) return 0; else return _source.Opacity;
            }
            set
            {
                if (_source == null) return;
                txtOpacity.TextChanged += txtOpacity_TextChanged;
                txtOpacity.Text = value.ToString();
                txtOpacity.TextChanged += txtOpacity_TextChanged;
            }
        }

        void txtOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            double opacity;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;

            if (double.TryParse(txt.Text, out opacity))
            {
                _source.Opacity = opacity;
                if (PropertyOpacityChanged == null) return;

                PropertyOpacityChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldOpacity, opacity));
            }
        }

        public string ItemName
        {
            get
            {
                if (_source == null) return ""; else return txtName.Text;
            }
            set
            {
                if (_source == null) return;
                txtName.TextChanged -= txtName_TextChanged;
                if (value != null)
                    txtName.Text = value.ToString();
                else
                    txtName.Text = "";
                txtName.TextChanged += txtName_TextChanged;
            }
        }

        void txtHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            double height;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;

            if (double.TryParse(txt.Text, out height))
            {
                _source.Height = height;
                if (PropertyHeightChanged == null) return;

                PropertyHeightChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldHeight, height));
            }
        }

        void txtWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            double width;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;

            if (double.TryParse(txt.Text, out width))
            {
                _source.Width = width;
                if (PropertyWidthChanged == null) return;

                PropertyWidthChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldHeight, width));
            }
        }

        void txtX_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;

            if (double.TryParse(txt.Text, out x))
            {
                DesignerCanvas.SetLeft(_source, x);
                if (PropertyXChanged == null) return;

                PropertyXChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldHeight, x));
            }
        }

        void txtY_TextChanged(object sender, TextChangedEventArgs e)
        {
            double y;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;

            if (double.TryParse(txt.Text, out y))
            {
                DesignerCanvas.SetTop(_source, y);
                if (PropertyYChanged == null) return;

                PropertyYChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldHeight, y));
            }
        }

        void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string itemname;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;

            itemname = txt.Text;
            _source.ItemName = itemname;
            if (PropertyItemNameChanged == null) return;

            PropertyItemNameChanged(_source, new RoutedPropertyChangedEventArgs<string>(oldItemName, itemname));
        }

        private void txtWidth_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldWidth);
        }

        private void txtHeight_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldHeight);
        }

        private void txtX_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldX);
        }

        private void txtY_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldY);
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            oldItemName = txt.Text;
        }

        private void txtOpacity_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            double.TryParse(txt.Text, out oldWidth);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;
            if (_source == null) return;
            _source.IsShowDiv = checkBox.IsChecked == true ? true : false;
        }

        private void chkIsShowDesc_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;
            if (_source == null) return;
            _source.IsDescPt = checkBox.IsChecked == true ? true : false;
        }
    }
}