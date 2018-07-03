using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
namespace jg.Editor.Topic
{
    [Serializable]
    public class TopicDragInfo
    {

        private int id = -1;
        [XmlAttribute("Id")]
        public int Id { get { return id; } set { id = value; } }

        private string title = "";
        [XmlAttribute("Title")]
        public string Title { get { return title; } set { title = value; } }

        private ObservableCollection<TopicDragItem> leftoption = new ObservableCollection<TopicDragItem>();
        [XmlElement("LeftOption")]
        public ObservableCollection<TopicDragItem> LeftOption
        {
            get { return leftoption; }
            set { leftoption = value; }
        }

        private ObservableCollection<TopicDragItem> rightoption = new ObservableCollection<TopicDragItem>();
        [XmlElement("RightOption")]
        public ObservableCollection<TopicDragItem> RightOption
        {
            get { return rightoption; }
            set { rightoption = value; }
        }
    }
}
