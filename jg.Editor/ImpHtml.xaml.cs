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
    /// ImpHtml.xaml 的交互逻辑
    /// </summary>
    /// 
    public delegate void setSteageBrower(HTML5Class model);
  
    public partial class ImpHtml : Window
    {
        public event setSteageBrower eventsetSteageBrower;
        public ImpHtml()
        {
            InitializeComponent();
        }
        HTML5Class _htmlModel = null;
        public ImpHtml(HTML5Class htmlModel)
        {
            InitializeComponent();
            _htmlModel = htmlModel;
            eventsetSteageBrower = null;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtHtmlDis.Text != "" && txtHtmlFile.Text != ""&&txtHtmlImg.Text!="")
            {
                _htmlModel.ActionHtmlDis = txtHtmlDis.Text;
                _htmlModel.ActionHtmlfile = txtHtmlFile.Text;
                _htmlModel.ImgFileName = txtHtmlImg.Text;

                //移动启动文件（复制）
                int htmlNameImgIdex = _htmlModel.ImgFileName.LastIndexOf('\\');
                //获取HTML5文件名字
                if (htmlNameImgIdex > 0)
                {
                    string Img = _htmlModel.ImgFileName.Substring(htmlNameImgIdex + 1);
                    string assetpath = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + Img;
                    FileSecurity.StreamToFileInfo(assetpath, _htmlModel.ImgFileName);
                    _htmlModel.ImgFileName = assetpath;

                }
                //移动启动文件（复制）
                int htmlNameIdex = _htmlModel.ActionHtmlfile.LastIndexOf('\\');
                //获取HTML5文件名字
                if (htmlNameIdex > 0)
                {
                    _htmlModel.FileName = _htmlModel.ActionHtmlfile.Substring(htmlNameIdex + 1);
                   string  assetpath = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" +  _htmlModel.FileName;
                   FileSecurity.StreamToFileInfo(assetpath,_htmlModel.ActionHtmlfile);

                   _htmlModel.ActionHtmlfile = assetpath;
                }

                //移动源目录（复制）
                int htmlDisNameIdex = _htmlModel.ActionHtmlDis.LastIndexOf('\\');
                if (htmlDisNameIdex > 0)
                {
                    string DisName = _htmlModel.ActionHtmlDis.Substring(htmlDisNameIdex + 1);
                    FileSecurity.CopyFolderTo(_htmlModel.ActionHtmlDis,Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + DisName);
                    _htmlModel.ActionHtmlDis = Globals.appStartupPath + "\\" + Globals.assetFolder + "\\" + DisName;
                }
                

                if (eventsetSteageBrower != null)
                {
                    eventsetSteageBrower(_htmlModel);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("请检查您的源文件夹和启动文件、源缩略图是否都获取了");
            }


        }



        private void SetAsset()
        {
           

        }


        private void BtnDisDiaLog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBDDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (FBDDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtHtmlDis.Text = FBDDialog.SelectedPath;


            }
        }

        private void BtnFileDiaLog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog sfDialog = new System.Windows.Forms.OpenFileDialog();
            sfDialog.Filter = global::jg.Editor.Properties.Resources.ExtensionServerHTMLFilter;
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
