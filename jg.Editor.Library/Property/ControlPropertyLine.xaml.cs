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
    /// ControlPropertyActon.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyLine : UserControl
    {
        public ControlPropertyLine()
        {
            InitializeComponent();
        }
        
        private DesignerItem _source = null;
        Control.ControlLine controlLine = null;
        ToolboxItem toolboxItem = null;
        public DesignerItem Source
        {
            get { return _source; }
            set
            {
                _source = value;

                if((toolboxItem= _source.Content as ToolboxItem) == null) return;
                if ((controlLine = toolboxItem.Content as Control.ControlLine) == null) return;
                txtPoint1_x.TextChanged -= txtPoint_TextChanged;
                txtPoint1_y.TextChanged -= txtPoint_TextChanged;

                txtPoint1_x.Text = controlLine.Point1.X.ToString();
                txtPoint1_y.Text = controlLine.Point1.Y.ToString();

                txtPoint1_x.TextChanged += txtPoint_TextChanged;
                txtPoint1_y.TextChanged += txtPoint_TextChanged;

            }
        }

        void txtPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x1, y1, x2, y2;
            if (!double.TryParse(txtPoint1_x.Text, out x1)) { txtPoint1_x.Focus(); txtPoint1_x.SelectAll(); }
            if (!double.TryParse(txtPoint1_y.Text, out y1)) { txtPoint1_y.Focus(); txtPoint1_y.SelectAll(); }
            controlLine.Point1 = new Point(x1, y1);
        }     
    }
}
