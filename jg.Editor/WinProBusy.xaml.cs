using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jg.Editor
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class WinProBusy : Window
    {
        public WinProBusy()
        {
            InitializeComponent();
        }
        string _UserName = "";
        string _UserPassword = "";
        public WinProBusy(string UserName, string UserPassword)
        {
            InitializeComponent();
            _UserName = UserName;
            _UserPassword = UserPassword;
        }




        public delegate void Checkeddelegate(string UserName, string UserPassword);

        /// <summary>
        /// 给当前窗体赋值
        /// </summary>
        /// <param name="result"></param>
        public void setThisStaut(bool result)
        {
            Dispatcher.Invoke((Action)delegate
            {
                this.DialogResult = result;
            });
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Task task = new Task((Action)delegate
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                try
                {
                    string message = "";

                    log.Info(string.Format("User:{0} Login", _UserName));
                    //Globals.client.Login(out message, _UserName, _UserPassword);

                    if (message != "")
                    {
                        System.Windows.Forms.MessageBox.Show(message);
                        setThisStaut(false);
                        log.Info(string.Format("User:{0} Login failed", _UserName));
                    }
                    else
                        log.Info(string.Format("User:{0} Successful login", _UserName));




                    //#region 安全认证
                    //System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
                    //object[] attrs = ass.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                    //Guid guid = new Guid(((System.Runtime.InteropServices.GuidAttribute)attrs[0]).Value);


                    //#region IP
                    //string Message = "";
                    //IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                    //foreach (IPAddress ip in arrIPAddresses)
                    //{
                    //    if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                    //    {
                    //        Message += ip.ToString() + "_";
                    //    }
                    //}
                    //#endregion

                    //jg.Security.Library.CommandInfo commandInfo = new Security.Library.CommandInfo();
                    //commandInfo.Command = "login";
                    //commandInfo.User = windowlogin.UserName;
                    //commandInfo.Password = windowlogin.UserPassword;
                    //commandInfo.Key = guid.ToString().ToUpper();
                    //commandInfo.ModuleKey = guid.ToString().ToUpper();
                    //commandInfo.CustomerKey = guid.ToString().ToUpper();
                    //commandInfo.Message = Message;

                    //localSc = new SocketClient(Globals.LocalIP, Globals.RemoteIP, Globals.LocalPort, Globals.RemotePort);
                    //localSc.DisConnectedServer += sc_DisConnectedServer;
                    //localSc.ReceivedDatagram += sc_ReceivedDatagram;
                    //localSc.ConnectFailed += sc_ConnectFailed;


                    //if (localSc.Connected())
                    //{
                    //    localSc.Send(commandInfo.ToString(SocketClient.RESOLVER));
                    //}
                    //else
                    //{
                    //    MessageBox.Show(this, "无法正常连接到授权认证服务器。");
                    //    this.Close();
                    //}
                    //#endregion

                    Globals.InitializeComponent();
                   
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message + "\r\n\t" + ex.StackTrace);
                    MessageBox.Show(ex.Message);
                    Environment.Exit(0);
                    setThisStaut(false);
                }
                setThisStaut(true);
            });


            task.Start();
            
        }

    }
}
