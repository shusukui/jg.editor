using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace jg.Editor.Library
{
    [Serializable]
    public class ContentTpageGroup : abstractContent
    {
        [XmlAttribute("ImgCount")]
        public int ImgCount { get; set; }

        [XmlAttribute("ShowHeight")]
        public double ShowHeight { get; set; }

        [XmlAttribute("ShowWidth")]
        public double ShowWidth { get; set; }



        public  ObservableCollection<AssResInfo> Children { get; set; }
    }

    public class AssResInfo : INotifyPropertyChanged
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

        public AssResInfo()
        {
            ArId = Guid.NewGuid();
        }
        private Guid _ArId;

        public Guid ArId
        {
            get { return _ArId; }
            set { _ArId = value; }
        }

        private string _AssetPath;

        /// <summary>
        /// 资源
        /// </summary>
        public string AssetPath
        {
            get { return _AssetPath; }
            set { _AssetPath = value; }
        }
        private string _Thumbnails;

        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumbnails
        {
            get { return _Thumbnails; }
            set { _Thumbnails = value; }
        }

        private string _AssetName;

        /// <summary>
        /// 名称
        /// </summary>
        public string AssetName
        {
            get { return _AssetName; }
            set { _AssetName = value; }
        }

        private int _Index;

        /// <summary>
        /// 索引键
        /// </summary>
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }


        private bool _IsChecked=false;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { 
                _IsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
        
    }
}
