using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;
using jg.Editor.Library;
using log4net;
using System.Net;
using System.Net.Sockets;
namespace jg.Editor
{
    public class Program
    {
        //static SecurityClientCallback callback = new SecurityClientCallback();
        //static RemoteSecurityClientCallback remoteCallback = new RemoteSecurityClientCallback();

        //static System.ServiceModel.InstanceContext context = null;
        //static System.ServiceModel.InstanceContext remoteContext = null;

        //static System.Threading.Thread thread = null;
        

        //static SecurityClient.ServiceClient sc = null;
        //static SecurityClientRemote.ServiceClient scr = null;
        [STAThread]
        static void Main(string[] args)
        {
            string key, version, id;
            Process processUpdate;
            const string UpdatePath = "jg.UpdateClient.exe";

            //SplashScreen splashScreen = new SplashScreen("SplashScreen1.png");
            //splashScreen.Show(true);
            //Window2 w2 = new Window2();
            //w2.ShowDialog();
            //return;


            //生成公钥、私钥
            Globals.rsaCryption.RSAKey(out Globals.PrivateKey, out Globals.PublicKey);


            #region 检查更新

            //Attribute guid_attr = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute));
            //key = ((GuidAttribute)guid_attr).Value;
            //version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //id = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            //if (File.Exists(Globals.appStartupPath + "\\" + UpdatePath))
            //{
            //    processUpdate = Process.Start(Globals.appStartupPath + "\\" + UpdatePath, key + " " + version + " " + id);
            //    processUpdate.WaitForExit();
            //}

            #endregion



            log4net.Config.XmlConfigurator.Configure(); // only config one time
            jg.Editor.Library.Globals.AssetDecryptKey = "2-1655469";
            //Globals.log = log4net.LogManager.GetLogger("LogFileAppender");
            RegisterFileAssociation(); //添加注册表，jgx文件格式
            XmlSerializer xmlSerializer;
            xmlSerializer = new XmlSerializer(typeof(AssetActionInfo));
            xmlSerializer = new XmlSerializer(typeof(TimePoint), new Type[] { typeof(abstractAssetProperty), typeof(AssetDoubleProperty), typeof(AssetColorProperty) });
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    Globals.SavePath = args[0];
                }
            }
            App app = new App();
            app.InitializeComponent();
            app.Run();

            //if (System.IO.Directory.Exists(Globals.TempFolder))
            //{
            //    foreach (var v in System.IO.Directory.GetFiles(Globals.TempFolder))
            //    {
            //        try
            //        {
            //            System.IO.File.Delete(v);
            //        }
            //        catch { }
            //    }
            //    try
            //    {
            //        System.IO.Directory.Delete(Globals.TempFolder);
            //    }
            //    catch { }
            //}
            
        }

       
        //设置jgx文件格式的注册表
        private static void RegisterFileAssociation()
        {
            string path = Assembly.GetEntryAssembly().Location;
            string openPath = path.Substring(0, path.LastIndexOf("\\") + 1) + Globals.PCPlayerPath;
            string appId = Path.GetFileNameWithoutExtension(path).Replace(" ", "_");
            FileAssociationHelper.Register(appId, appId, string.Format("\"{0}\" \"%1\"", openPath), string.Format("\"{0}\" \"%1\"", path), Properties.Resources.CourseExtension);
            FileAssociationHelper.Register(appId, appId, string.Format("\"{0}\" \"%1\"", openPath), string.Format("\"{0}\" \"%1\"", path), Properties.Resources.CourseExtensionServer);
        }


        private void SetWebbrower()
        {
           
        }

        public static void SetAutoRun()
        {
            Microsoft.Win32.RegistryKey HKLM = Microsoft.Win32.Registry.LocalMachine;

            using (Microsoft.Win32.RegistryKey runKey = HKLM.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
            {
                runKey.SetValue("test",10001 ,Microsoft.Win32.RegistryValueKind.DWord);
                runKey.Close();
            }
        }  
    }
  
    //public class RemoteSecurityClientCallback : SecurityClientRemote.IServiceCallback
    //{

    //    public void LogOff(string code)
    //    {
    //        MessageBox.Show(code);
    //        System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
    //    }
    //}

}