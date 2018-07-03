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
    /// windowClose.xaml 的交互逻辑
    /// </summary>
    public partial class windowClose : Window
    {
        public event RoutedEventHandler _Click = null;
        public windowClose()
        {            
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_Click != null) _Click(sender, e);
        }
    }
}
