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
    /// 记录素材动作
    /// </summary>
    [Serializable]
    public class AssetActionInfo
    {
        /// <summary>
        /// 事件
        /// </summary>
        [XmlAttribute("AssetEvent")]
        public enumAssetEvent AssetEvent { get; set; }
        /// <summary>
        /// 动作
        /// </summary>
        [XmlAttribute("AssetAction")]
        public enumAssetAction AssetAction { get; set; }
        /// <summary>
        /// 素材名称
        /// </summary>
        [XmlAttribute("AssetName")]
        public string AssetName { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        [XmlAttribute("Time")]
        public double Time { get; set; }
    }
}
