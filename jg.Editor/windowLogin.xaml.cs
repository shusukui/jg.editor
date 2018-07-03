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

namespace jg.Editor
{
    /// <summary>
    /// windowLogin.xaml 的交互逻辑
    /// </summary>
    public partial class windowLogin : Window
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public windowLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            UserName = txtName.Text;
            UserPassword = txtPassword.Password;

            if (UserName != "")
                this.DialogResult = true;
            else
                this.DialogResult = false;

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
