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
using System.IO;
using jg.Editor.Library;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace jg.Editor
{
    /// <summary>
    /// WindowAssetSel.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAssetSel : Window
    {
        private string path, thumbnails;
        private double mediatimelength = 0;
        //素材路径
        public string Path { get { return path; } }
        //缩略图路径
        public string Thumbnails { get { return thumbnails; } }
        //媒体文件播放时长
        public double MediaTimeLength { get { return mediatimelength; } }

        //要查找的文件类型
        private string[] filetype = new string[] { };

        public WindowAssetSel(string[] FileType)
        {
            InitializeComponent();
            searchBox.TextChanged += TextBox_TextChanged;
            Bpage = true;
            filetype = FileType;
            tvResTree.ItemsSource = Globals.CPDPM_DirSourceClassList;
        }

        bool Bpage = false;
        AssResInfo _AssInfo = new AssResInfo();
        public WindowAssetSel(string[] FileType, out jg.Editor.Library.AssResInfo Assinfo, bool b = true)
        {
            Assinfo = _AssInfo;
            InitializeComponent();
            Bpage = b;
            filetype = FileType;
            tvResTree.ItemsSource = Globals.CPDPM_DirSourceClassList;
        }
        //点击树节点填充列表
        private void tvResTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Globals.AssetLoadingIcon);
                MINIService.CPDPM_DirSourceClass model = e.NewValue as MINIService.CPDPM_DirSourceClass;
                Globals.CPDPM_Source_FullInfo_ViewList.Clear();

                foreach (var v in Globals.client.GetCPDPM_Source_FullInfo_ViewInfo("AS_SC_Id = " + model._sc_id))
                    Globals.CPDPM_Source_FullInfo_ViewList.Add(new AssetInfo()
                    {
                        _as_guid = v._as_guid,
                        _as_name = v._as_name,
                        _as_url = v._as_url,
                        _sc_id = v._sc_id,
                        _asf_name = v._asf_name,
                        /*加载中使用的缩略图*/
                        _as_thumbnail = fileInfo.FullName

                    });
                itemListControl1.Items.Clear();
                foreach (var v in Globals.CPDPM_Source_FullInfo_ViewList)
                {
                    if (filetype.Contains(v._asf_name.Replace(".", "")))
                        itemListControl1.Items.Add(v);
                }
                //后台下载缩略图
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThumbnail));
                thread.Start();

            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        //下载缩略图
        void DownloadThumbnail()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Globals.NullIcon);
                string assetpath, thumbnailpath;
                byte[] buffer;
                foreach (var v in Globals.CPDPM_Source_FullInfo_ViewList)
                {
                    thumbnailpath = Globals.appStartupPath + "\\" + Globals.thumbnailFolder + "\\" + v._as_guid.ToString() + "_s.png";
                    assetpath = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + v._as_guid.ToString() + v._asf_name;

                    //检查缩略图是否存在,如存在则跳过,否则下载缩略图.
                    if (!System.IO.File.Exists(thumbnailpath))
                    {
                        buffer = Globals.client.DownLoadAssetThumbnail(Guid.Parse(v._as_guid));
                        if (buffer != null)
                        {
                            using (FileStream fs = new FileStream(thumbnailpath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                fs.Write(buffer, 0, buffer.Length);
                                fs.Close();
                            }
                        }
                        else
                            System.IO.File.Copy(Globals.NullIcon, thumbnailpath);
                    }
                    //更新素材路径到本地
                    v._as_url = assetpath;
                    //更改缩略图路径到本地
                    if (System.IO.File.Exists(thumbnailpath))
                        v._as_thumbnail = thumbnailpath;
                    else
                    {
                        //无缩略图。
                        v._as_thumbnail = fileInfo.FullName;
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        //选择素材进行下载
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ShowNewWin.Visibility = Visibility.Visible;
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task((Action)delegate
            {
                try
                {
                    Grid grid = sender as Grid;
                    if (grid == null) return;
                    AssetInfo model=new AssetInfo();
                    this.Dispatcher.Invoke((Action)delegate { model = grid.DataContext as AssetInfo; });
                    

                    if (model == null) return;

                    byte[] buffer;
                    if (!File.Exists(model._as_url))
                    {
                        FileStream fs = new FileStream(model._as_url, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        fs.Seek(fs.Length, SeekOrigin.Current);


                        while ((buffer = Globals.client.DownLoadAsset(Guid.Parse(model._as_guid), fs.Length)).Length > 0)
                        {
                            fs.Write(buffer, 0, buffer.Length);
                        }

                        fs.Flush();
                        fs.Close();

                    }


                    if (Bpage)
                    {
                        
                        _AssInfo.AssetName = model._as_name;
                        _AssInfo.AssetPath = model._as_url;
                        _AssInfo.Thumbnails = model._as_thumbnail;




                    }
                    thumbnails = model._as_thumbnail;
                    path = model._as_url;

                    this.Dispatcher.Invoke((Action)delegate { ShowNewWin.Visibility = Visibility.Visible; this.DialogResult = true; });

                }
                catch (Exception ex)
                
                {
                    log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    log.Error(ex.Message + "\r\n" + ex.StackTrace);
                }

            });
            task.Start();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Foreground = Brushes.Black;
            ((TextBox)sender).SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAAAAAA"));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;
            if (string.IsNullOrEmpty(txt.Text)) return;
            try
            {

                FileInfo fileInfo = new FileInfo(Globals.AssetLoadingIcon);

                Globals.CPDPM_Source_FullInfo_ViewList.Clear();

                foreach (var v in Globals.client.GetCPDPM_Source_FullInfo_ViewInfo("AS_NAME LIKE '%" + txt.Text + "%'"))
                    Globals.CPDPM_Source_FullInfo_ViewList.Add(new AssetInfo()
                    {
                        _as_guid = v._as_guid,
                        _as_name = v._as_name,
                        _as_url = v._as_url,
                        _sc_id = v._sc_id,
                        _asf_name = v._asf_name,
                        /*加载中使用的缩略图*/
                        _as_thumbnail = fileInfo.FullName
                    });
                itemListControl1.Items.Clear();
                foreach (var v in Globals.CPDPM_Source_FullInfo_ViewList)
                {
                    if (filetype.Contains(v._asf_name.Replace(".", "")))
                        itemListControl1.Items.Add(v);
                }
                //后台下载缩略图
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(DownloadThumbnail));
                thread.Start();

            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox_TextChanged(searchBox, null);
        }
    }
    public class AssetInfo : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;


        private string as_thumbnail = "";
        public int _sc_id { get; set; }
        public string _as_guid { get; set; }
        public string _as_url { get; set; }
        public string _as_thumbnail
        {
            get { return as_thumbnail; }
            set
            {
                as_thumbnail = value;
                OnPropertyChanged("_as_thumbnail");
            }
        }
        public string _as_name { get; set; }
        public string _asf_name { get; set; }
    }
}