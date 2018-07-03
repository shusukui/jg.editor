using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.IO;

namespace jg.Editor.Library
{
    [Serializable]
    public class ContentTopicDrag : abstractContent
    {
        public ContentTopicDrag()
        {
            topicDragItemAnswerList = new List<Control.TopicDragItemAnswerInfo>();
            topicDragItemList = new List<Control.TopicDragItemInfo>();

        }
        [XmlAttribute("Background")]
        public string Background { get; set; }
        [XmlAttribute("Foreground")]
        public string Foreground { get; set; }
        [XmlAttribute("Score")]
        public double Score { get; set; }

        public List<Control.TopicDragItemAnswerInfo> topicDragItemAnswerList { get; set; }
        public List<Control.TopicDragItemInfo> topicDragItemList { get; set; }
    }
}
