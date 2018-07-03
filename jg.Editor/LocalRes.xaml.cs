using jg.Editor.Library;
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
    /// LocalRes.xaml 的交互逻辑
    /// </summary>
    public partial class LocalRes : Window
    {

        public LocalRes()
        {
            InitializeComponent();
        }

        private string path, thumbnails;
        //素材路径
        public string Path { get { return path; } }
        //缩略图路径
        public string Thumbnails { get { return thumbnails; } }

        private string FilePathFilter = "";
        public LocalRes(string _FilePathFilter)
        {
            InitializeComponent();
            FilePathFilter = _FilePathFilter;
        }

        bool IsTpage=false;

        AssResInfo _AssInfo = new AssResInfo();
        public LocalRes(string _FilePathFilter, bool b, out AssResInfo info)
        {
            InitializeComponent();
            IsTpage = b;
            info = _AssInfo;
            FilePathFilter = _FilePathFilter;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

           
            if (txtHtmlFile.Text != "" && txtHtmlImg.Text != "")
            {

                path = txtHtmlFile.Text;
                thumbnails = txtHtmlImg.Text;

        
                int htmlNameImgIdex = thumbnails.LastIndexOf('\\');

                string NewName = "";
                if (htmlNameImgIdex > 0)
                {
                    string Img = thumbnails.Substring(htmlNameImgIdex + 1);

                    int indexd=Img.LastIndexOf('.');

                    string Expender = Img.Substring(indexd);

                     NewName = Guid.NewGuid().ToString();


                     string ImgName = NewName + "_s" + Expender;

                     string assetpath = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + ImgName;
                    FileSecurity.StreamToFileInfo(assetpath, thumbnails);
                    thumbnails = assetpath;
                   
                }
              
                int htmlNameIdex = path.LastIndexOf('\\');
              
                if (htmlNameIdex > 0)
                {
                   string pathTmp = path.Substring(htmlNameIdex + 1);


                   int indexd = pathTmp.LastIndexOf('.');

                   string Expender = pathTmp.Substring(indexd);

                   string AssName = "";

                   if (NewName != "")
                   {
                       AssName = NewName + Expender;
                   }
                   else

                   {
                       NewName = Guid.NewGuid().ToString();
                       AssName = NewName + Expender;
                   }
                   string assetpath = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + AssName;

                    FileSecurity.StreamToFileInfo(assetpath, path);

                    path = assetpath;
                  
                }

                if (IsTpage)
                {
                    _AssInfo.AssetPath = path;
                    _AssInfo.AssetName = NewName;
                    _AssInfo.Thumbnails = thumbnails;
                }
                this.DialogResult = true;
                this.Close();

            }
            else
            {
                MessageBox.Show("请检查您的源文件、源缩略图是否都获取了");
            }


        }

        private void BtnFileDiaLog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog sfDialog = new System.Windows.Forms.OpenFileDialog();
            sfDialog.Filter = FilePathFilter;
            if (sfDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                txtHtmlFile.Text = sfDialog.FileName;
            }
        }

        private void BtnImgDiaLog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog sfDialog = new System.Windows.Forms.OpenFileDialog();
            sfDialog.Filter = global::jg.Editor.Properties.Resources.ExtensionServerImgFilter;
            if (sfDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtHtmlImg.Text = sfDialog.FileName;
            }
        }
    }
}
