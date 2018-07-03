using jg.Editor.Library;
using jg.Editor.Library.Topic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace jg.Editor
{
    /// <summary>
    /// windowSave.xaml 的交互逻辑
    /// </summary>
    public partial class windowSave : Window
    {

        public List<string> DataFileList { get; set; }
        private List<string> _assetfilelist = new List<string>();
        public List<string> AssetFileList
        {
            get
            {
                return _assetfilelist;
            }
            set
            {
                _assetfilelist = value;
                for (int i = 0; i < _assetfilelist.Count; i++)
                    if (!System.IO.File.Exists(_assetfilelist[i]))
                    {
                        if (System.IO.File.Exists(Globals.TempFolder + "\\" + _assetfilelist[i]))
                            _assetfilelist[i] = Globals.TempFolder + "\\" + _assetfilelist[i];
                        else if (System.IO.File.Exists(Globals.appStartupPath + "\\" + Globals.thumbnailFolder + "\\" + _assetfilelist[i]))
                            _assetfilelist[i] = Globals.appStartupPath + "\\" + Globals.thumbnailFolder + "\\" + _assetfilelist[i];
                        else if (System.IO.File.Exists(Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + _assetfilelist[i]))
                            _assetfilelist[i] = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + _assetfilelist[i];
                    }
            }
        }

        /// <summary>
        /// 电子书是否包含带成绩的题目。
        /// </summary>
        public bool IsScore
        {
            get
            {
                bool value = false;
                foreach (var v in Globals.savePageList)
                {
                    foreach (var vv in v.saveItemList)
                    {
                        if (vv.Score > 0) return value = true;
                    }
                }
                return value;
            }
        }

        public List<DesignerCanvas> designerCanvasList { get; set; }
        private List<string> fileList = new List<string>();

        private List<CateGoryInfo> CateGoryList = new List<CateGoryInfo>();
        private bool IsLogin_Nop = false;
        public int PageCount { get; set; }

        byte[] Thumbnail;
        byte[] NOP_Thumbnail;

        Aliyun.OSS aliyun_oss;
        Aliyun.OpenServices.ClientConfiguration aliyun_cc = new Aliyun.OpenServices.ClientConfiguration() { ConnectionTimeout = -1 };
        delegate void OnShowProcess(double value);
        OnShowProcess ShowProcess = null;

        public windowSave()
        {
            InitializeComponent();       
        }

    
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TabItem item = tabControl.SelectedItem as TabItem;
                switch (item.Tag.ToString())
                {               
                    case "Location":
                        if (txtLocationPath.Text == "")
                        {
                            MessageBox.Show("请先选择保存路径。", "景格软件", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtLocationPath.Focus();
                            return;
                        }
                        Location(txtLocationPath.Text, DataFileList, AssetFileList);
                        this.DialogResult = true;
                        break;
               
                }

            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }

        }
       
        public void Location(string path, List<string> dataFileList, List<string> assetFileList)
        {
            if (string.IsNullOrEmpty(path)) return;
            Globals.SavePath = path;
            if (System.IO.File.Exists(Globals.SavePath)) System.IO.File.Delete(Globals.SavePath);

            AutoResetEvent[] events = new AutoResetEvent[] { new AutoResetEvent(false) };
            List<string> list = new List<string>();
            list.AddRange(dataFileList);
            list.AddRange(assetFileList);
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Publish));
            thread.Start(new Tuple<List<string>, AutoResetEvent>(list, events[0]));
            WaitHandle.WaitAll(events);
        }
     
        //设置进度条进度
        void course_Process(double value)
        {
            //pro.Dispatcher.Invoke(SetProcess, value);
        }
        private void Publish(object param)
        {
            var p = (Tuple<List<string>, AutoResetEvent>)param;

            List<string> fileList = p.Item1 as List<string>;
            if (fileList == null) return;

            FilePackage package = new FilePackage();

            package.Process += new FilePackage.OnProcess(course_Process);
            package.Publish(Globals.CourseWareGuid, fileList, Globals.SavePath);
            p.Item2.Set();
        }
        private void btnLocationPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfDialog = new System.Windows.Forms.SaveFileDialog();
            sfDialog.Filter = global::jg.Editor.Properties.Resources.ExtensionFilter;
            if (sfDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtLocationPath.Text = sfDialog.FileName;
                //Globals.SavePath = txtLocationPath.Text;
            }
        }
     
  
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CateGoryInfo
    {
        public CateGoryInfo()
        {
            Children = new List<CateGoryInfo>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CateGoryInfo> Children { get; set; }
    }
    public class ProductInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class NopUserInfo
    {
        public string UserState { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public double Integral { get; set; }
        public float Balance { get; set; }
    }
}