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
using Gecko.Windows;
namespace jg.Editor
{
    /// <summary>
    /// test.xaml 的交互逻辑
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class test : Window
    {
       
        public test()
        {
          
            InitializeComponent();
            SetAutoRun();


        
       
        
        }


      
        public static void SetAutoRun()
        {
            Microsoft.Win32.RegistryKey HKLM = Microsoft.Win32.Registry.LocalMachine;

            using (Microsoft.Win32.RegistryKey runKey = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true))
            {
              
                runKey.SetValue( System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, 10001, Microsoft.Win32.RegistryValueKind.DWord);
                runKey.Close();
            }
        }  
    }
}
