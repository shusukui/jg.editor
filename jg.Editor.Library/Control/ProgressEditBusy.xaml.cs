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

namespace jg.Editor.Library.Control
{
    /// <summary>
    /// ProgressEditBusy.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressEditBusy : UserControl
    {
        public ProgressEditBusy()
        {
            InitializeComponent();
            
        }
        /// <summary>
        /// 进度条颜色
        /// </summary>
        public Brush BusyColor
        {
            get { return (Brush)GetValue(BusyColorProperty); }
            set
            {
                SetValue(BusyColorProperty, value);
               
            }
        }

        private readonly static DependencyProperty BusyColorProperty = DependencyProperty.Register("BusyColor", typeof(Brush), typeof(ProgressEditBusy));

        /// <summary>
        /// 第一行文字
        /// </summary>
        public string FirstLine
        {
            get { return (string)GetValue(FirstLineProperty); }
            set { SetValue(FirstLineProperty, value);
           
            }
        }

        private readonly static DependencyProperty FirstLineProperty = DependencyProperty.Register("FirstLine", typeof(string), typeof(ProgressEditBusy));

        /// <summary>
        /// 第二行文字
        /// </summary>
        public string TwoLine
        {
            get { return (string)GetValue(TwoLineProperty); }
            set { 
                SetValue(TwoLineProperty, value);
              
            }
        }

        private readonly static DependencyProperty TwoLineProperty = DependencyProperty.Register("TwoLine", typeof(string), typeof(ProgressEditBusy));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.busyPro.Foreground = BusyColor;
            this.firstLine.Content = FirstLine;
            this.twoLine.Content = TwoLine;
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
           
        }


        
     
       
    }
}
