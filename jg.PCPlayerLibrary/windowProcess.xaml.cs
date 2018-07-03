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
using System.Windows.Shapes;

namespace jg.PCPlayerLibrary
{
    /// <summary>
    /// windowProcess.xaml 的交互逻辑
    /// </summary>
    public partial class windowProcess : Window
    {
        private double _process;
        public double Process
        {
            get { return _process; }
            set
            {
                _process = value; tbProcess.Text = string.Format("{0}", value.ToString("p"));
            }
        }
        public windowProcess()
        {
            InitializeComponent();
        }
    }
}
