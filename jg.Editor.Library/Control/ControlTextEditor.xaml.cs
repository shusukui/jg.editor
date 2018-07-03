
namespace jg.Editor.Library.Control
{
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
    using System.Windows.Controls.Primitives;
    using jg.Editor.Library.Property;
    using System.ComponentModel;

    /// <summary>
    /// ControlTextEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ControlTextEditor : UserControl, INotifyPropertyChanged
    {
        public delegate void SelectedChanged(SolidColorBrush c, object sender);
        public SelectedChanged selectedChangeddelegate = null;
        public bool IsEdit { get; set; }
        FontCollection fontCollection = new FontCollection();
        public FlowDocument Document
        {
            get { return mainRTB.Document; }
        }

        private SolidColorBrush _Forcground;

        public SolidColorBrush Forcground
        {
            get { return _Forcground; }
            set
            {
                _Forcground = value;
                this.OnPropertyChanged("Forcground");
            }
        }
        public ControlTextEditor()
        {
            InitializeComponent();
            AllowDrop = true;
            IsEdit = true;
            //cmbFontList.SelectionChanged += cmbFontList_SelectionChanged;

            FlowDocument document = mainRTB.Document;
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Background");
            document.SetBinding(FlowDocument.BackgroundProperty, binding);
            t = new TextBox();

            
            //cmbFontList.Items.Add("");
            //foreach (FontFamily v in fontCollection)
            //    cmbFontList.Items.Add(v);
        }

        //void cmbFontList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox cmb = sender as ComboBox;
        //    if (cmb == null) return;
        //    if (cmb.SelectedItem == null) return;

        //    string fontfamily = cmb.SelectedItem.ToString();
        //    if (fontfamily == "") return;
        //    mainRTB.Selection.ApplyPropertyValue(Run.FontFamilyProperty, fontfamily);
        //}

        private void mainRTB_SelectionChanged(object sender, RoutedEventArgs e)
        {

            //    RichTextBox rtb = sender as RichTextBox;
            //    if (rtb == null) return;

            //    object o = rtb.Selection.GetPropertyValue(Run.FontFamilyProperty);
            //    cmbFontList.SelectionChanged -= cmbFontList_SelectionChanged;
            //    for (int i = 0; i < cmbFontList.Items.Count; i++)
            //        if (cmbFontList.Items[i].ToString() == o.ToString()) cmbFontList.SelectedIndex = i;
            //    cmbFontList.SelectionChanged += cmbFontList_SelectionChanged;

        }

        public TextBox t = null;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainRTB.Selection.Changed += Selection_Changed;
            PropertyChanged += v_PropertyChanged;
        }



        void Selection_Changed(object sender, EventArgs e)
        {


            TextSelection richTextBox = sender as TextSelection;
            Brush b = richTextBox.GetPropertyValue(Hyperlink.ForegroundProperty) as Brush;
            if (b != null)
            {
                SolidColorBrush solidcolor = (SolidColorBrush)b;
                Forcground = solidcolor;
            }
            //if (richTextBox.Tag != null)
            //{
            //    ToolboxItem item = richTextBox.Tag as ToolboxItem;
            //    item.ItemForeground = solidcolor;
            //}
        }

        //private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        //{
        //    Xceed.Wpf.Toolkit.ColorPicker colorPicker = sender as Xceed.Wpf.Toolkit.ColorPicker;
        //    if (colorPicker == null) return;
        //    mainRTB.Selection.ApplyPropertyValue(Run.ForegroundProperty, new SolidColorBrush(e.NewValue));
        //}

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this.mainRTB, new PropertyChangedEventArgs(propertyName));
            }
        }

        void v_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            if (rtb.Tag != null)
            {
                ToolboxItem item = rtb.Tag as ToolboxItem;

                Binding b = new Binding();
                b.Source = this.Forcground;
                item.SetBinding(ToolboxItem.ItemForegroundProperty, b);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}