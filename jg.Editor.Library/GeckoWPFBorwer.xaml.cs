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
    /// GeckoWPFBorwer.xaml 的交互逻辑
    /// </summary>
    public partial class GeckoWPFBorwer : UserControl
    {
        private WebBrowerGecko _control = new WebBrowerGecko();
        public GeckoWPFBorwer()
    {
        InitializeComponent();
        host.Child = _control;
     
        this.Content = host;
    }

        public WebBrowerGecko Browser
    {
        get { return _control; }
    }
    }
}
