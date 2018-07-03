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
    /// ControlLine控件的值
    /// </summary>
    [Serializable]
    public class ContentLine : abstractContent
    {
        [XmlAttribute("Point1X")]
        public double Point1X { get; set; }
        [XmlAttribute("Point1Y")]
        public double Point1Y { get; set; }

    }
}
