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
    /// ContentGrid的值
    /// </summary>
    [Serializable]
    public class ContentGridItem
    {
        [XmlAttribute("Row")]
        public int Row { get; set; }

        [XmlAttribute("Column")]
        public int Column { get; set; }

        [XmlAttribute("RowSpan")]
        public int RowSpan { get; set; }

        [XmlAttribute("ColumnSpan")]
        public int ColumnSpan { get; set; }

        [XmlAttribute("Content")]
        public string Content { get; set; }

        public SaveItemInfo Children { get; set; }
    }
}
