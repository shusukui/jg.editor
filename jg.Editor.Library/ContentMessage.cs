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
    /// <summary>
    /// ControlMessage控件的值
    /// </summary>
    [Serializable]
    public class ContentMessage : abstractContent
    {
        [XmlAttribute("PointX")]
        public double PointX { get; set; }

        [XmlAttribute("PointY")]
        public double PointY { get; set; }

        [XmlAttribute("Title")]
        public string Title { get; set; }
    }
}
