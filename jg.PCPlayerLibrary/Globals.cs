
namespace jg.PCPlayerLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Windows.Threading;
    using System.Threading;
    using System.IO;
    using System.Diagnostics;
    using jg.Editor.Library;
    using System.Xml.Serialization;
    public class Globals
    {

        //public static readonly string appStartupPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        public static FilePackage filePackage = new FilePackage();


        //应用程序所在目录 
        public static readonly string appStartupPath = Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf("\\"));

        public static string thumbnailFolder = "Thumbnail";
        public static string assetFolder = "Asset";

        public static List<SavePageInfo> savePageList = new List<SavePageInfo>(); //页面信息
        public static ObservableCollection<TreeViewItemInfo> treeviewSource = new ObservableCollection<TreeViewItemInfo>();//目录信息

        public static string SavePath = "";
        public static string TempFolder = "";
        public static void InitializeComponent()
        {

            #region 双击打开的文件，直接加载。

            if (Globals.SavePath != "")
            {
                Release();
                Load(Globals.TempFolder);
            }

            #endregion

        }
        private static void Release()
        {
            Globals.TempFolder = appStartupPath + "\\" + SavePath.Substring(SavePath.LastIndexOf("\\") + 1);
            //filePackage.Process += filePackage_Process;
            filePackage.Release(Globals.SavePath, Globals.TempFolder);
        }
        private static void Load(string path)
        {
            XmlSerializer xmlSerializer;
            // 树目录
            xmlSerializer = new XmlSerializer(typeof(ObservableCollection<TreeViewItemInfo>));
            using (System.IO.FileStream fs = new System.IO.FileStream(path + "\\TreeSource.xml", System.IO.FileMode.OpenOrCreate))
            {
                Globals.treeviewSource = (ObservableCollection<TreeViewItemInfo>)xmlSerializer.Deserialize(fs);
                fs.Flush();
                fs.Close();

            }
            // 舞台集合
            xmlSerializer = new XmlSerializer(typeof(List<SavePageInfo>), new Type[] { typeof(SaveItemInfo), 
                    typeof(ContentShape), 
                    typeof(ContentGrid),
                    typeof(ContentText),
                    typeof(ContentLine),
                    typeof(ContentMessage),
                    typeof(ContentTopicDrag),
                    typeof(abstractContent),
                    typeof(ContentGridItem),
                    typeof(AssetActionInfo),
                    typeof(TimeLineItemInfo), 
                    typeof(TimePoint),
                    typeof(ContentTpageGroup),
                    typeof(abstractAssetProperty),
                    typeof(AssetDoubleProperty), 
                    typeof(AssetColorProperty),  });


            using (System.IO.FileStream fs = new System.IO.FileStream(path + "\\Content.xml", System.IO.FileMode.Open))
            {
                try
                {
                    Globals.savePageList = (List<SavePageInfo>)xmlSerializer.Deserialize(fs);

                    DesignerCanvas canvas;

                    //修改素材路径到本地
                    foreach (var v in Globals.savePageList)
                    {
                        foreach (var item in v.saveItemList)
                        {
                            //表格控件需要单独处理
                            if (item.assetType == AssetType.TextGrid)
                            {
                                foreach (var vv in ((ContentGrid)item.Content).List)
                                {
                                    if (vv.Children == null) continue;
                                    if (!string.IsNullOrEmpty(vv.Children.AssetPath))
                                        vv.Children.AssetPath = Globals.TempFolder + "\\" + vv.Children.AssetPath;
                                    if (!string.IsNullOrEmpty(vv.Children.Thumbnails))
                                        vv.Children.Thumbnails = Globals.TempFolder + "\\" + vv.Children.Thumbnails;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.AssetPath))
                                    item.AssetPath = Globals.TempFolder + "\\" + item.AssetPath;
                                if (!string.IsNullOrEmpty(item.Thumbnails))
                                    item.Thumbnails = Globals.TempFolder + "\\" + item.Thumbnails;
                                if (item.assetType == AssetType.HTML5)
                                {
                                    if (!string.IsNullOrEmpty(item.ItemsDis))
                                    {
                                        string ItemdisOld = item.ItemsDis;
                                        item.ItemsDis = Globals.TempFolder + "\\" + item.ItemsDis;
                                        string ItemdisFile = Globals.TempFolder + "\\" + ItemdisOld;
                                        if (Directory.Exists(ItemdisFile))
                                        {
                                            DirectoryInfo di = new DirectoryInfo(ItemdisFile);
                                            di.Delete(true);
                                        }

                                        Directory.CreateDirectory(ItemdisFile);
                                        string TempFile = ItemdisFile + "\\";

                                        string DecFile = Globals.TempFolder + "\\";


                                        GZip.Decompress(DecFile, TempFile, ItemdisOld + ".zip");
                                    }
                                }
                            }
                        }
                    }

                    fs.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                }
            }

        }

    }
}