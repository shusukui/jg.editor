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

namespace jg.Editor.Library
{
    /// <summary>
    /// TreeViewEditItem.xaml 的交互逻辑
    /// </summary>
    public partial class TreeViewEditItem : UserControl
    {
        void TreeViewEditItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
            TreeViewItemInfo info = e.NewValue as TreeViewItemInfo;
            if (info == null) return;
            txtTitle.Text = info.Title;
            tbTitle.Text = info.Title;

            if (info.IsEdit)
            {
                txtTitle.Visibility = System.Windows.Visibility.Visible;
                tbTitle.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                txtTitle.Visibility = System.Windows.Visibility.Hidden;
                tbTitle.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public TreeViewEditItem()
        {
            InitializeComponent();
            txtTitle.KeyDown += new KeyEventHandler(txtTitle_KeyDown);
            txtTitle.LostFocus += new RoutedEventHandler(txtTitle_LostFocus);

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(TreeViewEditItem_DataContextChanged);
        }

        void txtTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            TreeViewItemInfo info = this.DataContext as TreeViewItemInfo;
            if (info == null) return;
            info.IsEdit = false;
            txtTitle.Visibility = System.Windows.Visibility.Hidden;
            tbTitle.Visibility = System.Windows.Visibility.Visible;
        }

        void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            TreeViewItemInfo info = this.DataContext as TreeViewItemInfo;
            if (info == null) return;
            TextBox txt = sender as TextBox ;
            if(txt == null)return;
            switch (e.Key)
            {
                case Key.Return:
                    info.Title = txt.Text;
                    tbTitle.Text = txt.Text;
                    info.IsEdit = false;
                    txtTitle.Visibility = System.Windows.Visibility.Hidden;
                    tbTitle.Visibility = System.Windows.Visibility.Visible;
                    break;
                case Key.Escape:
                    info.IsEdit = false;
                    txtTitle.Visibility = System.Windows.Visibility.Hidden;
                    tbTitle.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

    }
}
