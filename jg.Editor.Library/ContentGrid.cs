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
    /// 素材类型为表格类型的类
    /// </summary>
    [Serializable]
    public class ContentGrid : abstractContent
    {
        public ContentGrid()
        {
            List = new List<ContentGridItem>();
        }

        [XmlAttribute("RowCount")]
        public int RowCount { get; set; }

        [XmlAttribute("ColumnCount")]
        public int ColumnCount { get; set; }

        [XmlAttribute("BorderWidth")]
        public int BorderWidth { get; set; }

        public List<ContentGridItem> List { get; set; }
    }
}
