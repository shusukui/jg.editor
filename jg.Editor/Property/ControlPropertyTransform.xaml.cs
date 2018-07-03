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

namespace jg.Editor.Property
{
    /// <summary>
    /// ControlPropertyTransform.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyTransform : UserControl
    {
        public event RoutedPropertyChangedEventHandler<double> PropertyRotateChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyScaleXChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyScaleYChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertySkewXChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertySkewYChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyTranslateXChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyTranslateYChanged = null;

        double oldAngle = 0, oldScaleX = 0, oldScaleY = 0, oldSkewX = 0, oldSkewY = 0, oldTranslateX = 0, oldTranslateY = 0;

        private DesignerItem _source = null;

        public ControlPropertyTransform()
        {
            InitializeComponent();
            txtAngle.TextChanged += txtAngle_TextChanged;
            txtScaleX.TextChanged += txtScaleX_TextChanged;
            txtScaleY.TextChanged += txtScaleY_TextChanged;
            txtSkewX.TextChanged += txtSkewX_TextChanged;
            txtSkewY.TextChanged += txtSkewY_TextChanged;
            txtTranslateX.TextChanged += txtTranslateX_TextChanged;
            txtTranslateY.TextChanged += txtTranslateY_TextChanged;
        }

        public DesignerItem Source
        {
            get { return _source; }
            set
            {
                _source = value;
                TransformGroup transformGroup = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                if (null == transformGroup) return;

                foreach (var v in transformGroup.Children)
                {
                    switch (v.GetType().Name)
                    {
                        case "RotateTransform":
                            RotateTransform rotateTransform = v as RotateTransform;
                            if (rotateTransform != null) { itemAngle = rotateTransform.Angle; oldAngle = rotateTransform.Angle; }
                            break;
                        case "ScaleTransform":
                            ScaleTransform scaleTransform = v as ScaleTransform;
                            if (scaleTransform != null) { itemScaleX = scaleTransform.ScaleX; itemScaleY = scaleTransform.ScaleY; oldScaleX = scaleTransform.ScaleX; oldScaleY = scaleTransform.ScaleY; }
                            break;
                        case "SkewTransform":
                            SkewTransform skewTransform = v as SkewTransform;
                            if (skewTransform != null) { itemSkewX = skewTransform.AngleX; itemSkewY = skewTransform.AngleY; oldSkewX = skewTransform.AngleX; oldSkewY = skewTransform.AngleY; }
                            break;
                        case "TranslateTransform":
                            TranslateTransform translateTransform = v as TranslateTransform;
                            if (translateTransform != null) { itemTranslateX = translateTransform.X; itemTranslateY = translateTransform.Y; oldTranslateX = translateTransform.X; oldTranslateY = translateTransform.Y; }
                            break;
                    }
                }
            }
        }

        public double itemAngle
        {
            get
            {
                if (_source == null) return 0;
                RotateTransform rt = FindTransform<RotateTransform>(((ToolboxItem)_source.Content).RenderTransform as TransformGroup);
                if (rt == null) return 0;
                return rt.Angle;
            }
            set
            {
                if (_source == null) return;
                txtAngle.TextChanged -= txtAngle_TextChanged;
                txtAngle.Text = value.ToString();
                txtAngle.TextChanged += txtAngle_TextChanged;
            }
        }

        void txtAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(RotateTransform).Name)
                    {
                        // ((RotateTransform)v).CenterX = _source.ActualWidth / 2;
                        // ((RotateTransform)v).CenterY = _source.ActualHeight / 2;
                        ((RotateTransform)v).Angle = value;
                    }
                }
                if (PropertyRotateChanged != null) PropertyRotateChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldAngle, value));
            }
            else
                txt.Text = oldAngle.ToString();
        }

        public double itemScaleX
        {
            get
            {
                if (_source == null) return 0;
                ScaleTransform st = FindTransform<ScaleTransform>(((ToolboxItem)_source.Content).RenderTransform as TransformGroup);
                if (st == null) return 0;
                return st.ScaleX;
            }
            set
            {
                if (_source == null) return;
                txtScaleX.TextChanged -= txtScaleX_TextChanged;
                txtScaleX.Text = value.ToString();
                txtScaleX.TextChanged += txtScaleX_TextChanged;
            }
        }

        void txtScaleX_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(ScaleTransform).Name)
                    {
                        ((ScaleTransform)v).ScaleX = value;
                    }
                }
                if (PropertyScaleXChanged != null) PropertyScaleXChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldScaleX, value));
            }
            else
                txt.Text = oldScaleX.ToString();
        }

        public double itemScaleY
        {
            get
            {
                if (_source == null) return 0;
                ScaleTransform st = FindTransform<ScaleTransform>(((ToolboxItem)_source.Content).RenderTransform as TransformGroup);
                if (st == null) return 0;
                return st.ScaleY;
            }
            set
            {
                if (_source == null) return;
                txtScaleY.TextChanged -= txtScaleY_TextChanged;
                txtScaleY.Text = value.ToString();
                txtScaleY.TextChanged += txtScaleY_TextChanged;
            }
        }

        void txtScaleY_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(ScaleTransform).Name)
                    {
                        ((ScaleTransform)v).ScaleY = value;
                    }
                }
                if (PropertyScaleYChanged != null) PropertyScaleYChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldScaleX, value));
            }
            else
                txt.Text = oldScaleY.ToString();
        }

        public double itemSkewX
        {
            get
            {
                if (_source == null) return 0;
                SkewTransform skew = FindTransform<SkewTransform>(((ToolboxItem)_source.Content).RenderTransform as TransformGroup);
                if (skew == null) return 0;
                return skew.AngleX;
            }
            set
            {
                if (_source == null) return;
                txtSkewX.TextChanged -= txtSkewX_TextChanged;
                txtSkewX.Text = value.ToString();
                txtSkewX.TextChanged += txtSkewX_TextChanged;
            }
        }

        void txtSkewX_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(SkewTransform).Name)
                    {
                        ((SkewTransform)v).AngleX = value;
                    }
                }
                if (PropertySkewXChanged != null) PropertySkewXChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldSkewX, value));
            }
            else
                txt.Text = oldSkewX.ToString();
        }

        public double itemSkewY
        {
            get
            {
                if (((ToolboxItem)_source.Content) == null) return 0;
                SkewTransform skew = FindTransform<SkewTransform>(_source.RenderTransform as TransformGroup);
                if (skew == null) return 0;
                return skew.AngleY;
            }
            set
            {
                if (_source == null) return;
                txtSkewY.TextChanged -= txtSkewY_TextChanged;
                txtSkewY.Text = value.ToString();
                txtSkewY.TextChanged += txtSkewY_TextChanged;
            }
        }

        void txtSkewY_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(SkewTransform).Name)
                    {
                        ((SkewTransform)v).AngleY = value;
                    }
                }
                if (PropertySkewYChanged != null) PropertySkewYChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldSkewY, value));
            }
            else
                txt.Text = oldSkewY.ToString();
        }

        public double itemTranslateX
        {
            get
            {
                if (_source == null) return 0;
                TranslateTransform tt = FindTransform<TranslateTransform>(((ToolboxItem)_source.Content).RenderTransform as TransformGroup);
                if (tt == null) return 0;
                return tt.X;
            }
            set
            {
                if (_source == null) return;
                txtTranslateX.TextChanged -= txtTranslateX_TextChanged;
                txtTranslateX.Text = value.ToString();
                txtTranslateX.TextChanged += txtTranslateX_TextChanged;
            }
        }

        void txtTranslateX_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(TranslateTransform).Name)
                    {
                        ((TranslateTransform)v).X = value;
                    }
                }
                if (PropertyTranslateXChanged != null) PropertyTranslateXChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldTranslateX, value));
            }
            else
                txt.Text = oldTranslateX.ToString();
        }

        public double itemTranslateY
        {
            get
            {
                if (_source == null) return 0;
                TranslateTransform tt = FindTransform<TranslateTransform>(((ToolboxItem)_source.Content).RenderTransform as TransformGroup);
                if (tt == null) return 0;
                return tt.Y;
            }
            set
            {
                if (_source == null) return;
                txtTranslateY.TextChanged -= txtTranslateY_TextChanged;
                txtTranslateY.Text = value.ToString();
                txtTranslateY.TextChanged += txtTranslateY_TextChanged;
            }
        }

        void txtTranslateY_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value;
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (_source == null) return;
            if (double.TryParse(txt.Text, out value))
            {
                TransformGroup tg = ((ToolboxItem)_source.Content).RenderTransform as TransformGroup;
                foreach (var v in tg.Children)
                {
                    if (v.GetType().Name == typeof(TranslateTransform).Name)
                    {
                        ((TranslateTransform)v).Y = value;
                    }
                }
                if (PropertyTranslateYChanged != null) PropertyTranslateYChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldTranslateY, value));
            }
            else
                txt.Text = oldTranslateY.ToString();
        }

        private T FindTransform<T>(TransformGroup tg) where T : Transform
        {
            if (tg == null) return null;
            foreach (var v in tg.Children)
            {
                if (v is T) return v as T;
            }
            return null;
        }
    }
}