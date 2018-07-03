using jg.Editor.Library;
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
    /// windowTree.xaml 的交互逻辑
    /// </summary>
    public partial class windowTree : Window
    {
        public event RoutedPropertyChangedEventHandler<object> _SelectedItemChanged = null;
        public event MouseEventHandler _MouseEnter = null;
        public event MouseEventHandler _MouseLeave = null;
        public windowTree()
        {
            InitializeComponent();
        }

        RoutedPropertyChangedEventArgs<object> Olde = null;
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
           
            if (_SelectedItemChanged != null)
            {
                _SelectedItemChanged(sender, e);
            }
        }

        private void treeView_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_MouseEnter != null)
                _MouseEnter(sender, e);
        }

        private void treeView_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_MouseLeave != null) 
                _MouseLeave(sender, e);
        }     

        private void treeView_Selected(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TreeViewItem).IsExpanded = true;
        }


    }
}
