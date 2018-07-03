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
    /// ContrilPropertyBackground.xaml 的交互逻辑
    /// </summary>
    public partial class ContrilPropertyBackground : UserControl
    {
        private DesignerItem _source;


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
                Binding b = new Binding();
                b.Mode = BindingMode.OneWay;
                b.Source = toolboxItem.ItemBackground.Color;
                selectCor.SetBinding(Xceed.Wpf.Toolkit.ColorCanvas.SelectedColorProperty, b);
            }
        }


        public ContrilPropertyBackground()
        {
            InitializeComponent();
        }
    }
}
