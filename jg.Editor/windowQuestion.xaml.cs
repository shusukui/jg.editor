using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    /// windowQuestion.xaml 的交互逻辑
    /// </summary>
    public partial class windowQuestion : Window
    {
        public windowQuestion()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Net.Mail.SmtpClient client = new SmtpClient("smtp.exmail.qq.com");

                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("shushukui@jingge.com", "mm4a6flr");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailAddress addressFrom = new MailAddress("shushukui@jingge.com", "WPF编辑器问题提交");
                MailAddress addressTo = new MailAddress("shushukui@jingge.com", "WPF编辑器问题提交");

                System.Net.Mail.MailMessage message = new MailMessage(addressFrom, addressTo);
                message.Sender = new MailAddress("shushukui@jingge.com");
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Subject = txtTitle.Text;
                message.Body = txtContent.Text;
                client.Send(message);
                MessageBox.Show("提交成功！");
            }
            catch(Exception ex)
            {
                MessageBox.Show("提交失败！");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
