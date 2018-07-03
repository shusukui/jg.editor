using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
namespace jg.Editor.Library.Topic
{
    [Serializable]
    public enum TopicType
    {
        /// <summary>
        /// 判断题
        /// </summary>
        Judge,
        /// <summary>
        /// 单选题
        /// </summary>
        Single,
        /// <summary>
        /// 多选题
        /// </summary>
        Multiple
    }

    [Serializable]
    public class TopicInfo : INotifyPropertyChanged
    {
        public TopicInfo()
        {
            topicoptionlist.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(topicoptionlist_CollectionChanged);
        }

        void topicoptionlist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (TopicOptionInfo v in e.NewItems)
                    v.PropertyChanged += TopicOptionInfo_PropertyChanged;
        }

        public TopicInfo(int id, string title, ObservableCollection<TopicOptionInfo> topicoptionlist, bool optionrand, float score, Guid material, TopicType topictype)
            : this()
        {
            this.Id = id;
            this.Title = title;
            this.TopicOptionList = topicoptionlist;
            this.OptionRand = optionrand;
            this.Score = score;
            this.Material = material;
            this.TopicType = topictype;
        }

        public void Clear()
        {
            foreach (var v in TopicOptionList)
            {
                v.PropertyChanged -= TopicOptionInfo_PropertyChanged;
                v.Right = false;
                v.PropertyChanged += TopicOptionInfo_PropertyChanged;
            }
        }


        private int id = -1;
        /// <summary>
        /// 编号
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get { return id; } set { id = value; } }

        private string title = "";
        /// <summary>
        /// 题干
        /// </summary>
        [XmlAttribute("Title")]
        public string Title { get { return title; } set { title = value; } }
        
        private ObservableCollection<TopicOptionInfo> topicoptionlist = new ObservableCollection<TopicOptionInfo>();
        /// <summary>
        /// 选项集合
        /// </summary>
        [XmlElement("TopicOptionList")]
        public ObservableCollection<TopicOptionInfo> TopicOptionList
        {
            get { return topicoptionlist; }
            set
            {
                topicoptionlist = value;
                foreach (var v in value)
                    v.PropertyChanged += TopicOptionInfo_PropertyChanged;
            }
        }
        void TopicOptionInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //bool ischeck=false;
            //TopicOptionInfo info = sender as TopicOptionInfo;
            //if (info == null) return;
            //foreach (var v in topicoptionlist)
            //{
            //    v.PropertyChanged -= TopicOptionInfo_PropertyChanged;
            //    if (topictype != Topic.TopicType.Multiple) v.Right = false;
            //}
            //if (topictype == Topic.TopicType.Multiple)
            //{
            //    foreach (var v in topicoptionlist)
            //        if (v.Right == true)
            //        {
            //            ischeck = true;
            //            break; ;
            //        }
            //    if (ischeck != true)
            //        info.Right = true;
            //}
            //else
            //    info.Right = true;

            //foreach (var v in topicoptionlist)
            //    v.PropertyChanged += TopicOptionInfo_PropertyChanged;
        }

        private bool optionrand = false;
        /// <summary>
        /// 选项随机
        /// </summary>
        [XmlAttribute("OptionRand")]
        public bool OptionRand { get { return optionrand; } set { optionrand = value; } }

        private float score = 0;
        /// <summary>
        /// 分值
        /// </summary>
        [XmlAttribute("Score")]
        public float Score { get { return score; } set { score = value; } }

        private Guid material = new Guid();
        /// <summary>
        /// 选项对应的素材编号
        /// </summary>
        [XmlAttribute("Material")]
        public Guid Material { get { return material; } set { material = value; } }

        private TopicType topictype = TopicType.Single;
        /// <summary>
        /// 题目类型
        /// </summary>
        [XmlAttribute("TopicType")]
        public TopicType TopicType { get { return topictype; } set { topictype = value; } }

        private int optionCount = 2;
        [XmlAttribute("OptionCount")]
        public int OptionCount { get { return optionCount; } set { optionCount = value; OnPropertyChanged("OptionCount"); } }


        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}