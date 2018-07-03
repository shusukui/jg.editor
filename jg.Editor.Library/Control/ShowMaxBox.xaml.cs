using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace jg.Editor.Library
{
    /// <summary>
    /// ShowMaxBox.xaml 的交互逻辑
    /// </summary>
    public partial class ShowMaxBox : UserControl
    {
        public ShowMaxBox()
        {
            InitializeComponent();
        }
        private UIElement _item;
        public UIElement item
        {
            get { return _item; }
            set
            {
                _item = value;
                if (value != null)
                {
                    Gridbody.Children.Add(value);
                }
            }
        }

        public delegate void ShowAsset(UIElement item, ObservableCollection<string> liststring);

        public event ShowAsset eventShowAsset = null;


        public ObservableCollection<string> stringlist = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Type t=item.GetType();

            UIElement tbi = Gridbody.Children.OfType <UIElement>().FirstOrDefault(p=>p==item);

            if (eventShowAsset != null)
            {
                eventShowAsset(tbi, stringlist);
            }

        }

    }
}
