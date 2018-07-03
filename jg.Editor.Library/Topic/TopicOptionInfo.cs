using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows;
namespace jg.Editor.Library.Topic
{
    [Serializable]
    public class TopicOptionInfo : INotifyPropertyChanged
    {
        public TopicOptionInfo()
        {
        }
        public TopicOptionInfo(int id, int index, string title, bool right, Guid material)
            : this()
        {
            this.Id = id;
            this.Index = index;
            this.Title = title;
            this.Right = right;
            this.Material = material;

        }
        private int id = -1;
        /// <summary>
        /// 编号
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get { return id; } set { id = value; OnPropertyChanged("Id"); } }

        private int index = -1;
        /// <summary>
        /// 选项顺序
        /// </summary>
        [XmlAttribute("Index")]
        public int Index { get { return index; } set { index = value; OnPropertyChanged("Index"); } }

        private string title = "";
        /// <summary>
        /// 选项内容
        /// </summary>
        [XmlAttribute("Title")]
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged("Title"); }
        }
        private bool right = false;
        /// <summary>
        /// 是否正确
        /// </summary>
        [XmlAttribute("Right")]
        public bool Right
        {
            get { return right; }
            set { right = value;
                
                OnPropertyChanged("Right"); }
        }

        private Guid material = new Guid();
        /// <summary>
        /// 选项对应的素材编号
        /// </summary>
        [XmlAttribute("Material")]
        public Guid Material { get { return material; } set { material = value; OnPropertyChanged("Material"); } }
        private bool? isselected = false;



        private Visibility _RightVisibility=Visibility.Collapsed;

        /// <summary>
        /// 正确答案提交以后，正确答案显示
        /// </summary>
        public Visibility RightVisibility
        {
            get { return _RightVisibility; }
            set { _RightVisibility = value; OnPropertyChanged("RightVisibility"); }
        }
        /// <summary>
        /// 用户是否选中
        /// </summary>
        [XmlAttribute("IsSelected")]
        public bool? IsSelected
        {
            get { return isselected; }
            set
            {
                isselected = value;
                OnPropertyChanged("IsSelected");
            }
        }

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