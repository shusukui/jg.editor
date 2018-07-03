using jg.Editor.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace jg.PCPlayerLibrary
{
    [Serializable]
    public class Entrance //: Application
    {
        public delegate void OnReleaseProcessing(double value);
        public event OnReleaseProcessing ReleaseProcessing = null;
        public MainWindow windowPreview { get; set; }

        public double Score
        {
            get
            {
                double score = 0;
                if (windowPreview != null)
                {
                    foreach (var v in windowPreview.pageStageList)
                        foreach (var vv in v.canvas.Children.OfType<ToolboxItem>())
                        {
                            switch (vv.Content.GetType().Name)
                            {
                                case "ControlTopicDrag":
                                    jg.Editor.Library.Control.ControlTopicDrag controlTopicDrag = vv.Content as jg.Editor.Library.Control.ControlTopicDrag;
                                    if (controlTopicDrag != null)
                                        score += controlTopicDrag.UserScore;
                                    break;
                                case "TopicControl":
                                    jg.Editor.Library.Topic.TopicControl topicControl = vv.Content as jg.Editor.Library.Topic.TopicControl;
                                    if (topicControl != null)
                                        score += topicControl.UserScore;
                                    break;
                            }
                        }
                }
                return score;
            }
        }
        public double TotalScore
        {
            get
            {
                double score = 0;

                foreach (var v in Globals.savePageList)
                {
                    foreach (var vv in v.saveItemList)
                    {
                        switch (vv.assetType)
                        {
                            case AssetType.TopicDrag:
                            case AssetType.Topic:
                                score += vv.Score;
                                break;
                        }
                    }
                }
                return score;
            }
        }
        public string Path { get; set; }
        public string CourseName { get; set; }


        public Entrance(string path)
        {
            Path = path;
            //设置课件标题
            SetCourseName(path);
            jg.Editor.Library.Globals.AssetDecryptKey = "2-1655469";
            XmlSerializer xmlSerializer;

            xmlSerializer = new XmlSerializer(typeof(AssetActionInfo));

            xmlSerializer = new XmlSerializer(typeof(TimePoint), new Type[] { typeof(abstractAssetProperty), typeof(AssetDoubleProperty), typeof(AssetColorProperty) });
            
            Globals.SavePath = path;

            Globals.InitializeComponent();
            Globals.filePackage.Process += filePackage_Process;

        }


        /// <summary>
        /// 设置课件标题
        /// </summary>
        /// <param name="stringPath">路径</param>
        public void SetCourseName(string stringPath)
        {
            int lastBiasIndex= stringPath.LastIndexOf("\\");
            int lastSpotIndex = stringPath.LastIndexOf(".");
            string parmName = stringPath.Substring(lastBiasIndex+1, lastSpotIndex - lastBiasIndex-1);
            CourseName = parmName;
        }
        void filePackage_Process(double value)
        {
            if (ReleaseProcessing != null) ReleaseProcessing(value);
        }
        //public void Initialize(string path)
        //{
        //    Path = path;
        //    jg.Editor.Library.Globals.AssetDecryptKey = "2-1655469";
        //    XmlSerializer xmlSerializer;

        //    xmlSerializer = new XmlSerializer(typeof(AssetActionInfo));

        //    xmlSerializer = new XmlSerializer(typeof(TimePoint), new Type[] { typeof(abstractAssetProperty), typeof(AssetDoubleProperty), typeof(AssetColorProperty) });


        //    Globals.SavePath = path;

        //    Globals.InitializeComponent();

        //    //App app = new App();
        //    //app.InitializeComponent();
        //    //app.Run();
        //    WindowPreview windowsPreview = new WindowPreview();
        //    windowsPreview.ShowDialog();
        //}
    }
}